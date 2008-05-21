/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Drs;

namespace Db4objects.Drs.Inside
{
	/// <summary>A default implementation of ConflictResolver.</summary>
	/// <remarks>
	/// A default implementation of ConflictResolver. In case of a conflict
	/// a
	/// <see cref="ReplicationConflictException">ReplicationConflictException</see>
	/// is thrown.
	/// </remarks>
	/// <author>Albert Kwan</author>
	/// <author>Carl Rosenberger</author>
	/// <author>Klaus Wuestefeld</author>
	/// <version>1.0</version>
	/// <since>dRS 1.0</since>
	public class DefaultReplicationEventListener : IReplicationEventListener
	{
		public virtual void OnReplicate(IReplicationEvent e)
		{
		}
	}
}
