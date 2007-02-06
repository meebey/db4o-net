namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oMultiClient : Db4oUnit.Extensions.Fixtures.AbstractClientServerDb4oFixture
	{
		public Db4oMultiClient(Db4oUnit.Extensions.Fixtures.IConfigurationSource configSource
			) : base(configSource)
		{
		}

		public Db4oMultiClient() : this(new Db4oUnit.Extensions.Fixtures.IndependentConfigurationSource
			())
		{
		}

		public override Db4objects.Db4o.Ext.IExtObjectContainer Db()
		{
			try
			{
				return Db4objects.Db4o.Db4oFactory.OpenClient(Config(), HOST, PORT, USERNAME, PASSWORD
					).Ext();
			}
			catch (System.IO.IOException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
				throw new Db4oUnit.TestException(e);
			}
		}

		public override string GetLabel()
		{
			return "C/S MULTI-CLIENT";
		}
	}
}
