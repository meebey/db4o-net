/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;

namespace Db4objects.Db4o
{
	/// <summary>Field MetaData to be stored to the database file.</summary>
	/// <remarks>
	/// Field MetaData to be stored to the database file.
	/// Don't obfuscate.
	/// </remarks>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class MetaField : IInternal4
	{
		public string name;

		public MetaIndex index;
	}
}
