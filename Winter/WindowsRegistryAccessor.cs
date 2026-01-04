using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace Smx.Winter;

public class WindowsRegistryAccessor : IDisposable
{
    private WindowsSystem _windows;

    private ManagedRegistryKey? _systemHive;
    private ManagedRegistryKey? _softwareHive;
    private ManagedRegistryKey? _componentsHive;

    public ManagedRegistryKey ComponentsHive
    {
        get
        {
            if(_componentsHive == null)
            {
                _componentsHive = LoadHive("COMPONENTS");
            }
            return _componentsHive;
        }
    }

    public ManagedRegistryKey SystemHive {
        get
        {
            if(_systemHive == null)
            {
                _systemHive = LoadHive("SYSTEM");
            }
            return _systemHive;
        }
    }

    public ManagedRegistryKey SoftwareHive
    {
        get
        {
            if(_softwareHive == null)
            {
                _softwareHive = LoadHive("SOFTWARE");
            }
            return _softwareHive;
        }
    }

    private static void EnsurePrivileges()
    {
        Util.EnablePrivileges([PInvoke.SE_BACKUP_NAME, PInvoke.SE_RESTORE_NAME]);
    }

    public WindowsRegistryAccessor(WindowsSystem windows)
    {
        _windows = windows;
        EnsurePrivileges();
    }

    private ManagedRegistryKey LoadHive(string hiveName)
    {
        var isComponentsHive = hiveName.Equals("COMPONENTS", StringComparison.InvariantCultureIgnoreCase);
        if (_windows.IsOnline && !isComponentsHive)
        {
            return ManagedRegistryKey.Open(@$"HKEY_LOCAL_MACHINE\{hiveName}");
        }

        var uuid = $"{Util.Djb2Hash(_windows.SystemRoot):x8}";
        var keyName = $"Winter_{uuid}_{hiveName}";

        if (_windows.IsOnline && isComponentsHive)
        {
            keyName = "COMPONENTS";
        }

        var mountPath = @$"HKEY_LOCAL_MACHINE\{keyName}";

        if (ManagedRegistryKey.Exists(mountPath))
        {
            return ManagedRegistryKey.Open(mountPath);
        }

        var hivePath = Path.Combine(_windows.SystemRoot, "System32", "Config", hiveName);
        using var hKey = new SafeRegistryHandle((nint)RegistryHive.LocalMachine, true);
        WIN32_ERROR err;
        if((err=PInvoke.RegLoadKey(hKey, keyName, hivePath)) != WIN32_ERROR.ERROR_SUCCESS)
        {
            throw new Win32Exception((int)err, "RegLoadKey failed");
        }

        return ManagedRegistryKey.Open(mountPath);
    }

    public void Dispose()
    {
        using var hKey = new SafeRegistryHandle((nint)RegistryHive.LocalMachine, true);

        if (_systemHive != null)
        {
            PInvoke.RegUnLoadKey(hKey, _systemHive.Name);
        }
        if(_softwareHive != null)
        {
            PInvoke.RegUnLoadKey(hKey, _softwareHive.Name);
        }
        if(_componentsHive != null)
        {
            PInvoke.RegUnLoadKey(hKey, _componentsHive.Name);
        }

        _systemHive?.Dispose();
    }
}
