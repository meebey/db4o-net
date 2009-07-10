/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4oUnit.Extensions.Fixtures;

namespace Db4oUnit.Extensions.Fixtures
{
	public abstract class AbstractFileBasedDb4oFixture : AbstractSoloDb4oFixture
	{
		private readonly Sharpen.IO.File _databaseFile;

		public AbstractFileBasedDb4oFixture(string fileName)
		{
			_databaseFile = new Sharpen.IO.File(fileName);
		}

		public virtual string GetAbsolutePath()
		{
			return _databaseFile.GetAbsolutePath();
		}

		protected override void DoClean()
		{
			_databaseFile.Delete();
		}
	}
}
