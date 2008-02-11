/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Config
{
	/// <summary>allows special comparison behaviour during query evaluation.</summary>
	/// <remarks>
	/// allows special comparison behaviour during query evaluation.
	/// &lt;br&gt;&lt;br&gt;db4o will use the Object returned by the &lt;code&gt;compare()&lt;/code&gt;
	/// method for all query comparisons.
	/// </remarks>
	public interface ICompare
	{
		/// <summary>return the Object to be compared during query evaluation.</summary>
		/// <remarks>return the Object to be compared during query evaluation.</remarks>
		object Compare();
	}
}
