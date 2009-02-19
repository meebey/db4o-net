/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	/// <summary>Strategy for file/byte array growth that will always double the current size
	/// 	</summary>
	public class DoublingGrowthStrategy : IGrowthStrategy
	{
		public virtual long NewSize(long curSize)
		{
			return curSize * 2;
		}
	}
}
