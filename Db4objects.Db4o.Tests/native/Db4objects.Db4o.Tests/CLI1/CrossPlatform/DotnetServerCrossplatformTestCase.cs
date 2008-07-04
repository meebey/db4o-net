using System.IO;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Tests.CLI1.CrossPlatform 
{
	class DotnetServerCrossplatformTestCase : CrossplatformTestCaseBase, IMessageRecipient
	{
#if !CF
		public void Test()
		{
			foreach (Person p in persons)
			{
				InsertFromJavaClient(p.Year, p.Name, p.LocalReleaseDate);
			}

			AssertQueryFromJavaClient();
		}

		protected override string GetClientAliases()
		{
//Db4objects.Db4o.Tests.CLI1.CrossPlatform.UnoptimizideJoeFinder, Db4objects.Db4o.Tests
//Db4objects.Db4o.Tests.CLI1.CrossPlatform.StopServer, Db4objects.Db4o.Tests
//Db4objects.Db4o.Tests.CLI1.CrossPlatform.PersonEvaluator, Db4objects.Db4o.Tests
			return @"
	config.add(new com.db4o.config.DotnetSupport(true));
	config.addAlias(new com.db4o.config.TypeAlias(""Db4objects.Db4o.Tests.CLI1.CrossPlatform.Person, Db4objects.Db4o.Tests"", Person.class.getName()));
	config.addAlias(new com.db4o.config.TypeAlias(""Db4objects.Db4o.Tests.CLI1.CrossPlatform.Movies, Db4objects.Db4o.Tests"", Movies.class.getName()));";
		}

		protected override void StartServer()
		{
			string databasePath = InitDatabaseFile();
			_server = Db4oFactory.OpenServer(databasePath, HOST_PORT);
			_server.GrantAccess(USER_NAME, USER_PWD);

			_server.Ext().Configure().ClientServer().SetMessageRecipient(this);
		}

		private static string InitDatabaseFile()
		{
			string databaseFile = Path.Combine(Path.GetTempPath(), "CrossplatformDotnetServer.odb");
			if (File.Exists(databaseFile))
			{
				File.Delete(databaseFile);
			}

			return databaseFile;
		}

		protected override IConfiguration Config()
		{
			return Db4oFactory.NewConfiguration();
		}

#endif

		public void ProcessMessage(IMessageContext context, object message)
		{
			_server.Close();
		}

		private IObjectServer _server;
	}
}
