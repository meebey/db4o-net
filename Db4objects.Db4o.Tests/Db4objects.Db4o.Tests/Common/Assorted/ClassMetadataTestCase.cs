namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ClassMetadataTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class SuperClazz
		{
			public int _id;

			public string _name;
		}

		public class SubClazz : Db4objects.Db4o.Tests.Common.Assorted.ClassMetadataTestCase.SuperClazz
		{
			public int _age;
		}

		protected override void Store()
		{
			Store(new Db4objects.Db4o.Tests.Common.Assorted.ClassMetadataTestCase.SubClazz());
		}

		public virtual void TestFieldIterator()
		{
			Db4objects.Db4o.Foundation.Collection4 expectedNames = new Db4objects.Db4o.Foundation.Collection4
				(new Db4objects.Db4o.Foundation.ArrayIterator4(new string[] { "_id", "_name", "_age"
				 }));
			Db4objects.Db4o.Internal.ClassMetadata clazz = Stream().GetYapClass(Reflector().ForClass
				(typeof(Db4objects.Db4o.Tests.Common.Assorted.ClassMetadataTestCase.SubClazz)));
			System.Collections.IEnumerator fieldIter = clazz.Fields();
			while (fieldIter.MoveNext())
			{
				Db4objects.Db4o.Internal.FieldMetadata curField = (Db4objects.Db4o.Internal.FieldMetadata
					)fieldIter.Current;
				Db4oUnit.Assert.IsNotNull(expectedNames.Remove(curField.GetName()));
			}
			Db4oUnit.Assert.IsTrue(expectedNames.IsEmpty());
		}
	}
}
