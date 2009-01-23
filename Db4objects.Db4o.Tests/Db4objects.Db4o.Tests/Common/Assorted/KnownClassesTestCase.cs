/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class KnownClassesTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new KnownClassesTestCase().RunAll();
		}

		public static readonly Type[] InternalClasses = new Type[] { typeof(Db4oDatabase)
			, typeof(PBootRecord), typeof(StaticClass), typeof(MetaClass), typeof(MetaField)
			 };

		public class Item
		{
		}

		protected override void Store()
		{
			Assert.IsFalse(IsKnownClass(typeof(KnownClassesTestCase.Item)));
			Store(new KnownClassesTestCase.Item());
			Assert.IsTrue(IsKnownClass(typeof(KnownClassesTestCase.Item)));
		}

		public virtual void TestNoPrimitives()
		{
			IReflectClass[] knownClassArray = Container().KnownClasses();
			for (int knownClassIndex = 0; knownClassIndex < knownClassArray.Length; ++knownClassIndex)
			{
				IReflectClass knownClass = knownClassArray[knownClassIndex];
				Assert.IsFalse(knownClass.IsPrimitive());
			}
		}

		public virtual void TestNoSecondClass()
		{
			IReflectClass[] knownClassArray = Container().KnownClasses();
			for (int knownClassIndex = 0; knownClassIndex < knownClassArray.Length; ++knownClassIndex)
			{
				IReflectClass knownClass = knownClassArray[knownClassIndex];
				Assert.IsFalse(IsSecondClass(knownClass));
			}
		}

		private bool IsSecondClass(IReflectClass knownClass)
		{
			if (Container().Handlers().IsTransient(knownClass))
			{
				return false;
			}
			ClassMetadata classMetadata = Container().ProduceClassMetadata(knownClass);
			return classMetadata.IsSecondClass();
		}

		public virtual void TestInternalClassesAreNotVisible()
		{
			IReflectClass[] knownClassArray = Container().KnownClasses();
			for (int knownClassIndex = 0; knownClassIndex < knownClassArray.Length; ++knownClassIndex)
			{
				IReflectClass knownClass = knownClassArray[knownClassIndex];
				AssertIsNotInternal(knownClass.GetName());
			}
		}

		public virtual void TestNewClassIsFound()
		{
			Assert.IsTrue(IsKnownClass(typeof(KnownClassesTestCase.Item)));
		}

		private bool IsKnownClass(Type klass)
		{
			return IsKnownClass(ReflectPlatform.FullyQualifiedName(klass));
		}

		private bool IsKnownClass(string expected)
		{
			IReflectClass[] knownClassArray = Container().KnownClasses();
			for (int knownClassIndex = 0; knownClassIndex < knownClassArray.Length; ++knownClassIndex)
			{
				IReflectClass knownClass = knownClassArray[knownClassIndex];
				string className = knownClass.GetName();
				if (className.Equals(expected))
				{
					return true;
				}
			}
			return false;
		}

		private void AssertIsNotInternal(string className)
		{
			for (int j = 0; j < InternalClasses.Length; j++)
			{
				Assert.AreNotEqual(InternalClasses[j].FullName, className);
			}
		}
	}
}
