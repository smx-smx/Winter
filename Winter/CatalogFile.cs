using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Win32.SafeHandles;
using Smx.SharpIO;
using Smx.SharpIO.Extensions;
using Smx.SharpIO.Memory;
using System.Buffers.Text;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Security.Cryptography;
using Windows.Win32.Security.Cryptography.Catalog;
using Windows.Win32.Security.Cryptography.Sip;
using Windows.Win32.Security.WinTrust;

namespace Smx.Winter;

public class CatalogCryptoAttribute
{

    public readonly string? ObjectId;
    public readonly byte[] Value;

    public CatalogCryptoAttribute(CRYPT_ATTRIBUTE_TYPE_VALUE native)
    {
        ObjectId = native.pszObjId.ToString();

        nint pData = 0;
        unsafe
        {
            pData = new nint(native.Value.pbData);
        }

        Value = new byte[native.Value.cbData];
        if(pData != 0 && Value.Length > 0)
        {
            Marshal.Copy(pData, Value, 0, Value.Length);
        }
    }
}

public class CatalogAttribute
{
    public readonly byte[] Data;
    public readonly string? ReferenceTag;
    public readonly CRYPTCATATTRIBUTE_FLAGS Flags;

    public string? DecodeAsString()
    {
        if (!Flags.HasFlag(CRYPTCATATTRIBUTE_FLAGS.CRYPTCAT_ATTR_NAMEASCII))
            return null;

        if (Flags.HasFlag(CRYPTCATATTRIBUTE_FLAGS.CRYPTCAT_ATTR_DATAASCII))
        {
            return new UnicodeEncoding(false, false).GetString(Data, 0, Data.Length - sizeof(char));
        }
        throw new NotImplementedException();
    }   

    public CatalogAttribute(CRYPTCATATTRIBUTE native)
    {
        nint pData = 0;
        unsafe
        {
            pData = new nint(native.pbValue);
        }

        Flags = native.dwAttrTypeAndAction;
        Data = new byte[native.cbValue];
        if (pData != 0 && Data.Length > 0)
        {
            Marshal.Copy(pData, Data, 0, Data.Length);
        }

        ReferenceTag = native.pwszReferenceTag.ToString();
    }
}

public class CatalogSubjectIndirectData
{
    public readonly CRYPT_ALGORITHM_IDENTIFIER Algorithm;
    public readonly byte[] Digest;


    public CatalogSubjectIndirectData(SIP_INDIRECT_DATA native)
    {
        Algorithm = native.DigestAlgorithm;

        nint pDigest = 0;
        unsafe
        {
            pDigest = new nint(native.Digest.pbData);
        }
        Digest = new byte[native.Digest.cbData];
        if(pDigest != 0 && Digest.Length > 0)
        {
            Marshal.Copy(pDigest, Digest, 0, Digest.Length);
        }
    }
}

public class CryptAlgorithmIdentifier
{
    public readonly string? Algorithm;
    public readonly byte[] Parameters;

    public CryptAlgorithmIdentifier(CRYPT_ALGORITHM_IDENTIFIER native)
    {
        Algorithm = native.pszObjId.ToString();

        nint pParams = 0;
        unsafe
        {
            pParams = new nint(native.Parameters.pbData);
        }
        Parameters = new byte[native.Parameters.cbData];
        if(pParams != 0 && Parameters.Length > 0)
        {
            Marshal.Copy(pParams, Parameters, 0, Parameters.Length);
        }
    }
}

public class CatalogSpcData
{
    public readonly byte[] Value;
    public readonly byte[] Digest;
    public readonly string? ObjectId;
    public readonly CryptAlgorithmIdentifier DigestAlgorithm;

    public CatalogSpcData(SPC_INDIRECT_DATA_CONTENT native)
    {
        nint pValue = 0;
        nint pDigest = 0;
        unsafe
        {
            pValue = new nint(native.Data.Value.pbData);
            pDigest = new nint(native.Digest.pbData);
        }
        Value = new byte[native.Data.Value.cbData];
        Digest = new byte[native.Digest.cbData];

        if (pValue != 0 && Value.Length > 0)
        {
            Marshal.Copy(pValue, Value, 0, Value.Length);
        }
        if(pDigest != 0 && Digest.Length > 0)
        {
            Marshal.Copy(pDigest, Digest, 0, Digest.Length);
        }

        ObjectId = native.Data.pszObjId.ToString();
        DigestAlgorithm = new CryptAlgorithmIdentifier(native.DigestAlgorithm);
    }
}

public class CatalogItem
{
    public readonly uint CertVersion;
    public readonly Guid SubjectType;
    public readonly string? ReferenceTag;
    public readonly string? FileName;
    public readonly uint MemberFlags;
    public readonly byte[] EncodedIndirectData;
    public readonly byte[] EncodedMemberInfo;

    public readonly ImmutableList<CatalogAttribute> Attributes;
    public readonly CatalogSpcData? SpcData;

    private readonly CatalogFile _file;

    public override string ToString()
    {
        return $"CatalogItem(FileName=\"{FileName}\", ReferenceTag=\"{ReferenceTag}\")";
    }

    public CatalogItem(CatalogFile file, CRYPTCATMEMBER native, IEnumerable<CatalogAttribute> attributes)
    {
        _file = file;

        CertVersion = native.dwCertVersion;
        SubjectType = native.gSubjectType;
        ReferenceTag = native.pwszReferenceTag.ToString();
        FileName = native.pwszFileName.ToString();
        MemberFlags = native.fdwMemberFlags;

        nint pIndirectData = 0;
        nint pMemberInfo = 0;
        unsafe
        {
            pIndirectData = new nint(native.sEncodedIndirectData.pbData);
            pMemberInfo = new nint(native.sEncodedMemberInfo.pbData);
        }


        EncodedIndirectData = new byte[native.sEncodedIndirectData.cbData];
        if (pIndirectData != 0 && EncodedIndirectData.Length > 0)
        {
            Marshal.Copy(pIndirectData, EncodedIndirectData, 0, EncodedIndirectData.Length);
        }



        EncodedMemberInfo = new byte[native.sEncodedMemberInfo.cbData];
        if (pMemberInfo != 0 && EncodedMemberInfo.Length > 0)
        {
            Marshal.Copy(pMemberInfo, EncodedMemberInfo, 0, EncodedMemberInfo.Length);
        }

        uint infoSize = 0;
        SPC_INDIRECT_DATA_CONTENT data = default;

        if (native.sEncodedIndirectData.cbData > 0)
        {
            unsafe
            {
                if (!PInvoke.CryptDecodeObject(
                    CERT_QUERY_ENCODING_TYPE.X509_ASN_ENCODING | CERT_QUERY_ENCODING_TYPE.PKCS_7_ASN_ENCODING,
                    PInvoke.SPC_INDIRECT_DATA_CONTENT_STRUCT,
                    native.sEncodedIndirectData.pbData,
                    native.sEncodedIndirectData.cbData,
                    0,
                    null,
                    &infoSize
                ))
                {
                    throw new Win32Exception();
                }

                using var hMem = MemoryHGlobal.Alloc(infoSize);

                if (!PInvoke.CryptDecodeObject(
                    CERT_QUERY_ENCODING_TYPE.X509_ASN_ENCODING | CERT_QUERY_ENCODING_TYPE.PKCS_7_ASN_ENCODING,
                    PInvoke.SPC_INDIRECT_DATA_CONTENT_STRUCT,
                    native.sEncodedIndirectData.pbData,
                    native.sEncodedIndirectData.cbData,
                    0,
                    hMem.Address.ToPointer(),
                    &infoSize
                ))
                {
                    throw new Win32Exception();
                }


                data = hMem.Span.Cast<SPC_INDIRECT_DATA_CONTENT>()[0];
            }

            SpcData = new CatalogSpcData(data);
        }

        Attributes = ImmutableList.Create<CatalogAttribute>(
            new Span<CatalogAttribute>(attributes.ToArray()));
    }
}

public class CryptCatHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public CryptCatHandle(bool ownsHandle) : base(ownsHandle)
    {
    }

    public CryptCatHandle(SafeFileHandle handle, bool ownsHandle) : base(ownsHandle)
    {
        this.handle = handle.DangerousGetHandle();
    }

    protected override bool ReleaseHandle()
    {
        if (IsInvalid) return false;
        return PInvoke.CryptCATClose(this);
    }
}

public class CryptCatSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    public CryptCatSafeHandle(HANDLE handle, bool ownsHandle) : base(ownsHandle)
    {
        this.handle = handle;
    }

    protected override bool ReleaseHandle()
    {
        return PInvoke.CryptCATClose(new HANDLE(this.handle));
    }
}

public class CatalogFile : IDisposable
{
    private readonly SafeHandle _handle;

    public CatalogFile(string filePath)
    {
        var encoding = new UnicodeEncoding(false, false);

        using var hMem = MemoryHGlobal.Alloc(encoding.GetByteCount(filePath) + 2);
        encoding.GetBytes(filePath).CopyTo(hMem.Memory);

        // the SafeHandle variant of CryptCATOpen is broken (closes the handle incorrectly)
        // use the raw one so we can wrap the HANDLE and use CryptCATClose
        var hCat = PInvoke.CryptCATOpen(
            hMem.ToPWSTR(),
            CRYPTCAT_OPEN_FLAGS.CRYPTCAT_OPEN_EXISTING,
            nuint.Zero,
            CRYPTCAT_VERSION.CRYPTCAT_VERSION_2,
            0);

        if (hCat == -1 || hCat == 0)
        {
            hCat = PInvoke.CryptCATOpen(
                hMem.ToPWSTR(),
                CRYPTCAT_OPEN_FLAGS.CRYPTCAT_OPEN_EXISTING,
                nuint.Zero,
                CRYPTCAT_VERSION.CRYPTCAT_VERSION_2,
                0);
            if (hCat == -1 || hCat == 0)
            {
                throw new InvalidOperationException($"Failed to open catalogue \"{filePath}\"");
            }
        }

        _handle = new CryptCatSafeHandle(hCat, ownsHandle: true);
    }

    private unsafe HANDLE Handle => new HANDLE(_handle.DangerousGetHandle());

    private IEnumerable<CatalogAttribute> ReadAttributes(TypedPointer<CRYPTCATMEMBER> item)
    {
        var attr = new TypedPointer<CRYPTCATATTRIBUTE>();
        unsafe
        {
            attr = new TypedPointer<CRYPTCATATTRIBUTE>(
                new nint(PInvoke.CryptCATEnumerateAttr(Handle, item.ToPointer(), null)));
        }

        if(attr.Address == 0)
        {
            yield break;
        }
        yield return new CatalogAttribute(attr.Value);

        while (true)
        {
            unsafe
            {
                attr = new TypedPointer<CRYPTCATATTRIBUTE>(
                    new nint(PInvoke.CryptCATEnumerateAttr(Handle, item.ToPointer(), attr.ToPointer())));
            }
            if(attr.Address == 0)
            {
                yield break;
            }
            yield return new CatalogAttribute(attr.Value);
        }
    }

    public IEnumerable<CatalogItem> Members
    {
        get
        {
            var item = new TypedPointer<CRYPTCATMEMBER>();
            unsafe
            {
                item = new TypedPointer<CRYPTCATMEMBER>(
                    new nint(PInvoke.CryptCATEnumerateMember(Handle, null)));
            }

            if(item.Address == 0)
            {
                yield break;
            }

            var attrs = ReadAttributes(item);
            yield return new CatalogItem(this, item.Value, attrs);

            while (true)
            {
                unsafe
                {
                    item = new TypedPointer<CRYPTCATMEMBER>(
                        new nint(PInvoke.CryptCATEnumerateMember(Handle, item.ToPointer())));
                }
                if (item.Address == 0)
                {
                    yield break;
                }

                attrs = ReadAttributes(item);
                yield return new CatalogItem(this, item.Value, attrs);
            }
        }   
    }

    public void Dispose()
    {
        _handle.Dispose();
    }
}
