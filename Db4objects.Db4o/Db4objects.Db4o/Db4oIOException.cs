using System;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o
{
	[System.Serializable]
	public class Db4oIOException : Db4oException
	{
		public Db4oIOException() : base()
		{
		}

		public Db4oIOException(Exception e) : base(e.Message, e)
		{
		}
	}
}
