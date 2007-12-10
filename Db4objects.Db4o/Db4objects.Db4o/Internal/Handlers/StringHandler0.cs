/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class StringHandler0 : StringHandler
	{
		public StringHandler0(ITypeHandler4 template) : base(template)
		{
		}

		public override object Read(IReadContext context)
		{
			Db4objects.Db4o.Internal.Buffer buffer = ReadIndirectedBuffer(context);
			if (buffer == null)
			{
				return null;
			}
			return ReadString(context, buffer);
		}

		public override void Delete(IDeleteContext context)
		{
			base.Delete(context);
			context.DefragmentRecommended();
		}
	}
}
