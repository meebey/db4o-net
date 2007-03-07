namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oSingleClient : Db4oUnit.Extensions.Fixtures.AbstractClientServerDb4oFixture
	{
		private Db4objects.Db4o.Ext.IExtObjectContainer _objectContainer;

		public Db4oSingleClient(Db4oUnit.Extensions.Fixtures.IConfigurationSource config, 
			string fileName, int port) : base(config, fileName, port)
		{
		}

		public Db4oSingleClient(Db4oUnit.Extensions.Fixtures.IConfigurationSource config, 
			int port) : base(config, FILE, port)
		{
		}

		public Db4oSingleClient(Db4oUnit.Extensions.Fixtures.IConfigurationSource config)
			 : base(config)
		{
		}

		public Db4oSingleClient() : this(new Db4oUnit.Extensions.Fixtures.IndependentConfigurationSource
			())
		{
		}

		public override void Close()
		{
			_objectContainer.Close();
			base.Close();
		}

		public override void Open()
		{
			base.Open();
			try
			{
				_objectContainer = _port == 0 ? OpenEmbeddedClient().Ext() : Db4objects.Db4o.Db4oFactory
					.OpenClient(Config(), HOST, _port, USERNAME, PASSWORD).Ext();
			}
			catch (System.IO.IOException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
				throw new Db4oUnit.TestException(e);
			}
		}

		public override Db4objects.Db4o.Ext.IExtObjectContainer Db()
		{
			return _objectContainer;
		}

		public override string GetLabel()
		{
			return "C/S SINGLE-CLIENT";
		}
	}
}
