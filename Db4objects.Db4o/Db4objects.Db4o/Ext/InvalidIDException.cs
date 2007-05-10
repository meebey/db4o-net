using System;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	[System.Serializable]
	public class InvalidIDException : Db4oException
	{
		public InvalidIDException(Exception e) : base(e)
		{
		}
	}
}
