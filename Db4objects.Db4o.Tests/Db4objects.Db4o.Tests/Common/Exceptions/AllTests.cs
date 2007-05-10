using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Exceptions.AllTests().RunAll();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(ActivationExceptionBubblesUpTestCase), typeof(BackupCSExceptionTestCase)
				, typeof(BackupDb4oIOExceptionTestCase), typeof(DatabaseClosedExceptionTestCase)
				, typeof(DatabaseReadonlyExceptionTestCase), typeof(GlobalOnlyConfigExceptionTestCase)
				, typeof(IncompatibleFileFormatExceptionTestCase), typeof(InvalidIDExceptionTestCase)
				, typeof(InvalidPasswordTestCase), typeof(ObjectCanActiviateExceptionTestCase), 
				typeof(ObjectCanDeleteExceptionTestCase), typeof(ObjectOnDeleteExceptionTestCase)
				, typeof(ObjectCanNewExceptionTestCase), typeof(StoreExceptionBubblesUpTestCase)
				, typeof(StoredClassExceptionBubblesUpTestCase), typeof(TSerializableOnInstantiateCNFExceptionTestCase)
				, typeof(TSerializableOnInstantiateIOExceptionTestCase), typeof(TSerializableOnStoreExceptionTestCase)
				 };
		}
	}
}
