using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
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

			public Component()
			{	
			}

			public Component(T value_)
			{
				value = value_;
			}
		}

		class Container<T>
		{
			public List<T> elements;

			public Container()
			{	
			}

			public Container(params T[] values)
			{
				elements = new List<T>(values);
			}
		}

		class Container
		{
			public Object elements;

			public Container()
			{	
			}

			public Container(params object[] values)
			{
				elements = new List<object>(values);
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ObjectClass(typeof(Container<Component<string>>)).CascadeOnDelete(true);
			config.ExceptionsOnNotStorable(true);
			config.RegisterTypeHandler(new GenericListPredicate(), new GenericListTypeHandler());
		}

		protected override void Store()
		{
			Component<int> componentM1 = new Component<int>(-1);
			Store(new Container<Component<int>>(componentM1, null, new Component<int>(42)));
			Store(new Container<int>(-1, 42));
			Store(new Component<Component<int>>(componentM1));
			Store(new Container<int?>(-1, null, 42));
			Store(new Container("foo", "bar"));

			// TestCascadeDelete
			Store(new Container<Component<string>>(new Component<string>("foo"), new Component<string>("bar")));
		}

		public void TestUntypedField()
		{
			Container container = RetrieveOnlyInstance<Container>();
			Assert.IsNotNull(container.elements);

			IList<object> list = (IList<object>) container.elements;
			Iterator4Assert.AreEqual(new object[] { "foo", "bar" }, list.GetEnumerator());
		}

		public void TestListOfNullables()
		{
			Container<int?> container = RetrieveOnlyInstance<Container<int?>>();
			int?[] expected = new int?[] { -1, null, 42};
			Iterator4Assert.AreEqual(expected.GetEnumerator(), container.elements.GetEnumerator());
		}

		public void _TestSimpleDelete()
		{
			DeleteAll(typeof(Container<int>));
			Reopen();
			AssertOccurrences(typeof(Container<int>), 0);
		}

		public void _TestCascadeDelete()
		{	
			DeleteAll(typeof(Container<Component<string>>));
			Reopen();
			AssertOccurrences(typeof(Container<Component<string>>), 0);
			AssertOccurrences(typeof(Component<string>), 0);
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

		internal class GenericListTypeHandler : ITypeHandler4, IVariableLengthTypeHandler, IEmbeddedTypeHandler, IFirstClassHandler
		{
			public void Defragment(IDefragmentContext context)
			{
				throw new NotImplementedException();
			}

			public void Delete(IDeleteContext context)
			{
				throw new NotImplementedException();
			}

			public void Write(IWriteContext context, object obj)
			{
				IList list = (IList)obj;
				WriteClassMetadata(context, list);
				WriteElementCount(context, list);

				if (IsNullableTypeList(list))
				{
					WriteNullableList(context, list);
				}
				else
				{
					WriteRegularList(context, list);
				}
			}

			private void WriteRegularList(IWriteContext context, IList list)
			{
				ITypeHandler4 elementHandler = ElementTypeHandler(context, list);
				foreach (object o in list)
				{
					context.WriteObject(elementHandler, o);
				}
			}

			private void WriteNullableList(IWriteContext context, IList list)
			{
				BitMap4 nullBitmap = NullBitmapFor(list);
				WriteBitmap(context, nullBitmap);
				WriteNullableListElements(context, list);
			}

			private static bool IsNullableTypeList(IList list)
			{
				Type type = ElementType(list);
				if (!type.IsGenericType) return false;
				return type.GetGenericTypeDefinition() == typeof(Nullable<>);
			}

			private void WriteBitmap(IWriteContext context, BitMap4 bitmap)
			{	
				context.WriteBytes(bitmap.Bytes());
			}

			private BitMap4 ReadBitMap(int bits, IReadContext context)
			{
				BitMap4 bitmap = new BitMap4(bits);
				context.ReadBytes(bitmap.Bytes());
				return bitmap;
			}

			private BitMap4 NullBitmapFor(IList list)
			{
				BitMap4 bitmap = new BitMap4(list.Count);
				int bit = 0;
				foreach (object o in list)
				{
					bitmap.Set(bit, o == null);
					++bit;
				}
				return bitmap;
			}

			public object Read(IReadContext context)
			{
				IList list = NewList(ReadClassMetadata(context));
				int count = ReadElementCount(context);
				if (IsNullableTypeList(list))
				{
					BitMap4 bitmap = ReadBitMap(count, context);
					ReadElements(context, bitmap, list, count);
				}
				else
				{
					ReadElements(context, list, count);
				}
				return list;
			}

			private void ReadElements(IReadContext context, IList list, int count)
			{
				ITypeHandler4 elementHandler = ElementTypeHandler(context, list);
				for (int i = 0; i < count; ++i)
				{
					list.Add(context.ReadObject(elementHandler));
				}
			}

			private static int ReadElementCount(IReadContext context)
			{
				return context.ReadInt();
			}

			private void WriteClassMetadata(IWriteContext context, IList list)
			{
				int id = ClassMetadataId(context, list);
				context.WriteInt(id);
			}

			private int ClassMetadataId(IWriteContext context, IList list)
			{
				ObjectContainerBase container = Container(context);
				return container.ProduceClassMetadata(container.Reflector().ForObject(list)).GetID();
			}

			private static IList NewList(ClassMetadata metadata)
			{
				Type type = NetReflector.ToNative(metadata.ClassReflector());
				return NewList(type);
			}

			private static IList NewList(Type type)
			{
				return (IList)Activator.CreateInstance(type);
			}

			private ClassMetadata ReadClassMetadata(IReadContext context)
			{
				int classMetadataId = context.ReadInt();
				return Container(context).ClassMetadataForId(classMetadataId);
			}

			private interface ICollectionInitializer
			{
                object Collection { set; }
                void Add(object o);
				void AddDefaultValue();
			}

			private class GenericCollectionInitializer<T> : ICollectionInitializer
			{
				private ICollection<T> _collection;

				public void Add(object o)
				{
                    _collection.Add((T)o);
				}

				public void AddDefaultValue()
				{
					_collection.Add(default(T));
				}

                public object Collection
                {
                    set
                    {
                        _collection = (ICollection<T>) value;
                    }
                }
			}

			private void ReadElements(IReadContext context, BitMap4 nullBitmap, IList list, long count)
			{
				ICollectionInitializer initializer = CollectionInitializerFor(list);
				ITypeHandler4 elementHandler = ElementTypeHandler(context, list);
				for (int i = 0; i < count; ++i)
				{
					if (nullBitmap.IsTrue(i))
					{
						initializer.AddDefaultValue();
					}
					else
					{
						initializer.Add(context.ReadObject(elementHandler));
					}
				}
			}

            private static ICollectionInitializer CollectionInitializerFor(IList list)
            {
                Type collectionInitializerType = typeof(GenericCollectionInitializer<>).MakeGenericType(ElementType(list));
                ICollectionInitializer collectionInitializer = (ICollectionInitializer)Activator.CreateInstance(collectionInitializerType);
                collectionInitializer.Collection = list;

                return collectionInitializer;
            }

			private static void WriteElementCount(IWriteContext context, IList list)
			{
				context.WriteInt(list.Count);
			}

			private void WriteNullableListElements(IWriteContext context, IList list)
			{
				ITypeHandler4 elementHandler = ElementTypeHandler(context, list);
				foreach (object element in list)
				{
					if (element == null) continue;
					context.WriteObject(elementHandler, element);
				}
			}

			private ObjectContainerBase Container(IContext context)
			{
				return ContainerBase(context.ObjectContainer());
			}

			private static ObjectContainerBase ContainerBase(IObjectContainer container)
			{
				return ((IInternalObjectContainer)container).Container();
			}

			private ITypeHandler4 ElementTypeHandler(IContext context, IList list)
			{
				return ElementTypeHandler(Container(context), list);
			}

			private static ITypeHandler4 ElementTypeHandler(ObjectContainerBase container, IList list)
			{
				IReflectClass elementClass = ElementClass(container, list);
				if (NetReflector.ToNative(elementClass) == typeof(object))
				{
					return container.Handlers().UntypedObjectHandler();
				}
				return container.Handlers().TypeHandlerForClass(elementClass);
			}

			private static IReflectClass ElementClass(ObjectContainerBase container, IList list)
			{
				return container.Reflector().ForClass(ElementType(list));
			}

			private static Type ElementType(IList list)
			{
				return list.GetType().GetGenericArguments()[0];
			}

			public IPreparedComparison PrepareComparison(IContext context, object obj)
			{
				throw new NotImplementedException();
			}

            public void CascadeActivation(ActivationContext4 context)
            {
                IEnumerator all = ((IList)context.TargetObject()).GetEnumerator();
                while (all.MoveNext())
                {
                    context.CascadeActivationToChild(all.Current);
                }
            }

            public ITypeHandler4 ReadCandidateHandler(QueryingReadContext context)
            {
                throw new NotImplementedException();
            }

            public void ReadCandidates(QueryingReadContext context)
            {
                throw new NotImplementedException();
            }

		    public void CollectIDs(QueryingReadContext context)
		    {
		        throw new System.NotImplementedException();
		    }
		}

		internal class GenericListPredicate : ITypeHandlerPredicate
		{
			public bool Match(IReflectClass classReflector)
			{
				Type type = NetReflector.ToNative(classReflector);
				if (!type.IsGenericType) return false;
				return type.GetGenericTypeDefinition() == typeof(List<>);
			}
		}
	}
}
