/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;

namespace Db4objects.Drs
{
	/// <summary>Thrown when a conflict occurs and no ReplicationEventListener is specified.
	/// 	</summary>
	/// <remarks>Thrown when a conflict occurs and no ReplicationEventListener is specified.
	/// 	</remarks>
	/// <author>Albert Kwan</author>
	/// <author>Klaus Wuestefeld</author>
	/// <version>1.2</version>
	/// <seealso cref="Db4objects.Drs.IReplicationEventListener">Db4objects.Drs.IReplicationEventListener
	/// 	</seealso>
	/// <since>dRS 1.2</since>
	[System.Serializable]
	public class ReplicationConflictException : Db4oException
	{
		public ReplicationConflictException(string message) : base(message)
		{
		}
	}
}
