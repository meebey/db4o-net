using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oInMemory : AbstractSoloDb4oFixture
	{
		public Db4oInMemory() : base(new IndependentConfigurationSource())
		{
		}

		public Db4oInMemory(IConfigurationSource configSource) : base(configSource)
		{
		}

		private MemoryFile _memoryFile;

		protected override IObjectContainer CreateDatabase(IConfiguration config)
		{
			if (null == _memoryFile)
			{
				_memoryFile = new MemoryFile();
			}
			return ExtDb4oFactory.OpenMemoryFile(config, _memoryFile);
		}

		protected override void DoClean()
		{
			_memoryFile = null;
		}

		public override string GetLabel()
		{
			return "IN-MEMORY";
		}

		public override void Defragment()
		{
		}
	}
}
