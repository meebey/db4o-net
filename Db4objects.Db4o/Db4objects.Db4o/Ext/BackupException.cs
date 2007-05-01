using System;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	[System.Serializable]
	public class BackupException : Db4oException
	{
		public BackupException(Exception e) : base(e)
		{
		}
	}
}
