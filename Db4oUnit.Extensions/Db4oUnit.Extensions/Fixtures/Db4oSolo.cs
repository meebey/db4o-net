using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;

namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oSolo : AbstractFileBasedDb4oFixture
	{
		private static readonly string FILE = "db4oSoloTest.yap";

		public Db4oSolo() : this(new IndependentConfigurationSource())
		{
		}

		public Db4oSolo(IConfigurationSource configSource) : base(configSource, FILE)
		{
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
			Defragment(FILE);
		}
	}
}
