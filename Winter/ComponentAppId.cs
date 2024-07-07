#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Smx.Winter
{

    [Flags]
    public enum NameHashFlags : uint
    {
        Name = 1 << 0,
        PublicKeyToken = 1 << 1,
        Version = 1 << 2,
        Culture = 1 << 3,
        ProcessorArchitecture = 1 << 4,
        Type = 1 << 5,
        TypeName = 1 << 7,
        VersionScope = 1 << 8
    }

    public partial class ComponentAppId
    {
        public string? Name { get; set; }
        public string? Culture { get; set; }
        public string? Version { get; set; }
        public string? PublicKeyToken { get; set; }
        public string? ProcessorArchitecture { get; set; }
        public string? VersionScope { get; set; }
        public string? Type { get; set; }

        public const string CULTURE_NEUTRAL = "neutral";

        public const string KEY_NAME = "name";
        public const string KEY_CULTURE = "culture";
        public const string KEY_TYPENAME = "typeName";
        public const string KEY_TYPE = "Type";
        public const string KEY_VERSION = "version";
        public const string KEY_PUBLICKEY = "PublicKeyToken";
        public const string KEY_PROCESSOR = "processorArchitecture";
        public const string KEY_VERSIONSCOPE = "versionScope";

        public const string PROP_CULTURE = "Culture";
        public const string PROP_VERSION = "Version";
        public const string PROP_PROCESSOR = "ProcessorArchitecture";
        public const string PROP_TYPE = "Type";
        public const string PROP_VERSIONSCOPE = "versionScope";
        public const string PROP_PUBLICKEY = "PublicKeyToken";

        private const char PROPSEP_PACKAGE = '~';
        private const char PROPSEP_APPID = ',';
        private const char PROPSEP_SXS = '_';

        private const char SEP_KEYVALUE = '=';

        public string ToPackageId()
        {
            var sb = new StringBuilder(Name ?? "");

            var appendProp = (string? prop) =>
            {
                sb.Append(PROPSEP_PACKAGE).Append(prop ?? "");
            };
            appendProp(PublicKeyToken);
            appendProp(ProcessorArchitecture);
            appendProp(Culture);
            appendProp(Version);
            return sb.ToString();
        }

        public static ComponentAppId FromPackageId(string str)
        {
            // Microsoft-Windows-ShellExperienceHost-Package~31bf3856ad364e35~amd64~en-US~10.0.19041.1
            var p = str.Split(PROPSEP_PACKAGE);
            if (p.Length != 5) throw new ArgumentException();

            return new ComponentAppId
            {
                Name = p[0],
                PublicKeyToken = p[1],
                ProcessorArchitecture = p[2],
                Culture = p[3],
                Version = p[4]
            };
        }

        public static ComponentAppId FromAppId(string str)
        {
            var p = str.Split(PROPSEP_APPID)
                .Select(p => p.Trim());

            if (!p.Any()) throw new ArgumentException();

            var props = p.Skip(1).Select(p => p.Split(SEP_KEYVALUE, 2))
                .ToDictionary(p => p[0], p => p[1]);

            return new ComponentAppId
            {
                Name = p.First(),
                Culture = props.GetValueOrDefault(PROP_CULTURE),
                Version = props.GetValueOrDefault(PROP_VERSION),
                ProcessorArchitecture = props.GetValueOrDefault(PROP_PROCESSOR),
                VersionScope = props.GetValueOrDefault(PROP_VERSIONSCOPE),
                PublicKeyToken = props.GetValueOrDefault(PROP_PUBLICKEY),
                Type = props.GetValueOrDefault(PROP_TYPE)
            };
        }

        private const uint HASH_PRIME = 0x1003F;

        private ulong RtlHashEncodedLBlob(string str, bool toLower = true)
        {
            var hashGroups = new uint[4];

            var ich = 0;
            var strEnd = str.Length;
            foreach (var ch in str)
            {
                if (ich >= strEnd) break;

                var groupIdx = (ich % 4);
                var ch2 = (toLower) ? char.ToLowerInvariant(ch) : ch;
                hashGroups[groupIdx] = ch2 + HASH_PRIME * hashGroups[groupIdx];

                ++ich;
            }

            var finalHash = 0UL
                + hashGroups[0] * 0x1E5FFFFFD27UL
                + hashGroups[1] * 0xFFFFFFDC00000051UL
                + hashGroups[2] * 0x1FFFFFFF7UL
                + hashGroups[3] * 1UL;
            return finalHash;
        }

        private const ulong HASH_KEY = 0x1FFFFFFF7;

        private ulong HashNameAndValue(string key, string value)
        {
            return 0
                + RtlHashEncodedLBlob(key) * HASH_KEY
                + RtlHashEncodedLBlob(value);
        }

        private void HashAppend(ref ulong prevHash, ulong hash)
        {
            prevHash = prevHash * HASH_KEY + hash;
        }

        private NameHashFlags GetNameFlags()
        {
            NameHashFlags flags = (0
                | (string.IsNullOrEmpty(Name) ? 0
                    : NameHashFlags.Name)
                | ((string.IsNullOrEmpty(Culture) || Culture == CULTURE_NEUTRAL) ? 0 
                    : NameHashFlags.Culture)
                | (string.IsNullOrEmpty(Type) ? 0 
                    : NameHashFlags.Type)
                | (string.IsNullOrEmpty(PublicKeyToken) ? 0
                    : NameHashFlags.PublicKeyToken)
                | (string.IsNullOrEmpty(Version) ? 0
                    : NameHashFlags.Version)
                | (string.IsNullOrEmpty(VersionScope) ? 0
                    : NameHashFlags.VersionScope)
                | (string.IsNullOrEmpty(ProcessorArchitecture) ? 0
                    : NameHashFlags.ProcessorArchitecture)
            );
            return flags;
        }

        public ulong GetHash(NameHashFlags flags)
        {
            const string FIXME_TODO = "dummy";

            ulong hash = 0;
            if (flags.HasFlag(NameHashFlags.Name) && this.Name != null)
            {
                // Microsoft-Windows-CoreSystem-RemoteFS-Client-Deployment-LanguagePack
                HashAppend(ref hash, HashNameAndValue(KEY_NAME, this.Name));
            }
            if (flags.HasFlag(NameHashFlags.Culture) && this.Culture != null)
            {
                // en-US
                HashAppend(ref hash, HashNameAndValue(KEY_CULTURE, this.Culture));
            }
            if (flags.HasFlag(NameHashFlags.TypeName))
            {
                throw new NotImplementedException();
                HashAppend(ref hash, HashNameAndValue(KEY_TYPENAME, FIXME_TODO));
            }
            if (flags.HasFlag(NameHashFlags.Type) && this.Type != null)
            {
                HashAppend(ref hash, HashNameAndValue(KEY_TYPE, this.Type));
            }


            if (flags.HasFlag(NameHashFlags.Version) && this.Version != null)
            {
                // 10.0.19041.3570
                HashAppend(ref hash, HashNameAndValue(KEY_VERSION, this.Version));
            }
            if (flags.HasFlag(NameHashFlags.PublicKeyToken) && this.PublicKeyToken != null)
            {
                // 31bf3856ad364e35
                HashAppend(ref hash, HashNameAndValue(KEY_PUBLICKEY, this.PublicKeyToken));
            }

            //var fallbackProcessorArch = "data";
            if (flags.HasFlag(NameHashFlags.ProcessorArchitecture) && this.ProcessorArchitecture != null)
            {
                // amd64
                HashAppend(ref hash, HashNameAndValue(KEY_PROCESSOR, this.ProcessorArchitecture));
            }
            if (flags.HasFlag(NameHashFlags.VersionScope) && this.VersionScope != null)
            {
                // NonSxS
                HashAppend(ref hash, HashNameAndValue(KEY_VERSIONSCOPE, this.VersionScope));
            }
            return hash;
        }

        public override string ToString()
        {
            const string sep = ", ";

            var sb = new StringBuilder(Name);

            var appendProp = (string key, string? value) =>
            {
                if (value == null) return;
                sb.Append(sep)
                    .Append(key)
                    .Append(SEP_KEYVALUE)
                    .Append(value);
            };
            appendProp(PROP_CULTURE, Culture);
            appendProp(PROP_VERSION, Version);
            appendProp(PROP_PUBLICKEY, PublicKeyToken);
            appendProp(PROP_PROCESSOR, ProcessorArchitecture);
            appendProp(PROP_VERSIONSCOPE, VersionScope);
            
            return sb.ToString();
        }

        private const NameHashFlags DEFAULT_HASH_FLAGS = 0
            | NameHashFlags.Name
            | NameHashFlags.PublicKeyToken
            | NameHashFlags.Version
            | NameHashFlags.ProcessorArchitecture
            | NameHashFlags.VersionScope;

        private string GetSanitizedString(string str, int maxLength = -1)
        {
            const string dotdot = "..";

            str = CharsToRemove().Replace(str.ToLowerInvariant(), "");

            if (maxLength > 0 && str.Length > maxLength)
            {
                var mid = (maxLength / 2)-1;
                var right_start = str.Length - mid;

                return new StringBuilder()
                    .Append(str.Substring(0, mid))
                    .Append(dotdot)
                    .Append(str.Substring(right_start, mid))
                    .ToString();
            }
            return str;
        }

        public string GetSxSName(bool compact = true)
        {
            /** GenerateKeyFormIntoBuffer_LHFormat **/

            Console.WriteLine(Name);

            var hash = GetHash(GetNameFlags()).ToString("x16");
            var culture = Culture == CULTURE_NEUTRAL
                ? "none" 
                : Culture ?? "";

            var sb = new StringBuilder(ProcessorArchitecture ?? "");

            var appendProp = (string prop) =>
            {
                sb.Append(PROPSEP_SXS).Append(prop);
            };
            appendProp(GetSanitizedString(Name ?? "", compact ? 40 : -1));
            appendProp(PublicKeyToken ?? "");
            appendProp(Version ?? "");
            appendProp(GetSanitizedString(culture, compact ? 8 : -1));
            appendProp(hash);
            return sb.ToString();
        }

        [GeneratedRegex(@"[ \(\)]")]
        private static partial Regex CharsToRemove();
    }
}
