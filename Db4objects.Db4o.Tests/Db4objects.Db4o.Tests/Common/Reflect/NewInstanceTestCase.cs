/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Core;
using Db4objects.Db4o.Tests.Common.Reflect;

namespace Db4objects.Db4o.Tests.Common.Reflect
{
	public class NewInstanceTestCase : ITestCase, ITestLifeCycle
	{
		private IReflector _reflector;

		private class ItemThrowingConstructors
		{
			public ItemThrowingConstructors()
			{
				throw new Exception();
			}

			public ItemThrowingConstructors(int value)
			{
				throw new Exception();
			}
		}

		public class ItemNoDefaultConstructor
		{
			public ItemNoDefaultConstructor(int value)
			{
			}
		}

		public class ItemParent
		{
			public NewInstanceTestCase.ItemChild _child;

			public ItemParent(NewInstanceTestCase.ItemChild child)
			{
				_child = child;
			}
		}

		public class ItemChild
		{
			public string _name;

			public ItemChild(string name)
			{
				_name = name;
			}
		}

		public class MockReflectorConfiguration : IReflectorConfiguration
		{
			private IList _classNames;

			private bool _testConstructor;

			public MockReflectorConfiguration(string[] classNames) : this(classNames, true)
			{
			}

			public MockReflectorConfiguration(string[] classNames, bool testConstructor)
			{
				_classNames = Sharpen.Util.Arrays.AsList(classNames);
				_testConstructor = testConstructor;
			}

			public virtual bool CallConstructor(IReflectClass clazz)
			{
				return _classNames.Contains(clazz.GetName());
			}

			public virtual bool TestConstructors()
			{
				return _testConstructor;
			}
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestComplexItem()
		{
			IReflectClass parentClazz = _reflector.ForObject(new NewInstanceTestCase.ItemParent
				(null));
			IReflectField[] fields = parentClazz.GetDeclaredFields();
			Assert.AreEqual(1, fields.Length);
			IReflectClass fieldClazz = fields[0].GetFieldType();
			IReflectClass childClazz = _reflector.ForClass(typeof(NewInstanceTestCase.ItemChild
				));
			Assert.AreEqual(childClazz.GetName(), fieldClazz.GetName());
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestNotStorable()
		{
			AssertCannotBeInstantiated(typeof(IList));
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestPlatformDependentInstantiation()
		{
			IConstructorAwareReflectClass reflectClass = (IConstructorAwareReflectClass)_reflector
				.ForClass(typeof(NewInstanceTestCase.ItemThrowingConstructors));
			if (reflectClass.GetSerializableConstructor() != null)
			{
				Assert.IsTrue(reflectClass.EnsureCanBeInstantiated());
				Assert.IsNotNull(reflectClass.NewInstance());
			}
			else
			{
				Assert.IsFalse(reflectClass.EnsureCanBeInstantiated());
				Assert.IsNull(reflectClass.NewInstance());
			}
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestNoDefaultConstructor()
		{
			Assert.IsNotNull(CreateInstanceOf(typeof(NewInstanceTestCase.ItemNoDefaultConstructor
				)));
		}

		public virtual void AssertCannotBeInstantiated(Type clazz)
		{
			IReflectClass reflectClass = _reflector.ForClass(clazz);
			Assert.IsFalse(reflectClass.EnsureCanBeInstantiated());
			Assert.IsNull(reflectClass.NewInstance());
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestHashTable()
		{
			Hashtable hashTable = (Hashtable)CreateInstanceOf(typeof(Hashtable));
			AssertIsUsable(hashTable);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestHashMap()
		{
			Hashtable hashMap = (Hashtable)CreateInstanceOf(typeof(Hashtable));
			AssertIsUsable(hashMap);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestList()
		{
			IList list = (IList)CreateInstanceOf(typeof(ArrayList));
			AssertIsUsable(list);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestFloat()
		{
			float f = (float)CreateInstanceOf(typeof(float));
			AssertIsUsable(f);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestString()
		{
			string s = (string)CreateInstanceOf(typeof(string));
			AssertIsUsable(s);
		}

		private void AssertIsUsable(float f)
		{
			Assert.AreEqual(0.0, f);
		}

		private void AssertIsUsable(string s)
		{
			Assert.AreEqual(0, s.Length);
		}

		private void AssertIsUsable(ICollection collection)
		{
			Assert.AreEqual(0, collection.Count);
		}

		private void AssertIsUsable(IDictionary map)
		{
			map.Add(1, "one");
			Assert.AreEqual(1, map.Count);
			Assert.AreEqual("one", map[1]);
			map.Remove(1);
			Assert.AreEqual(0, map.Count);
		}

		private object CreateInstanceOf(Type clazz)
		{
			return _reflector.ForClass(clazz).NewInstance();
		}

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			_reflector = Platform4.ReflectorForType(this.GetType());
			string[] clazzs = new string[] { _reflector.ForClass(typeof(string)).GetName(), _reflector
				.ForClass(typeof(Hashtable)).GetName(), _reflector.ForClass(typeof(Hashtable)).GetName
				(), _reflector.ForClass(typeof(ArrayList)).GetName() };
			NewInstanceTestCase.MockReflectorConfiguration config = new NewInstanceTestCase.MockReflectorConfiguration
				(clazzs, true);
			_reflector.Configuration(config);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
		}
	}
}
