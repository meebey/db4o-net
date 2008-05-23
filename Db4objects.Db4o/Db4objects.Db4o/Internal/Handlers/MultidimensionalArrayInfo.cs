/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Handlers;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class MultidimensionalArrayInfo : ArrayInfo
	{
		private int[] _dimensions;

		public virtual void Dimensions(int[] dim)
		{
			_dimensions = dim;
		}

		public virtual int[] Dimensions()
		{
			return _dimensions;
		}
	}
}
