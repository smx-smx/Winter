#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.ApplicationInstallationAndServicing;

namespace Smx.Winter
{
    public class MsPatch
    {
        private delegate Windows.Win32.Foundation.BOOL fnApplyDeltaB(
            long ApplyFlags,
            DELTA_INPUT Source, DELTA_INPUT Delta, out DELTA_OUTPUT lpTarget);

        private const uint PA30_MAGIC = 0x50413330;

        private const long DELTA_FLAG_NONE = 0x00000000;
        private const long DELTA_APPLY_FLAG_ALLOW_PA19 = 0x00000001;

        public static byte[] ApplyPatch(Span<byte> input, Span<byte> patch)
        {
            if (BinaryPrimitives.ReadUInt32BigEndian(patch) != PA30_MAGIC)
            {
                throw new InvalidDataException();
            }

            var dictIn = new DELTA_INPUT
            {
                uSize = (nuint)input.Length,
                Editable = false
            };

            var patchIn = new DELTA_INPUT
            {
                uSize = (nuint)patch.Length,
                Editable = false
            };

            var hLib = PInvoke.LoadLibrary("msdelta.dll");
            var pfnApplyDeltaB = PInvoke.GetProcAddress(hLib, "ApplyDeltaB");
            var pfnTrap = Util.MakeDebuggerTrap<fnApplyDeltaB>(pfnApplyDeltaB);

            var DEBUG = false;

            DELTA_OUTPUT lpTarget;
            unsafe
            {
                fixed(byte* pInput = input)
                fixed(byte* pPatch = patch)
                {
                    dictIn.Anonymous.lpcStart = pInput;
                    patchIn.Anonymous.lpcStart = pPatch;
                    BOOL res;
                    if (DEBUG)
                    {
                        res = pfnTrap(DELTA_FLAG_NONE, dictIn, patchIn, out lpTarget);
                    } else
                    {
                        res = PInvoke.ApplyDeltaB(DELTA_FLAG_NONE, dictIn, patchIn, out lpTarget);
                    }
                    if (!res)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
            }

            var output = new byte[(int)lpTarget.uSize];
            unsafe {
                var targetPtr = new nint(lpTarget.lpStart);
                Marshal.Copy(targetPtr, output, 0, output.Length);
                PInvoke.DeltaFree(lpTarget.lpStart);
            }
            return output;
        }

    }
}
