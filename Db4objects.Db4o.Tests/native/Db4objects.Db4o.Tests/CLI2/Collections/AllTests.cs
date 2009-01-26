/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
namespace Db4objects.Db4o.Tests.CLI2.Collections
{
	public class AllTests : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
			return new System.Type[]
			{ 
                typeof(ArrayDictionary4TestCase),
                typeof(ArrayDictionary4TATestCase),
				typeof(ArrayDictionary4TransparentPersistenceTestCase),

				typeof(ArrayList4ActivatableTestCase), 
				typeof(ArrayList4TestCase), 
                typeof(ArrayList4TATestCase), 
                typeof(GenericDictionaryTestCase),
				typeof(GenericDictionaryTestSuite),

				typeof(GenericListTypeHandlerTestCase),

#if NET_3_5 && ! CF
                typeof(HashSetTestCase),
#endif 

            };
		}
	}
}
