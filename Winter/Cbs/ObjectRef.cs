#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter.Cbs
{
    public enum ObjectType
    {
        File,
        Object,
        Package,
        TypeI,
        TypeS
    }

    public class ObjectRef
    {
        public const char CLASS_FILE = 'f';
        public const char CLASS_COMPONENT = 'c';
        public const char CLASS_PACKAGE = 'p';

        public const char CLASS_I = 'i';
        public const char CLASS_S = 's';

        public string ObjectName { get; set; }
        public ObjectType ObjectType { get; set; }

        public ObjectRef(string objectId)
        {
            if (objectId.Length < 2 || objectId[1] != '!') throw new ArgumentException();
            ObjectType = objectId[0] switch
            {
                CLASS_FILE => ObjectType.File,
                CLASS_COMPONENT => ObjectType.Object,
                CLASS_PACKAGE => ObjectType.Package,
                CLASS_I => ObjectType.TypeI,
                CLASS_S => ObjectType.TypeS,
                _ => throw new NotSupportedException(objectId)
            };
            ObjectName = objectId.Substring(2);
            if (string.IsNullOrWhiteSpace(ObjectName)) throw new ArgumentException();
        }
    }
}
