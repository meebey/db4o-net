/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class NullFieldMetadata : FieldMetadata
	{
		public NullFieldMetadata() : base(null)
		{
		}

		public override IComparable4 PrepareComparison(object obj)
		{
			return Null.INSTANCE;
		}

		internal override object Read(MarshallerFamily mf, StatefulBuffer a_bytes)
		{
			return null;
		}

		public override object ReadQuery(Transaction a_trans, MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 a_reader)
		{
			return null;
		}
	}
}
