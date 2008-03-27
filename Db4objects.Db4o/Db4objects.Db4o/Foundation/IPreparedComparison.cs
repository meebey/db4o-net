/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Foundation
{
	/// <summary>
	/// TODO: rename to Comparable4 as soon we find
	/// a smart name for the current Comparable4.
	/// </summary>
	/// <remarks>
	/// TODO: rename to Comparable4 as soon we find
	/// a smart name for the current Comparable4.
	/// </remarks>
	public interface IPreparedComparison
	{
		/// <summary>
		/// return a negative int, zero or a positive int if
		/// the object being held in 'this' is smaller, equal
		/// or greater than the passed object.<br /><br />
		/// Typical implementation: return this.object - obj;
		/// </summary>
		int CompareTo(object obj);
	}
}
