using System.IO;
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oSingleClient : AbstractClientServerDb4oFixture
	{
		private IExtObjectContainer _objectContainer;

		public Db4oSingleClient(IConfigurationSource config, string fileName, int port) : 
			base(config, fileName, port)
		{
		}

		public Db4oSingleClient(IConfigurationSource config, int port) : base(config, FILE
			, port)
		{
		}

		public Db4oSingleClient(IConfigurationSource config) : base(config)
		{
		}

		public Db4oSingleClient() : this(new IndependentConfigurationSource())
		{
		}

		public override void Close()
		{
			if (null != _objectContainer)
			{
				_objectContainer.Close();
				_objectContainer = null;
			}
			base.Close();
		}

		public override void Open()
		{
			base.Open();
			try
			{
				_objectContainer = _port == 0 ? OpenEmbeddedClient().Ext() : Db4oFactory.OpenClient
					(Config(), HOST, _port, USERNAME, PASSWORD).Ext();
			}
			catch (IOException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
				throw new TestException(e);
			}
		}

		public override IExtObjectContainer Db()
		{
			return _objectContainer;
		}

		public override string GetLabel()
		{
			return "C/S SINGLE-CLIENT";
		}
	}
}
