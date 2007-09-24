/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Sharpen;

namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oSolo : AbstractFileBasedDb4oFixture
	{
		private static readonly string FILE = "db4oSoloTest.yap";

		public Db4oSolo() : this(new IndependentConfigurationSource())
		{
		}

		public Db4oSolo(IConfigurationSource configSource) : base(configSource, FilePath(
			))
		{
		}

		private static string FilePath()
		{
			string path = Runtime.GetProperty("db4ounit.file.path");
			if (path == null || path.Length == 0)
			{
				path = ".";
			}
			return Path.Combine(path, FILE);
		}

		protected override IObjectContainer CreateDatabase(IConfiguration config)
		{
			return Db4oFactory.OpenFile(config, GetAbsolutePath());
		}

		public override string GetLabel()
		{
			return "SOLO";
		}

		public override void Defragment()
		{
			Defragment(FilePath());
		}
	}
}
