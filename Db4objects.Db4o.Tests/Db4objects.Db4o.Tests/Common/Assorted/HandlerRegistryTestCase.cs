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

		public class Item
		{
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new HandlerRegistryTestCase.Item());
		}

		public virtual void TestCorrectHandlerVersion()
		{
			UntypedFieldHandler untypedFieldHandler = new UntypedFieldHandler(Stream());
			AssertCorrectedHandlerVersion(typeof(UntypedFieldHandler0), untypedFieldHandler, 
				-1);
			AssertCorrectedHandlerVersion(typeof(UntypedFieldHandler0), untypedFieldHandler, 
				0);
			AssertCorrectedHandlerVersion(typeof(UntypedFieldHandler2), untypedFieldHandler, 
				1);
			AssertCorrectedHandlerVersion(typeof(UntypedFieldHandler2), untypedFieldHandler, 
				2);
			AssertCorrectedHandlerVersion(typeof(UntypedFieldHandler), untypedFieldHandler, HandlerRegistry
				.HandlerVersion);
			AssertCorrectedHandlerVersion(typeof(UntypedFieldHandler), untypedFieldHandler, HandlerRegistry
				.HandlerVersion + 1);
			FirstClassObjectHandler firstClassObjectHandler = new FirstClassObjectHandler(ItemClassMetadata
				());
			AssertCorrectedHandlerVersion(typeof(FirstClassObjectHandler0), firstClassObjectHandler
				, 0);
			AssertCorrectedHandlerVersion(typeof(FirstClassObjectHandler), firstClassObjectHandler
				, 2);
			PrimitiveFieldHandler primitiveFieldHandler = new PrimitiveFieldHandler(null, untypedFieldHandler
				, 0, null);
			AssertPrimitiveFieldHandlerDelegate(typeof(UntypedFieldHandler0), primitiveFieldHandler
				, 0);
			AssertPrimitiveFieldHandlerDelegate(typeof(UntypedFieldHandler2), primitiveFieldHandler
				, 1);
			AssertPrimitiveFieldHandlerDelegate(typeof(UntypedFieldHandler2), primitiveFieldHandler
				, 2);
			AssertPrimitiveFieldHandlerDelegate(typeof(UntypedFieldHandler), primitiveFieldHandler
				, HandlerRegistry.HandlerVersion);
			ArrayHandler arrayHandler = new ArrayHandler(untypedFieldHandler, false);
			AssertCorrectedHandlerVersion(typeof(ArrayHandler0), arrayHandler, 0);
			AssertCorrectedHandlerVersion(typeof(ArrayHandler2), arrayHandler, 1);
			AssertCorrectedHandlerVersion(typeof(ArrayHandler2), arrayHandler, 2);
			AssertCorrectedHandlerVersion(typeof(ArrayHandler), arrayHandler, HandlerRegistry
				.HandlerVersion);
			ArrayHandler multidimensionalArrayHandler = new MultidimensionalArrayHandler(untypedFieldHandler
				, false);
			AssertCorrectedHandlerVersion(typeof(MultidimensionalArrayHandler0), multidimensionalArrayHandler
				, 0);
			AssertCorrectedHandlerVersion(typeof(MultidimensionalArrayHandler), multidimensionalArrayHandler
				, 1);
			AssertCorrectedHandlerVersion(typeof(MultidimensionalArrayHandler), multidimensionalArrayHandler
				, 2);
			AssertCorrectedHandlerVersion(typeof(MultidimensionalArrayHandler), multidimensionalArrayHandler
				, HandlerRegistry.HandlerVersion);
		}

		private void AssertPrimitiveFieldHandlerDelegate(Type fieldHandlerClass, PrimitiveFieldHandler
			 primitiveFieldHandler, int version)
		{
			PrimitiveFieldHandler primitiveFieldHandler0 = (PrimitiveFieldHandler)CorrectHandlerVersion
				(primitiveFieldHandler, version);
			Assert.AreSame(fieldHandlerClass, primitiveFieldHandler0.DelegateTypeHandler().GetType
				());
		}

		private ClassMetadata ItemClassMetadata()
		{
			return Stream().ClassMetadataForObject(new HandlerRegistryTestCase.Item());
		}

		private void AssertCorrectedHandlerVersion(Type expectedClass, ITypeHandler4 typeHandler
			, int version)
		{
			Assert.AreSame(expectedClass, CorrectHandlerVersion(typeHandler, version).GetType
				());
		}

		private ITypeHandler4 CorrectHandlerVersion(ITypeHandler4 typeHandler, int version
			)
		{
			return Handlers().CorrectHandlerVersion(typeHandler, version);
		}

		public virtual void TestInterfaceHandlerIsSameAsObjectHandler()
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
			if (NullableArrayHandling.Disabled())
			{
				return ReflectorFor(typeof(int));
			}
			else
			{
				return ReflectorFor(Platform4.NullableTypeFor(typeof(int)));
			}
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
