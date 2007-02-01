/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Inside
{
    abstract internal class YapTypeStruct : YapTypeAbstract {

		public YapTypeStruct(Db4objects.Db4o.Inside.ObjectContainerBase stream)
			: base(stream)
		{
        }

        public override bool IsEqual(Object compare, Object with){
            // TODO: Does == work here? Check !
            return compare.Equals(with);
        }

    }
}
