namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oSingleClient : Db4oUnit.Extensions.Fixtures.AbstractClientServerDb4oFixture
	{
		private Db4objects.Db4o.Ext.IExtObjectContainer _objectContainer;

		public Db4oSingleClient(Db4oUnit.Extensions.Fixtures.IConfigurationSource config, 
			string fileName, int port) : base(config, fileName, port)
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
				_objectContainer = Db4objects.Db4o.Db4o.OpenClient(Config(), HOST, PORT, USERNAME
					, PASSWORD).Ext();
			}
			catch (System.IO.IOException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
				throw new Db4oUnit.TestException(e);
			}
		}

		public override bool Accept(System.Type clazz)
		{
			if ((typeof(Db4oUnit.Extensions.Fixtures.IOptOutCS).IsAssignableFrom(clazz)))
			{
				return false;
			}
			return true;
		}

		public override Db4objects.Db4o.Ext.IExtObjectContainer Db()
		{
			return _objectContainer;
		}
	}
}
