/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fieldhandlers;
using Db4objects.Db4o.Internal.Handlers;
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
			return (ITypeHandler4)Stream().FieldHandlerForClass(ReflectClass(clazz));
		}

		private HandlerRegistry Handlers()
		{
			return Stream().Handlers();
		}

		public virtual void TestTypeHandlerForID()
		{
			AssertTypeHandlerClass(Handlers4.IntId, typeof(IntHandler));
			AssertTypeHandlerClass(Handlers4.UntypedId, typeof(PlainObjectHandler));
		}

		private void AssertTypeHandlerClass(int id, Type clazz)
		{
			ITypeHandler4 typeHandler = Handlers().TypeHandlerForID(id);
			Assert.IsInstanceOf(clazz, typeHandler);
		}

		public virtual void TestTypeHandlerID()
		{
			AssertTypeHandlerID(Handlers4.IntId, IntegerClassReflector());
			AssertTypeHandlerID(Handlers4.UntypedId, ObjectClassReflector());
		}

		private void AssertTypeHandlerID(int handlerID, IReflectClass integerClassReflector
			)
		{
			ITypeHandler4 typeHandler = Handlers().TypeHandlerForClass(integerClassReflector);
			int id = Handlers().TypeHandlerID(typeHandler);
			Assert.AreEqual(handlerID, id);
		}

		public virtual void TestTypeHandlerForClass()
		{
			Assert.IsInstanceOf(typeof(IntHandler), Handlers().TypeHandlerForClass(IntegerClassReflector
				()));
			Assert.IsInstanceOf(typeof(PlainObjectHandler), Handlers().TypeHandlerForClass(ObjectClassReflector
				()));
		}

		public virtual void TestFieldHandlerForID()
		{
			AssertFieldHandler(Handlers4.IntId, typeof(IntHandler));
			AssertFieldHandler(Handlers4.AnyArrayId, typeof(UntypedArrayFieldHandler));
			AssertFieldHandler(Handlers4.AnyArrayNId, typeof(UntypedMultidimensionalArrayFieldHandler
				));
		}

		private void AssertFieldHandler(int handlerID, Type fieldHandlerClass)
		{
			IFieldHandler fieldHandler = Handlers().FieldHandlerForId(handlerID);
			Assert.IsInstanceOf(fieldHandlerClass, fieldHandler);
		}

		public virtual void TestClassForID()
		{
			IReflectClass byReflector = IntegerClassReflector();
			IReflectClass byID = Handlers().ClassForID(Handlers4.IntId);
			Assert.IsNotNull(byID);
			Assert.AreEqual(byReflector, byID);
		}

		public virtual void TestClassReflectorForHandler()
		{
			IReflectClass byReflector = IntegerClassReflector();
			IReflectClass byID = Handlers().ClassForID(Handlers4.IntId);
			Assert.IsNotNull(byID);
			Assert.AreEqual(byReflector, byID);
		}

		private IReflectClass ObjectClassReflector()
		{
			return ReflectorFor(typeof(object));
		}

		private IReflectClass IntegerClassReflector()
		{
			return ReflectorFor(typeof(int));
		}

		private IReflectClass ReflectorFor(Type clazz)
		{
			return Reflector().ForClass(clazz);
		}

		public static void Main(string[] arguments)
		{
			new HandlerRegistryTestCase().RunSolo();
		}
	}
}
