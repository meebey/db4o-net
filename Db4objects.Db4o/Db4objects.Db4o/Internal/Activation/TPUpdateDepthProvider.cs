/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Internal.Activation
{
	public class TPUpdateDepthProvider : IUpdateDepthProvider
	{
		public virtual FixedUpdateDepth ForDepth(int depth)
		{
			return new TPFixedUpdateDepth(depth, false);
		}

		public virtual UnspecifiedUpdateDepth Unspecified(bool tpCommitMode)
		{
			return new TPUnspecifiedUpdateDepth(tpCommitMode);
		}
	}
}
