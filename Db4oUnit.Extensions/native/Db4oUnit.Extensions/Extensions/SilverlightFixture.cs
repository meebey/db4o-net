/* Copyright (C) 2009 Versant Inc.  http://www.db4o.com */
#if SILVERLIGHT

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.IO;
using Db4oUnit.Extensions.Fixtures;

namespace Db4oUnit.Extensions.Extensions
{
	public class SilverlightFixture : AbstractSoloDb4oFixture
	{
		protected override void DoClean()
		{
			if (null != _storage)
			{
				_storage.Delete(DatabaseFileName);
			}
		}

		public override string Label()
		{
			return BuildLabel("Silverlight Solo");
		}

		public override void Defragment()
		{
			Defragment(DatabaseFileName);
		}

		protected override IObjectContainer CreateDatabase(IConfiguration config)
		{
			return Db4oFactory.OpenFile(config, DatabaseFileName);
		}

		protected override IConfiguration NewConfiguration()
		{
			IConfiguration config = base.NewConfiguration();
			config.Storage = _storage;

			return config;
		}

		private const string DatabaseFileName = "SilverlightDatabase.db4o";
		private readonly IsolatedStorageStorage _storage = new IsolatedStorageStorage();
	}
}

#endif