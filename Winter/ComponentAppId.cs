#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System.Collections.Immutable;
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

        public static ComponentAppId Parse(string str)
        {
            var p = str.Split([','])
                .Select(p => p.Trim());

            if (!p.Any()) throw new ArgumentException();

            var props = p.Skip(1).Select(p => p.Split(['='], 2))
                .ToDictionary(p => p[0], p => p[1]);

            return new ComponentAppId
            {
                Name = p.First(),
                Culture = props.GetValueOrDefault("Culture"),
                Version = props.GetValueOrDefault("Version"),
                ProcessorArchitecture = props.GetValueOrDefault("ProcessorArchitecture"),
                VersionScope = props.GetValueOrDefault("versionScope"),
                PublicKeyToken = props.GetValueOrDefault("PublicKeyToken"),
                Type = props.GetValueOrDefault("Type")
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
                | ((string.IsNullOrEmpty(Culture) || Culture == "neutral") ? 0 
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
                HashAppend(ref hash, HashNameAndValue("name", this.Name));
            }
            if (flags.HasFlag(NameHashFlags.Culture) && this.Culture != null)
            {
                // en-US
                HashAppend(ref hash, HashNameAndValue("culture", this.Culture));
            }
            if (flags.HasFlag(NameHashFlags.TypeName))
            {
                throw new NotImplementedException();
                HashAppend(ref hash, HashNameAndValue("typeName", FIXME_TODO));
            }
            if (flags.HasFlag(NameHashFlags.Type) && this.Type != null)
            {
                HashAppend(ref hash, HashNameAndValue("Type", this.Type));
            }


            if (flags.HasFlag(NameHashFlags.Version) && this.Version != null)
            {
                // 10.0.19041.3570
                HashAppend(ref hash, HashNameAndValue("version", this.Version));
            }
            if (flags.HasFlag(NameHashFlags.PublicKeyToken) && this.PublicKeyToken != null)
            {
                // 31bf3856ad364e35
                HashAppend(ref hash, HashNameAndValue("PublicKeyToken", this.PublicKeyToken));
            }

            //var fallbackProcessorArch = "data";
            if (flags.HasFlag(NameHashFlags.ProcessorArchitecture) && this.ProcessorArchitecture != null)
            {
                // amd64
                HashAppend(ref hash, HashNameAndValue("processorArchitecture", this.ProcessorArchitecture));
            }
            if (flags.HasFlag(NameHashFlags.VersionScope) && this.VersionScope != null)
            {
                // NonSxS
                HashAppend(ref hash, HashNameAndValue("versionScope", this.VersionScope));
            }
            return hash;
        }

        public override string ToString()
        {
            return $"{Name}, " +
                $"Culture={Culture}, " +
                $"Version={Version}, " +
                $"PublicKeyToken={PublicKeyToken}, " +
                $"ProcessorArchitecture={ProcessorArchitecture}, " +
                $"versionScope={VersionScope}";
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

                var sb = new StringBuilder();
                sb.Append(str.Substring(0, mid));
                sb.Append(dotdot);
                sb.Append(str.Substring(right_start, mid));
                str = sb.ToString();
            }
            return str;
        }

        public string GetSxSName(bool compact = true)
        {
            /** GenerateKeyFormIntoBuffer_LHFormat **/

            Console.WriteLine(Name);

            var hash = GetHash(GetNameFlags()).ToString("x16");
            var culture = Culture == "neutral"
                ? "none" 
                : Culture;

            var output = new StringBuilder();
            output.Append(ProcessorArchitecture);
            output.Append('_');
            output.Append(GetSanitizedString(Name, compact ? 40 : -1));
            output.Append('_');
            output.Append(PublicKeyToken);
            output.Append('_');
            output.Append(Version);
            output.Append('_');
            output.Append(GetSanitizedString(culture, compact ? 8 : -1));
            output.Append('_');
            output.Append(hash);
            return output.ToString();
        }

        [GeneratedRegex(@"[ \(\)]")]
        private static partial Regex CharsToRemove();
    }
}
