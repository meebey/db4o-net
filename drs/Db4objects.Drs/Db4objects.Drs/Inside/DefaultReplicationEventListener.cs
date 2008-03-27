/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Inside
{
	/// <summary>A default implementation of ConflictResolver.</summary>
	/// <remarks>
	/// A default implementation of ConflictResolver. In case of a conflict
	/// a
	/// <see cref="Db4objects.Drs.ReplicationConflictException">Db4objects.Drs.ReplicationConflictException
	/// 	</see>
	/// is thrown.
	/// </remarks>
	/// <author>Albert Kwan</author>
	/// <author>Carl Rosenberger</author>
	/// <author>Klaus Wuestefeld</author>
	/// <version>1.0</version>
	/// <since>dRS 1.0</since>
	public class DefaultReplicationEventListener : Db4objects.Drs.IReplicationEventListener
	{
		public virtual void OnReplicate(Db4objects.Drs.IReplicationEvent e)
		{
		}
	}
}
