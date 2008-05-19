/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Tests.Common.Handlers;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class SecondClassTestCase : AbstractDb4oTestCase
	{
		internal static Hashtable4 objectIsSecondClass;

		public class Item
		{
		}

		public class CustomSecondClassItem
		{
		}

		public class CustomFirstClassItem
		{
		}

		static SecondClassTestCase()
		{
			objectIsSecondClass = new Hashtable4();
			Register(1, true);
			Register(new DateTime(), true);
			Register("astring", true);
			Register(new SecondClassTestCase.Item(), false);
			Register(new int[] { 1 }, true);
			Register(new DateTime[] { new DateTime() }, true);
			Register(new SecondClassTestCase.Item[] { new SecondClassTestCase.Item() }, true);
			Register(new SecondClassTestCase.CustomFirstClassItem(), false);
			Register(new SecondClassTestCase.CustomSecondClassItem(), true);
		}

		private static void Register(object obj, bool isSecondClass)
		{
			objectIsSecondClass.Put(obj, isSecondClass);
		}

		public class FirstClassTypeHandler : FirstClassObjectHandler
		{
		}

		public class SecondClassTypeHandler : FirstClassObjectHandler, IEmbeddedTypeHandler
		{
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.RegisterTypeHandler(new SingleClassTypeHandlerPredicate(typeof(SecondClassTestCase.CustomFirstClassItem
				)), new SecondClassTestCase.FirstClassTypeHandler());
			config.RegisterTypeHandler(new SingleClassTypeHandlerPredicate(typeof(SecondClassTestCase.CustomSecondClassItem
				)), new SecondClassTestCase.SecondClassTypeHandler());
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new SecondClassTestCase.Item());
			Store(new SecondClassTestCase.CustomFirstClassItem());
			Store(new SecondClassTestCase.CustomSecondClassItem());
		}

		public virtual void Test()
		{
			IEnumerator i = objectIsSecondClass.Keys();
			while (i.MoveNext())
			{
				object currentObject = i.Current;
				bool isSecondClass = ((bool)objectIsSecondClass.Get(currentObject));
				ClassMetadata classMetadata = Container().ClassMetadataForObject(currentObject);
				Assert.AreEqual(isSecondClass, classMetadata.IsSecondClass());
			}
		}
	}
}
