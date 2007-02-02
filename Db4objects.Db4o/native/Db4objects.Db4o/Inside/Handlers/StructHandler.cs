/* Copyright (C) 2004 - 2007   db4objects Inc.   http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Inside.Handlers
{
    abstract public class StructHandler : NetTypeHandler {

		public StructHandler(ObjectContainerBase containerBase)
			: base(containerBase)
		{
        }

        public override bool IsEqual(Object compare, Object with){
            // TODO: Does == work here? Check !
            return compare.Equals(with);
        }

    }
}
