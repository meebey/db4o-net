/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using Sharpen.Lang;

namespace Db4objects.Db4o {

    abstract internal class YapTypeStruct : YapTypeAbstract {

        public YapTypeStruct(Db4objects.Db4o.YapStream stream) : base(stream) {
        }

        public override bool IsEqual(Object compare, Object with){
            // TODO: Does == work here? Check !
            return compare.Equals(with);
        }

    }
}
