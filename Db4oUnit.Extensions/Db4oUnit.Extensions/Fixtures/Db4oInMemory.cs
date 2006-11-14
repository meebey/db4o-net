namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oInMemory : Db4oUnit.Extensions.Fixtures.AbstractSoloDb4oFixture
	{
		public Db4oInMemory() : base(new Db4oUnit.Extensions.Fixtures.IndependentConfigurationSource
			())
		{
		}

		public Db4oInMemory(Db4oUnit.Extensions.Fixtures.IConfigurationSource configSource
			) : base(configSource)
		{
		}

		private Db4objects.Db4o.Ext.MemoryFile _memoryFile;

		protected override Db4objects.Db4o.IObjectContainer CreateDatabase(Db4objects.Db4o.Config.IConfiguration
			 config)
		{
			if (null == _memoryFile)
			{
				_memoryFile = new Db4objects.Db4o.Ext.MemoryFile();
			}
			return Db4objects.Db4o.Ext.ExtDb4oFactory.OpenMemoryFile(config, _memoryFile);
		}

		protected override void DoClean()
		{
			_memoryFile = null;
		}

		public override string GetLabel()
		{
			return "IN-MEMORY";
		}
	}
}
