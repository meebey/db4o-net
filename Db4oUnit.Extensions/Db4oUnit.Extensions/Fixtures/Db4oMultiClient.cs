using System.IO;
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oMultiClient : AbstractClientServerDb4oFixture
	{
		public Db4oMultiClient(IConfigurationSource configSource) : base(configSource)
		{
		}

		public Db4oMultiClient() : this(new IndependentConfigurationSource())
		{
		}

		public override IExtObjectContainer Db()
		{
			try
			{
				return Db4oFactory.OpenClient(Config(), HOST, PORT, USERNAME, PASSWORD).Ext();
			}
			catch (IOException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
				throw new TestException(e);
			}
		}

		public override string GetLabel()
		{
			return "C/S MULTI-CLIENT";
		}
	}
}
