/* This file is part of the db4o object database http://www.db4o.com

Copyright (C) 2004 - 2009  Versant Corporation http://www.versant.com

db4o is free software; you can redistribute it and/or modify it under
the terms of version 3 of the GNU General Public License as published
by the Free Software Foundation.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program.  If not, see http://www.gnu.org/licenses/. */
using Db4objects.Drs;

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
	public class DefaultReplicationEventListener : IReplicationEventListener
	{
		public virtual void OnReplicate(IReplicationEvent e)
		{
		}
	}
}
