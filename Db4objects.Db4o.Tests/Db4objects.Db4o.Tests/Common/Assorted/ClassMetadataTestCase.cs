/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
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

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new ClassMetadataTestCase.SubClazz());
		}

		public virtual void TestFieldIterator()
		{
			Collection4 expectedNames = new Collection4(new ArrayIterator4(new string[] { "_id"
				, "_name", "_age" }));
			IEnumerator fieldIter = ClassMetadataFor(typeof(ClassMetadataTestCase.SubClazz)).
				Fields();
			while (fieldIter.MoveNext())
			{
				FieldMetadata curField = (FieldMetadata)fieldIter.Current;
				Assert.IsNotNull(expectedNames.Remove(curField.GetName()));
			}
			Assert.IsTrue(expectedNames.IsEmpty());
		}
	}
}
