#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Microsoft.Diagnostics.Tracing.Parsers.ApplicationServer;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using Smx.SharpIO;
using Smx.SharpIO.Extensions;
using Smx.SharpIO.Memory;
using System;
using System.Buffers.Binary;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Registry;

namespace Smx.Winter;

public struct RegistryKeyInfo
{
    public int NumberOfSubKeys;
    public int MaxSubkeyLength;
    public int MaxClassLength;
    public int NumberOfValues;
    public int MaxValueNameLength;
    public int MaxValueLength;
}

public struct RegistryValue
{
    public object Value;
    public REG_VALUE_TYPE Type;
}


public class ManagedRegistryKey : IDisposable
{
    private readonly SafeHandle keyHandle;
    public string Path { get; private set; }
    public string Name => System.IO.Path.GetFileName(Path);

    public RegistryKeyInfo CachedInfo { get; private set; }

    private ManagedRegistryKey(SafeRegistryHandle handle, string fullPath)
    {
        this.keyHandle = handle;
        this.Path = fullPath;
        this.CachedInfo = UpdateCachedInfo();
    }


    public ManagedRegistryKey this[string sub]
    {
        get
        {
            return Open(System.IO.Path.Combine(Path, sub));
        }
    }

    public ManagedRegistryKey OpenChildKey(string sub, REG_SAM_FLAGS flags = DEFAULT_FLAGS)
    {
        return Open($"{Path}\\{sub}", flags);
    }

    private RegistryKeyInfo GetKeyNameLimits()
    {
        uint cSubKeys = 0;
        uint cbMaxSubkeyLength = 0;
        uint cbMaxClassLength = 0;
        uint cValues = 0;
        uint cbMaxValueNameLength = 0;
        uint cbMaxValueLength = 0;

        unsafe
        {
            PInvoke.RegQueryInfoKey(
                keyHandle, null, null,
                &cSubKeys,
                &cbMaxSubkeyLength,
                &cbMaxClassLength,
                &cValues,
                &cbMaxValueNameLength,
                &cbMaxValueLength,
                null, null
            );
        }

        return new RegistryKeyInfo
        {
            MaxClassLength = (int)cbMaxClassLength,
            MaxSubkeyLength = (int)cbMaxSubkeyLength,
            MaxValueLength = (int)cbMaxValueLength,
            MaxValueNameLength = (int)cbMaxValueNameLength,
            NumberOfSubKeys = (int)cSubKeys,
            NumberOfValues = (int)cValues
        };
    }

    private WIN32_ERROR EnumKey(
        uint i,
        NativeMemoryHandle bufKeyName, out string keyName,
        NativeMemoryHandle bufKeyClass, out string keyClass
    )
    {
        WIN32_ERROR res;
        uint cchName = (uint)bufKeyName.Size;
        uint cchClass = (uint)bufKeyClass.Size;

        var pKeyName = bufKeyName.ToPWSTR();
        var pKeyClass = bufKeyClass.ToPWSTR();

        unsafe
        {
            res = PInvoke.RegEnumKeyEx(
                new HKEY(keyHandle.DangerousGetHandle()), i,
                pKeyName, &cchName, null,
                pKeyClass, &cchClass, null
            );
        }
        if (res != WIN32_ERROR.ERROR_SUCCESS)
        {
            throw new InvalidOperationException();
        }
        keyName = pKeyName.ToString();
        keyClass = pKeyClass.ToString();
        return res;
    }

    private WIN32_ERROR EnumValue(
        uint i,
        NativeMemoryHandle bufValueName, out string valueName
    )
    {
        uint type;

        WIN32_ERROR res;
        uint cchValueName = (uint)bufValueName.Size;
        var pValueName = bufValueName.ToPWSTR();

        unsafe
        {
            res = PInvoke.RegEnumValue(
                new HKEY(keyHandle.DangerousGetHandle()), i,
                pValueName, &cchValueName,
                null, &type, null, null
            );
        }
        if (res != WIN32_ERROR.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)res, "Failed to enumerate value");
        }

        valueName = new string(pValueName.AsSpan().Slice(0, (int)cchValueName));
        return res;
    }

    public bool TryGetValue<T>(string valueName, [MaybeNullWhen(false)] out T value)
    {
        try
        {
            value = GetValue<T>(valueName);
            if (value == null)
            {
                return false;
            }
            return true;
        }
        catch (Win32Exception ex)
        {
            value = default;
            if (ex.ErrorCode == (int)WIN32_ERROR.ERROR_OBJECT_NOT_FOUND)
            {
                return false;
            } else
            {
                throw;
            }
        }
    }

    public T? GetValue<T>(string valueName)
    {
        var v = GetValue(valueName, out var type);
        var t = typeof(T);
        if (t.IsAssignableFrom(typeof(string)))
        {
            if (type != REG_VALUE_TYPE.REG_SZ) throw new ArgumentException();
        }
        else if (t.IsAssignableFrom(typeof(uint)) || t.IsAssignableFrom(typeof(int)))
        {
            switch (type)
            {
                case REG_VALUE_TYPE.REG_DWORD_LITTLE_ENDIAN:
                case REG_VALUE_TYPE.REG_DWORD_BIG_ENDIAN:
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        else if (t.IsAssignableFrom(typeof(long)) || t.IsAssignableFrom(typeof(ulong)))
        {
            if (type != REG_VALUE_TYPE.REG_QWORD) throw new ArgumentException();
        }
        else if (t.IsAssignableFrom(typeof(string[])))
        {
            switch (type)
            {
                case REG_VALUE_TYPE.REG_EXPAND_SZ:
                case REG_VALUE_TYPE.REG_MULTI_SZ:
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        return (T?)v;
    }

    private byte[] GetValueBytes<T>(REG_VALUE_TYPE valueType, T value)
    {
        switch (valueType)
        {
            case REG_VALUE_TYPE.REG_NONE:
            case REG_VALUE_TYPE.REG_BINARY:
                if (value is not byte[] vb)
                {
                    throw new ArgumentException("not a byte array value");
                }
                return vb;
            case REG_VALUE_TYPE.REG_DWORD_BIG_ENDIAN:
            case REG_VALUE_TYPE.REG_DWORD_LITTLE_ENDIAN:
                var isLittle = valueType == REG_VALUE_TYPE.REG_DWORD_LITTLE_ENDIAN;
                uint vu = Convert.ToUInt32(value);
                var dword = BitConverter.GetBytes(vu);
                return isLittle
                    ? dword
                    : Enumerable.Reverse(dword).ToArray();

            case REG_VALUE_TYPE.REG_QWORD_LITTLE_ENDIAN:
                var vl = Convert.ToUInt64(value);
                return BitConverter.GetBytes(vl);

            case REG_VALUE_TYPE.REG_SZ:
            case REG_VALUE_TYPE.REG_EXPAND_SZ:
            case REG_VALUE_TYPE.REG_LINK:
                if(value is not string vs)
                {
                    throw new ArgumentException("not a string");
                }
                return new UnicodeEncoding(false, false).GetBytes(vs);
            case REG_VALUE_TYPE.REG_MULTI_SZ:
                var ms = new MemoryStream();
                var bw = new BinaryWriter(ms);
                var strings = value as IEnumerable<string>;
                if(strings == null)
                {
                    throw new ArgumentException("not a string enumerable");
                }
                var encoding = new UnicodeEncoding(false, false);
                foreach(var str in strings)
                {
                    bw.Write(encoding.GetBytes(str));
                    bw.Write((ushort)0);
                }
                return ms.ToArray();
            default:
                throw new NotImplementedException(Enum.GetName(valueType));
        }
    }

    public ManagedRegistryKey CreateKey(string keyName, REG_SAM_FLAGS flags = REG_SAM_FLAGS.KEY_WOW64_64KEY)
    {
        var keyDisp = new REG_CREATE_KEY_DISPOSITION();
        WIN32_ERROR res;
        SafeRegistryHandle? resultKeyHandle;
        unsafe
        {
            res = PInvoke.RegCreateKeyEx(
                keyHandle, keyName, null, REG_OPEN_CREATE_OPTIONS.REG_OPTION_BACKUP_RESTORE, flags, null,
                out resultKeyHandle, &keyDisp);
        }
        if(res != WIN32_ERROR.NO_ERROR || resultKeyHandle == null)
        {
            throw new Win32Exception((int)res, $"Cannot create key {Path}\\{keyName}"); 
        }
        UpdateCachedInfo();
        return new ManagedRegistryKey(resultKeyHandle, $"{Path}\\{keyName}");
    }

    public void SetValue<T>(string valueName, REG_VALUE_TYPE valueType, T value)
    {
        var bytes = GetValueBytes(valueType, value);
        var res = PInvoke.RegSetValueEx(keyHandle, valueName, valueType, bytes);
        if(res != WIN32_ERROR.NO_ERROR)
        {
            throw new Win32Exception((int)res, $"Failed to write value {Path}\\{valueName}");
        }
    }

    public object? GetValue(
        string valueName,
        out REG_VALUE_TYPE type,
        REG_ROUTINE_FLAGS flags = REG_ROUTINE_FLAGS.RRF_RT_ANY)
    {
        type = default;
        uint cbData = 0;
        WIN32_ERROR res;
        REG_VALUE_TYPE type_tmp;
        unsafe
        {
            res = PInvoke.RegGetValue(
                keyHandle, null, valueName,
                flags, &type_tmp, null, &cbData);

            type = type_tmp;
        }
        if (res != WIN32_ERROR.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)res, $"Failed to read value {valueName}");
        }

        using var buf = MemoryHGlobal.Alloc((nint)cbData);
        unsafe
        {
            res = PInvoke.RegGetValue(
                keyHandle, null, valueName,
                flags, &type_tmp, buf.Address.ToPointer(), &cbData
            );
        }

        PWSTR pStr;

        switch (type)
        {
            case REG_VALUE_TYPE.REG_NONE:
            case REG_VALUE_TYPE.REG_BINARY:
                return buf.Span.ToArray();
            case REG_VALUE_TYPE.REG_DWORD_LITTLE_ENDIAN:
            case REG_VALUE_TYPE.REG_DWORD_BIG_ENDIAN:
                if (buf.Span.Length < sizeof(uint)) return null;
                var isLittleDw = type == REG_VALUE_TYPE.REG_DWORD_LITTLE_ENDIAN;
                return isLittleDw
                    ? BinaryPrimitives.ReadUInt32LittleEndian(buf.Span)
                    : BinaryPrimitives.ReadUInt32BigEndian(buf.Span);
            case REG_VALUE_TYPE.REG_QWORD:
                if (buf.Span.Length < sizeof(ulong)) return null;
                var isLittleQw = type == REG_VALUE_TYPE.REG_QWORD_LITTLE_ENDIAN;
                return isLittleQw
                    ? BinaryPrimitives.ReadUInt64LittleEndian(buf.Span)
                    : BinaryPrimitives.ReadUInt64BigEndian(buf.Span);
            case REG_VALUE_TYPE.REG_MULTI_SZ:
                List<string> stringArray = new List<string>();
                int iCh = 0;
                var nCh = cbData / sizeof(char);
                while (iCh < nCh)
                {
                    unsafe
                    {
                        pStr = new PWSTR((char*)buf.Address.ToPointer() + iCh);
                    }
                    var str = pStr.ToString();
                    iCh += (str.Length + 1);

                    if (str.Length == 0 && iCh >= cbData) { }
                    else stringArray.Add(str);
                }
                return stringArray.ToArray();
            case REG_VALUE_TYPE.REG_SZ:
            case REG_VALUE_TYPE.REG_EXPAND_SZ:
            case REG_VALUE_TYPE.REG_LINK:
                unsafe
                {
                    pStr = new PWSTR((char*)buf.Address.ToPointer());
                }
                return pStr.ToString();
            /*
            case REG_VALUE_TYPE.REG_RESOURCE_LIST:
            case REG_VALUE_TYPE.REG_RESOURCE_REQUIREMENTS_LIST:
            case REG_VALUE_TYPE.REG_DWORD_BIG_ENDIAN:
            case REG_VALUE_TYPE.REG_FULL_RESOURCE_DESCRIPTOR:
                break;
            */
            default:
                if (Enum.IsDefined(type))
                {
                    throw new NotImplementedException("Type not implemented: " + Enum.GetName(type));
                } else
                {
                    // SAM types and other binary data
                    return buf.Span.ToArray();
                }
        }

        return null;
    }


    private RegistryKeyInfo UpdateCachedInfo()
    {
        CachedInfo = GetKeyNameLimits();
        return CachedInfo;
    }

    private IEnumerable<string> GetKeyNames()
    {
        var info = CachedInfo;
        for (uint i = 0; i < info.NumberOfSubKeys; i++)
        {
            var cchName = info.MaxSubkeyLength + 1;
            var cchClass = info.MaxClassLength + 1;

            using var bufKeyName = MemoryHGlobal.Alloc(cchName * sizeof(char));
            using var bufKeyClass = MemoryHGlobal.Alloc(cchClass * sizeof(char));

            WIN32_ERROR res = EnumKey(i,
                bufKeyName, out var keyName,
                bufKeyClass, out var keyClass
            );


            switch (res)
            {
                case WIN32_ERROR.ERROR_SUCCESS:
                    yield return keyName;
                    break;
                case WIN32_ERROR.ERROR_NO_MORE_ITEMS:
                default:
                    break;
            }
        }
    }

    private IEnumerable<string> GetValueNames()
    {
        var info = CachedInfo;
        for (uint i = 0; i < info.NumberOfValues; i++)
        {
            var cchValueName = info.MaxValueNameLength + 1;
            using var bufValueName = MemoryHGlobal.Alloc(cchValueName * sizeof(char));

            WIN32_ERROR res = EnumValue(i,
                bufValueName, out var valueName
            );

            switch (res)
            {
                case WIN32_ERROR.ERROR_SUCCESS:
                    yield return valueName;
                    break;
                case WIN32_ERROR.ERROR_NO_MORE_ITEMS:
                default:
                    break;
            }
        }
    }

    public IEnumerable<string> KeyNames
    {
        get => GetKeyNames();
    }

    public IEnumerable<string> ValueNames
    {
        get => GetValueNames();
    }

    public IEnumerable<ManagedRegistryKey> ChildKeys
    {
        get
        {
            foreach(var k in KeyNames)
            {
                var child = OpenChildKey(k);
                yield return child;
            }
        }
    }

    public IEnumerable<RegistryValue> Values
    {
        get
        {
            foreach(var k in ValueNames)
            {
                var v = GetValue(k, out var valueType);
                yield return new RegistryValue
                {
                    Value = v,
                    Type = valueType,
                };
            }
        }
    }

    private const string HKCR = "HKEY_CLASSES_ROOT";
    private const string HKLM = "HKEY_LOCAL_MACHINE";
    private const string HKCU = "HKEY_CURRENT_USER";
    private const string HKU = "HKEY_USERS";
    private const string HKCC = "HKEY_CURRENT_CONFIG";

    private static readonly ImmutableDictionary<string, RegistryHive> HIVE_MAP = ImmutableDictionary.CreateRange(new Dictionary<string, RegistryHive> {
        { HKCR, RegistryHive.ClassesRoot },
        { HKLM, RegistryHive.LocalMachine },
        { HKCU, RegistryHive.CurrentUser },
        { HKU, RegistryHive.Users },
        { "HKEY_PERFORMANCE_DATA", RegistryHive.PerformanceData },
        { "HKEY_CURRENT_CONFIG", RegistryHive.CurrentConfig }
    });

    private static readonly ImmutableDictionary<string, string> HIVE_SHORT_LONG = ImmutableDictionary.CreateRange(new Dictionary<string, string>
    {
        { "HKCR", HKCR },
        { "HKCU", HKCU },
        { "HKLM", HKLM },
        { "HKU", HKU },
        { "HKCC", HKCC }
    });

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>WRITE_DAC and WRITE_OWNER might not be available to SYSTEM</remarks>
    private const REG_SAM_FLAGS DEFAULT_FLAGS = 0
        | REG_SAM_FLAGS.KEY_QUERY_VALUE
        | REG_SAM_FLAGS.KEY_ENUMERATE_SUB_KEYS
        | REG_SAM_FLAGS.KEY_READ
        | REG_SAM_FLAGS.KEY_WRITE // for SetValue
        | REG_SAM_FLAGS.KEY_CREATE_SUB_KEY // for CreateKey
        | REG_SAM_FLAGS.KEY_WOW64_64KEY;

    private static bool TrySplitKeyPath(string path, out RegistryHive hive, out string subKey)
    {
        hive = default;
        subKey = string.Empty;

        var parts = path.Split(['\\'], 2);
        if (parts.Length < 1) return false;

        var root = parts[0];
        if (root.Length <= 4)
        {
            if (!HIVE_SHORT_LONG.TryGetValue(root, out root)) return false;
        }
        if (!HIVE_MAP.TryGetValue(root, out hive)) return false;
        subKey = parts.ElementAtOrDefault(1) ?? string.Empty;

        return true;
    }

    private static (WIN32_ERROR, SafeRegistryHandle) OpenKey(SafeHandle hKey, string subKey, REG_SAM_FLAGS flags = DEFAULT_FLAGS)
    {
        var res = PInvoke.RegOpenKeyEx(hKey, subKey, 0, flags, out var keyHandle);
        if(res == WIN32_ERROR.ERROR_ACCESS_DENIED)
        {
            unsafe
            {
                // $NOTE: for whatever reason, SYSTEM/TI can't use REG_OPTION_BACKUP_RESTORE
                res = PInvoke.RegCreateKeyEx(hKey, subKey, null, 0
                    | REG_OPEN_CREATE_OPTIONS.REG_OPTION_BACKUP_RESTORE
                    | REG_OPEN_CREATE_OPTIONS.REG_OPTION_VOLATILE, flags, null, out keyHandle, null);
            }
        }
        return (res, keyHandle);
    }

    public static ManagedRegistryKey Open(string keyPath, REG_SAM_FLAGS flags = DEFAULT_FLAGS)
    {
        if (!TrySplitKeyPath(keyPath, out var hive, out var subKey))
        {
            throw new ArgumentException($"Invalid key path: \"{keyPath}\"");
        }
        var (err, handle) = OpenKey(new SafeRegistryHandle((nint)hive, true), subKey, flags);
        if (handle.IsInvalid)
        {
            throw new Win32Exception((int)err, $"Failed to open key {keyPath}");
        }
        return new ManagedRegistryKey(handle, keyPath);
    }

    private bool _disposed = false;

    public void Dispose()
    {
        if (!_disposed)
        {
            keyHandle.Dispose();
            _disposed = true;
        }
    }
}
