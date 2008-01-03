/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class HandlerRegistryTestCase : AbstractDb4oTestCase
	{
		public interface IFooInterface
		{
		}

		public virtual void _testInterfaceHandlerIsSameAsObjectHandler()
		{
			Assert.AreSame(HandlerForClass(typeof(object)), HandlerForClass(typeof(HandlerRegistryTestCase.IFooInterface
				)));
		}

		private ITypeHandler4 HandlerForClass(Type clazz)
		{
			return Handlers().HandlerForClass(Stream(), ReflectClass(clazz));
		}

		private HandlerRegistry Handlers()
		{
			return Stream().Handlers();
		}

		public virtual void TestClassForID()
		{
			IReflectClass byReflector = Reflector().ForClass(typeof(int));
			IReflectClass byID = Handlers().ClassForID(Handlers4.IntId);
			Assert.IsNotNull(byID);
			Assert.AreEqual(byReflector, byID);
		}

		public virtual void TestClassReflectorForHandler()
		{
			IReflectClass byReflector = Reflector().ForClass(typeof(int));
			IReflectClass byID = Handlers().ClassForID(Handlers4.IntId);
			Assert.IsNotNull(byID);
			Assert.AreEqual(byReflector, byID);
		}

		public static void Main(string[] arguments)
		{
			new HandlerRegistryTestCase().RunSolo();
		}
	}
}
