/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
namespace Db4objects.Drs
{
	/// <summary>The state of the entity in a provider.</summary>
	/// <remarks>The state of the entity in a provider.</remarks>
	/// <author>Albert Kwan</author>
	/// <author>Klaus Wuestefeld</author>
	/// <version>1.2</version>
	/// <since>dRS 1.2</since>
	public interface IObjectState
	{
		/// <summary>The entity.</summary>
		/// <remarks>The entity.</remarks>
		/// <returns>null if the object has been deleted or if it was not replicated in previous replications.
		/// 	</returns>
		object GetObject();

		/// <summary>Is the object newly created since last replication?</summary>
		/// <returns>true when the object is newly created since last replication</returns>
		bool IsNew();

		/// <summary>Was the object modified since last replication?</summary>
		/// <returns>true when the object was modified since last replication</returns>
		bool WasModified();

		/// <summary>The time when the object is modified in a provider.</summary>
		/// <remarks>The time when the object is modified in a provider.</remarks>
		/// <returns>time when the object is modified in a provider.</returns>
		long ModificationDate();
	}
}
