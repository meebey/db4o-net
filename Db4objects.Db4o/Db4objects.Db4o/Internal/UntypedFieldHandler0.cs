/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class UntypedFieldHandler0 : UntypedFieldHandler
	{
		public UntypedFieldHandler0(ObjectContainerBase container) : base(container)
		{
		}

		public override object Read(IReadContext context)
		{
			return context.ReadObject();
		}

		public override ObjectID ReadObjectID(IInternalReadContext context)
		{
			int id = context.ReadInt();
			return id == 0 ? ObjectID.IS_NULL : new ObjectID(id);
		}

		public override void Defragment(IDefragmentContext context)
		{
			throw new NotImplementedException();
		}
	}
}
