using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Net;
using Db4objects.Db4o.Typehandlers;
using Db4oUnit;
using Db4oUnit.Extensions;
using Sharpen.Lang;
using System.Text;

namespace Db4objects.Db4o.Tests.CLI2.Collections
{
	class GenericListTypeHandlerTestCase : AbstractDb4oTestCase
	{
		class Component<T>
		{
			public T value;

			public Component(T value_)
			{
				value = value_;
			}
		}

		class Container<T>
		{
			public List<T> elements = new List<T>();

			public Container(params T[] values)
			{
				elements.AddRange(values);
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.RegisterTypeHandler(new GenericListPredicate(), new GenericListTypeHandler());
		}

		protected override void Store()
		{
			Component<int> componentM1 = new Component<int>(-1);
			Store(new Container<Component<int>>(componentM1, null, new Component<int>(42)));
			Store(new Container<int>(-1, 42));
			Store(new Component<Component<int>>(componentM1));

			Store(new Container<int?>(-1, null, 42));
		}

		public void _TestListOfNullables()
		{
			Container<int?> container = RetrieveOnlyInstance<Container<int?>>();
			int?[] expected = new int?[] { -1, null, 42};
			Iterator4Assert.AreEqual(expected.GetEnumerator(), container.elements.GetEnumerator());
		}

		public void TestListOfFirstClassObjects()
		{
			Container<Component<int>> container = RetrieveOnlyInstance<Container<Component<int>>>();
			Assert.AreEqual(3, container.elements.Count);
			Assert.AreEqual(-1, container.elements[0].value);
			Assert.IsNull(container.elements[1]);
			Assert.AreEqual(42, container.elements[2].value);
		}

		public void TestIdentitySemantics()
		{
			Container<Component<int>> container = RetrieveOnlyInstance<Container<Component<int>>>();
			Component<int> componentM1 = container.elements[0];
			Component<Component<int>> composite = RetrieveOnlyInstance<Component<Component<int>>>();
			Assert.AreSame(componentM1, composite.value);
		}

		public void TestListOfIntegers()
		{
			Container<int> container = RetrieveOnlyInstance<Container<int>>();
			Iterator4Assert.AreEqual(new object[] { -1, 42 }, container.elements.GetEnumerator());
		}

		internal class GenericListTypeHandler : ITypeHandler4, IVariableLengthTypeHandler, IEmbeddedTypeHandler
		{
			public void Delete(IDeleteContext context)
			{
				throw new NotImplementedException();
			}

			public void Defragment(IDefragmentContext context)
			{
				throw new NotImplementedException();
			}

			public object Read(IReadContext context)
			{
				IList list = (IList)Activator.CreateInstance(ReadType(context));
				ReadElements(context, list, context.ReadLong());
				return list;
			}

			public void Write(IWriteContext context, object obj)
			{
				IList list = (IList) obj;
				WriteType(context, obj.GetType());
				WriteElementCount(context, list);
				WriteElements(context, list);
			}

			private void ReadElements(IReadContext context, IList list, long count)
			{
				ITypeHandler4 elementHandler = ElementTypeHandler(context, list);
				for (int i = 0; i < count; ++i)
				{
					list.Add(context.ReadObject(elementHandler));
				}
			}

			private static void WriteElementCount(IWriteContext context, IList list)
			{
				context.WriteLong(list.Count);
			}

			private void WriteElements(IWriteContext context, IList list)
			{
				ITypeHandler4 elementHandler = ElementTypeHandler(context, list);
				foreach (object element in list)
				{
					context.WriteObject(elementHandler, element);
				}
			}

			private static Type ReadType(IReadContext context)
			{
				string typeName = Decode(ReadByteArray(context));

				return TypeReference.FromString(typeName).Resolve();
			}

			private static void WriteType(IWriteContext context, Type type)
			{
				string typeName = TypeReference.FromType(type).GetUnversionedName();
				WriteByteArray(context, Encode(typeName));
			}

			private static string Decode(byte[] bytes)
			{
				return Encoding.UTF8.GetString(bytes);
			}

			private static byte[] ReadByteArray(IReadContext context)
			{
				byte[] bytes = new byte[context.ReadInt()];
				context.ReadBytes(bytes);
				return bytes;
			}

			private static byte[] Encode(string typeName)
			{
				return Encoding.UTF8.GetBytes(typeName);
			}

			private ObjectContainerBase Container(IContext context)
			{
				return ((IInternalObjectContainer)context.ObjectContainer()).Container();
			}

			private ITypeHandler4 ElementTypeHandler(IContext context, IList list)
			{
				Type elementType = list.GetType().GetGenericArguments()[0];

				ObjectContainerBase container = Container(context);
				return container.Handlers().TypeHandlerForClass(container.Reflector().ForClass(elementType));
			}  

			private static void WriteByteArray(IWriteContext context, byte[] bytes)
			{
				context.WriteInt(bytes.Length);
				context.WriteBytes(bytes);
			}

			public IPreparedComparison PrepareComparison(IContext context, object obj)
			{
				throw new NotImplementedException();
			}
		}

		internal class GenericListPredicate : ITypeHandlerPredicate
		{
			public bool Match(IReflectClass classReflector, int version)
			{
				Type type = NetReflector.ToNative(classReflector);
				if (!type.IsGenericType) return false;
				return type.GetGenericTypeDefinition() == typeof(List<>);
			}
		}
	}
}
