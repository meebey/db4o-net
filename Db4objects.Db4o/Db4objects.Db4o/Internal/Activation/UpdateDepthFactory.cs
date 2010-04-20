/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Internal.Activation
{
	public class UpdateDepthFactory
	{
		public static IUpdateDepth ForDepth(int depth)
		{
			if (depth == Const4.Unspecified)
			{
				return UnspecifiedUpdateDepth.Instance;
			}
			return new FixedUpdateDepth(depth);
		}

		private UpdateDepthFactory()
		{
		}
	}
}
