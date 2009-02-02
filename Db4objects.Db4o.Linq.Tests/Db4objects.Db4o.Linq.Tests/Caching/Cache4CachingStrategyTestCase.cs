
using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Caching;
using Db4objects.Db4o.Linq.Caching;
using Db4oUnit;
using Db4oUnit.Mocking;

namespace Db4objects.Db4o.Linq.Tests.Caching
{
	class Cache4CachingStrategyTestCase : ITestCase
	{
		class Cache4Mock : MethodCallRecorder, ICache4
		{
			public IEnumerator GetEnumerator()
			{
				throw new System.NotImplementedException();
			}

			public object Produce(object key, IFunction4 producer, IProcedure4 onDiscard)
			{
				Record(new MethodCall("Produce", new object[] { key }));
				return producer.Apply(key);
			}
		}

		public void TestProduce()
		{
			var cache4Mock = new Cache4Mock();
			var subject = Cache4CachingStrategy<string, int>.NewInstance(cache4Mock);

			cache4Mock.Verify(new MethodCall[0]);
			Assert.AreEqual(42, subject.Produce("42", key => 42));
			cache4Mock.Verify(new MethodCall[]
			                  {
			                  	new MethodCall("Produce", new object[] { "42" }),
			                  });

			cache4Mock.Reset();
			Assert.AreEqual(-1, subject.Produce("42", key => -1));
			cache4Mock.Verify(new MethodCall[]
			                  {
			                  	new MethodCall("Produce", new object[] { "42" }),
			                  });

		}

		public void TestProduceWithEqualityComparer()
		{
			var cache4 = CacheFactory.NewLRUCache(2);
			var subject = Cache4CachingStrategy<string, int>.NewInstance(cache4, StringComparer.CurrentCultureIgnoreCase);

			Assert.AreEqual(42, subject.Produce("foo", key=>42));
			Assert.AreEqual(42, subject.Produce("FOO", key=>-1));

			Iterator4Assert.AreEqual(new object[] { 42 }, cache4);

		}
	}
}
