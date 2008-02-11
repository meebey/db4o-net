/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions.Fixtures;

namespace Db4oUnit.Extensions.Fixtures
{
	public abstract class AbstractFileBasedDb4oFixture : AbstractSoloDb4oFixture
	{
		private readonly Sharpen.IO.File _yap;

		public AbstractFileBasedDb4oFixture(IConfigurationSource configSource, string fileName
			) : base(configSource)
		{
			_yap = new Sharpen.IO.File(fileName);
		}

		public virtual string GetAbsolutePath()
		{
			return _yap.GetAbsolutePath();
		}

		protected override void DoClean()
		{
			_yap.Delete();
		}
	}
}
