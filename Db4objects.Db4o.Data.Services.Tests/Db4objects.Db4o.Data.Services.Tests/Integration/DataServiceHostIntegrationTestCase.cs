using System;
using System.Data.Services;
using System.Data.Services.Client;
using System.Linq;
using Db4objects.Db4o.Linq;
using Db4oUnit;
using Moq;

namespace Db4objects.Db4o.Data.Services.Tests.Integration
{
	class DataServiceHostIntegrationTestCase : ITestLifeCycle
	{
		private static readonly Uri ServiceUri = new Uri("http://127.0.0.1:666/integration");

		public void TestAddObjectSaveChanges()
		{
			var contact = new Contact { Email = "a@b.c", Name = "abc" };
			
			var sessionMock = new Mock<IObjectContainer>(MockBehavior.Strict);
			sessionMock.Setup(session => session.Store(It.Is<Contact>(actual => actual.Equals(contact))))
				.AtMostOnce();
			sessionMock.Setup(session => session.Commit())
				.AtMostOnce();

			IntegrationDataContext.Session = sessionMock.Object;

			var context = new DataServiceContext(ServiceUri);
			context.AddObject("Contacts", contact);
			context.SaveChanges();

			sessionMock.Verify();
		}

		public void SetUp()
		{
			_dataHost = new DataServiceHost(typeof(IntegrationDataService), new Uri[] { ServiceUri });
			_dataHost.Open();
		}

		public void TearDown()
		{
			_dataHost.Close();
		}

		private DataServiceHost _dataHost;
	}

	[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public class IntegrationDataService : DataService<IntegrationDataContext>
	{
		public static void InitializeService(IDataServiceConfiguration config)
		{
			config.SetEntitySetAccessRule("*", EntitySetRights.All);
			config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
			config.UseVerboseErrors = true;
		}
	}

	[System.Data.Services.Common.DataServiceKey("Email")]
	public class Contact
	{
		public string Email { get; set; }

		public string Name { get; set; }

		public override bool Equals(object obj)
		{
			Contact other = obj as Contact;
			if (null == other)
				return false;
			return Email == other.Email
				 && Name == other.Name;
		}

		public override int GetHashCode()
		{
			return Email.GetHashCode();
		}
	}

	public class IntegrationDataContext : Db4oDataContext
	{
		public static IObjectContainer Session;

		protected override IObjectContainer OpenSession()
		{
			return Session;
		}

		public IQueryable<Contact> Contacts
		{
			get { return Container.Cast<Contact>().AsQueryable(); }
		}
	}

	
}
