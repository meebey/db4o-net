/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class StringHandler2 : StringHandler
	{
		public StringHandler2(ObjectContainerBase container) : base(container)
		{
		}

		public override object Read(IReadContext context)
		{
			return ReadString(context, context);
		}

		public override void Defragment(IDefragmentContext context)
		{
			context.IncrementOffset(LinkLength());
		}
	}
}
