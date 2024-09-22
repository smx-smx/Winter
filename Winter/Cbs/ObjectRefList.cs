#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter.Cbs
{
    public class ObjectRefList
    {
        public static IEnumerable<ObjectRef> FromRegistryKey(ManagedRegistryKey key)
        {
            return key.ValueNames.Where(v => v.Length > 1 && v[1] == '!')
                .Select(v => new ObjectRef(v));
        }
    }
}
