/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
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

		public override IPreparedComparison PrepareComparison(object obj)
		{
			return Null.Instance;
		}

		public sealed override object Read(IInternalReadContext context)
		{
			return null;
		}
	}
}
