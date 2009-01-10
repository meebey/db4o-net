/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Migration;

namespace Db4objects.Db4o.Tests.Common.Migration
{
	public class Db4oLibraryEnvironmentProvider
	{
		private readonly Hashtable4 _environments = new Hashtable4();

		private readonly Sharpen.IO.File _classPath;

		public Db4oLibraryEnvironmentProvider(Sharpen.IO.File classPath)
		{
			_classPath = classPath;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual Db4oLibraryEnvironment EnvironmentFor(string path)
		{
			Db4oLibraryEnvironment existing = ExistingEnvironment(path);
			if (existing != null)
			{
				return existing;
			}
			return NewEnvironment(path);
		}

		private Db4oLibraryEnvironment ExistingEnvironment(string path)
		{
			return (Db4oLibraryEnvironment)_environments.Get(path);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private Db4oLibraryEnvironment NewEnvironment(string path)
		{
			Db4oLibraryEnvironment env = new Db4oLibraryEnvironment(new Sharpen.IO.File(path)
				, _classPath);
			_environments.Put(path, env);
			return env;
		}
	}
}
