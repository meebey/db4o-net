/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ClassMetadataTestCase : AbstractDb4oTestCase
	{
		public class SuperClazz
		{
			public int _id;

			public string _name;
		}

		public class SubClazz : ClassMetadataTestCase.SuperClazz
		{
			public int _age;
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Store()
		{
			Store(new ClassMetadataTestCase.SubClazz());
		}

		public virtual void TestForEachField()
		{
			Collection4 expectedNames = new Collection4(new ArrayIterator4(new string[] { "_id"
				, "_name", "_age" }));
			ClassMetadata classMetadata = ClassMetadataFor(typeof(ClassMetadataTestCase.SubClazz
				));
			classMetadata.ForEachField(new _IProcedure4_29(expectedNames));
			Assert.IsTrue(expectedNames.IsEmpty());
		}

		private sealed class _IProcedure4_29 : IProcedure4
		{
			public _IProcedure4_29(Collection4 expectedNames)
			{
				this.expectedNames = expectedNames;
			}

			public void Apply(object arg)
			{
				FieldMetadata curField = (FieldMetadata)arg;
				Assert.IsNotNull(expectedNames.Remove(curField.GetName()));
			}

			private readonly Collection4 expectedNames;
		}
	}
}
