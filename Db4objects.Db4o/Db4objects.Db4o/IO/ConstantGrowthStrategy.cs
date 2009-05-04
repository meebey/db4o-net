/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	/// <summary>Strategy for file/byte array growth by a constant factor</summary>
	public class ConstantGrowthStrategy : IGrowthStrategy
	{
		private readonly int _growth;

		/// <param name="growth">The constant growth size</param>
		public ConstantGrowthStrategy(int growth)
		{
			_growth = growth;
		}

		/// <summary>
		/// returns the incremented size after the growth
		/// strategy has been applied
		/// </summary>
		/// <param name="curSize">the original size</param>
		/// <returns>the new size</returns>
		public virtual long NewSize(long curSize)
		{
			return curSize + _growth;
		}
	}
}
