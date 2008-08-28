/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Tests.Common.Migration;

namespace Db4objects.Db4o.Tests.Common.Migration
{
	/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
	public class Db4oLibrary
	{
		public readonly string path;

		public readonly Db4oLibraryEnvironment environment;

		public Db4oLibrary(string path, Db4oLibraryEnvironment environment)
		{
			this.path = path;
			this.environment = environment;
		}
	}
}
