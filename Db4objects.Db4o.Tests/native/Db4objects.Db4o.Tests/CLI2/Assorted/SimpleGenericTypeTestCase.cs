using Db4objects.Db4o.Query;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.Assorted
{
#if NET_2_0 || CF_2_0
	class SimpleGenericType<T>
	{
		public T value;

		public SimpleGenericType(T value)
		{
			this.value = value;
		}
	}
#endif

	class SimpleGenericTypeTestCase : AbstractDb4oTestCase
	{
#if NET_2_0 || CF_2_0
		override protected void Store()
		{
			Store(new SimpleGenericType<string>("Will it work?"));
			Store(new SimpleGenericType<int>(42));
		}

		public void Test()
		{
			TstGenericType("Will it work?");
			TstGenericType(42);
		}

		private void TstGenericType<T>(T expectedValue)
		{
			IQuery query = NewQuery(typeof(SimpleGenericType<T>));

			EnsureGenericItem<T>(expectedValue, query.Execute());

			query = NewQuery(typeof(SimpleGenericType<T>));
			query.Descend("value").Constrain(expectedValue);
			EnsureGenericItem<T>(expectedValue, query.Execute());
		}

		private static void EnsureGenericItem<T>(T expectedValue, IObjectSet os)
		{
			Assert.AreEqual(1, os.Size());

			SimpleGenericType<T> item = (SimpleGenericType<T>)os.Next();
			Assert.AreEqual(expectedValue, item.value);
		}
#endif
	}
}
