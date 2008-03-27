/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs
{
	/// <summary>Defines the contract for handling of replication events generated from a replication session.
	/// 	</summary>
	/// <remarks>
	/// Defines the contract for handling of replication events generated from a replication session.
	/// Users can implement this interface to resolve replication conflicts according to their own business rules.
	/// </remarks>
	/// <author>Albert Kwan</author>
	/// <author>Klaus Wuestefeld</author>
	/// <version>1.2</version>
	/// <since>dRS 1.2</since>
	public interface IReplicationEventListener
	{
		/// <summary>invoked when a replication of an object occurs.</summary>
		/// <remarks>invoked when a replication of an object occurs.</remarks>
		/// <param name="e"></param>
		void OnReplicate(Db4objects.Drs.IReplicationEvent e);
	}
}
