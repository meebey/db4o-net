using System;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Internal
{
	[System.Serializable]
	public class OpenDatabaseException : Db4oException
	{
		public OpenDatabaseException(Exception cause) : base(cause)
		{
		}
	}
}
