/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Defragment;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class TranslatedDefragTestCase : ITestLifeCycle
	{
		private static readonly string TranslatedName = "A";

		public class Translated
		{
			public string _name;

			public Translated(string name)
			{
				_name = name;
			}
		}

		public class TranslatedTranslator : IObjectConstructor
		{
			public virtual object OnInstantiate(IObjectContainer container, object storedObject
				)
			{
				return new TranslatedDefragTestCase.Translated((string)storedObject);
			}

			public virtual void OnActivate(IObjectContainer container, object applicationObject
				, object storedObject)
			{
			}

			public virtual object OnStore(IObjectContainer container, object applicationObject
				)
			{
				return ((TranslatedDefragTestCase.Translated)applicationObject)._name;
			}

			public virtual Type StoredClass()
			{
				return typeof(string);
			}
		}

		private static readonly string Filename = Path.GetTempFileName();

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestDefragWithTranslator()
		{
			AssertDefragment(true);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void TestDefragWithoutTranslator()
		{
			AssertDefragment(true);
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void AssertDefragment(bool registerTranslator)
		{
			Store();
			Defragment(registerTranslator);
			AssertTranslated();
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Defragment(bool registerTranslator)
		{
			DefragmentConfig defragConfig = new DefragmentConfig(Filename);
			defragConfig.Db4oConfig(Config(registerTranslator));
			defragConfig.ForceBackupDelete(true);
			Db4objects.Db4o.Defragment.Defragment.Defrag(defragConfig);
		}

		private void Store()
		{
			IObjectContainer db = OpenDatabase();
			db.Store(new TranslatedDefragTestCase.Translated(TranslatedName));
			db.Close();
		}

		private void AssertTranslated()
		{
			IObjectContainer db = OpenDatabase();
			IObjectSet result = db.Query(typeof(TranslatedDefragTestCase.Translated));
			Assert.AreEqual(1, result.Count);
			TranslatedDefragTestCase.Translated trans = (TranslatedDefragTestCase.Translated)
				result.Next();
			Assert.AreEqual(TranslatedName, trans._name);
			db.Close();
		}

		private IObjectContainer OpenDatabase()
		{
			return Db4oFactory.OpenFile(Config(true), Filename);
		}

		private IConfiguration Config(bool registerTranslator)
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			config.ReflectWith(Platform4.ReflectorForType(typeof(TranslatedDefragTestCase.Translated
				)));
			if (registerTranslator)
			{
				config.ObjectClass(typeof(TranslatedDefragTestCase.Translated)).Translate(new TranslatedDefragTestCase.TranslatedTranslator
					());
			}
			return config;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetUp()
		{
			DeleteDatabaseFile();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TearDown()
		{
			DeleteDatabaseFile();
		}

		private void DeleteDatabaseFile()
		{
			new Sharpen.IO.File(Filename).Delete();
		}
	}
}
