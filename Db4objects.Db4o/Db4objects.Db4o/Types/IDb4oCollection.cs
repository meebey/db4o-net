/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Types
{
	/// <summary>base interface for db4o collections</summary>
	public interface IDb4oCollection : IDb4oType
	{
		/// <summary>configures the activation depth for objects returned from this collection.
		/// 	</summary>
		/// <remarks>
		/// configures the activation depth for objects returned from this collection.
		/// &lt;br&gt;&lt;br&gt;Specify a value less than zero to use the default activation depth
		/// configured for the ObjectContainer or for individual objects.
		/// </remarks>
		/// <param name="depth">the desired depth</param>
		void ActivationDepth(int depth);

		/// <summary>
		/// configures objects are to be deleted from the database file if they are
		/// removed from this collection.
		/// </summary>
		/// <remarks>
		/// configures objects are to be deleted from the database file if they are
		/// removed from this collection.
		/// &lt;br&gt;&lt;br&gt;Default value: &lt;code&gt;false&lt;/code&gt;
		/// </remarks>
		/// <param name="flag">the desired setting</param>
		void DeleteRemoved(bool flag);
	}
}
