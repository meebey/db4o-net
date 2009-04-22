/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Refactor;

namespace Db4objects.Db4o.Tests.Common.Refactor
{
	public class AddIndexedValueTypeFieldTestCase : AbstractDb4oTestCase
	{
		public class Version1
		{
			public Version1(string id)
			{
				this.id = id;
			}

			public string id;
		}

		public class Version2
		{
			public Version2(string id, DateTime creationDate)
			{
				this.id = id;
				this.creationDate = creationDate;
			}

			public string id;

			public DateTime creationDate;
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Store()
		{
			Store(new AddIndexedValueTypeFieldTestCase.Version1("version1"));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test()
		{
			IConfiguration config = Fixture().Config();
			config.AddAlias(new TypeAlias(FullyQualifiedName(typeof(AddIndexedValueTypeFieldTestCase.Version1
				)), FullyQualifiedName(typeof(AddIndexedValueTypeFieldTestCase.Version2))));
			config.ObjectClass(typeof(AddIndexedValueTypeFieldTestCase.Version2)).ObjectField
				("creationDate").Indexed(true);
			Reopen();
			Store(new AddIndexedValueTypeFieldTestCase.Version2("version2", new DateTime()));
			Assert.AreEqual(2, NewQuery(typeof(AddIndexedValueTypeFieldTestCase.Version2)).Execute
				().Count);
		}

		private string FullyQualifiedName(Type clazz)
		{
			return ReflectPlatform.FullyQualifiedName(clazz);
		}
	}
}
