/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Tests.Common.Handlers;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class CustomTypeHandlerTestCase : AbstractDb4oTestCase
	{
		private static readonly int[] Data = new int[] { 1, 2 };

		public static void Main(string[] arguments)
		{
			new CustomTypeHandlerTestCase().RunSolo();
		}

		private static bool prepareComparisonCalled;

		private sealed class CustomItemTypeHandler : ITypeHandler4, IVariableLengthTypeHandler
		{
			public IPreparedComparison PrepareComparison(IContext context, object obj)
			{
				CustomTypeHandlerTestCase.prepareComparisonCalled = true;
				return new _IPreparedComparison_36();
			}

			private sealed class _IPreparedComparison_36 : IPreparedComparison
			{
				public _IPreparedComparison_36()
				{
				}

				public int CompareTo(object obj)
				{
					return 0;
				}
			}

			public void Write(IWriteContext context, object obj)
			{
				CustomTypeHandlerTestCase.Item item = (CustomTypeHandlerTestCase.Item)obj;
				CustomTypeHandlerTestCase.AssertCurrentSlot(context, item);
				if (item.numbers == null)
				{
					context.WriteInt(-1);
					return;
				}
				context.WriteInt(item.numbers.Length);
				for (int i = 0; i < item.numbers.Length; i++)
				{
					context.WriteInt(item.numbers[i]);
				}
			}

			public object Read(IReadContext context)
			{
				CustomTypeHandlerTestCase.Item item = (CustomTypeHandlerTestCase.Item)((UnmarshallingContext
					)context).PersistentObject();
				CustomTypeHandlerTestCase.AssertCurrentSlot(context, item);
				int elementCount = context.ReadInt();
				if (elementCount == -1)
				{
					return item;
				}
				item.numbers = new int[elementCount];
				for (int i = 0; i < item.numbers.Length; i++)
				{
					item.numbers[i] = context.ReadInt();
				}
				return item;
			}

			/// <exception cref="Db4oIOException"></exception>
			public void Delete(IDeleteContext context)
			{
			}

			public void Defragment(IDefragmentContext context)
			{
			}

			internal CustomItemTypeHandler(CustomTypeHandlerTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly CustomTypeHandlerTestCase _enclosing;
		}

		private sealed class CustomItemGrandChildTypeHandler : ITypeHandler4, IVariableLengthTypeHandler
		{
			public IPreparedComparison PrepareComparison(IContext context, object obj)
			{
				CustomTypeHandlerTestCase.prepareComparisonCalled = true;
				return new _IPreparedComparison_86();
			}

			private sealed class _IPreparedComparison_86 : IPreparedComparison
			{
				public _IPreparedComparison_86()
				{
				}

				public int CompareTo(object obj)
				{
					return 0;
				}
			}

			public void Write(IWriteContext context, object obj)
			{
				CustomTypeHandlerTestCase.AssertCurrentSlot(context, 0);
				CustomTypeHandlerTestCase.ItemGrandChild item = (CustomTypeHandlerTestCase.ItemGrandChild
					)obj;
				context.WriteInt(item.age);
				context.WriteInt(100);
			}

			public object Read(IReadContext context)
			{
				CustomTypeHandlerTestCase.AssertCurrentSlot(context, 0);
				CustomTypeHandlerTestCase.ItemGrandChild item = (CustomTypeHandlerTestCase.ItemGrandChild
					)((UnmarshallingContext)context).PersistentObject();
				item.age = context.ReadInt();
				int check = context.ReadInt();
				if (check != 100)
				{
					throw new InvalidOperationException();
				}
				return item;
			}

			/// <exception cref="Db4oIOException"></exception>
			public void Delete(IDeleteContext context)
			{
			}

			public void Defragment(IDefragmentContext context)
			{
			}

			internal CustomItemGrandChildTypeHandler(CustomTypeHandlerTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly CustomTypeHandlerTestCase _enclosing;
		}

		public class Item
		{
			public int[] numbers;

			public Item(int[] numbers_)
			{
				numbers = numbers_;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is CustomTypeHandlerTestCase.Item))
				{
					return false;
				}
				return AreEqual(numbers, ((CustomTypeHandlerTestCase.Item)obj).numbers);
			}

			private bool AreEqual(int[] expected, int[] actual)
			{
				if (expected == null)
				{
					return actual == null;
				}
				if (actual == null)
				{
					return false;
				}
				if (expected.Length != actual.Length)
				{
					return false;
				}
				for (int i = 0; i < expected.Length; i++)
				{
					if (expected[i] != actual[i])
					{
						return false;
					}
				}
				return true;
			}
		}

		public class ItemChild : CustomTypeHandlerTestCase.Item
		{
			public string name;

			public ItemChild(string name_, int[] numbers_) : base(numbers_)
			{
				name = name_;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is CustomTypeHandlerTestCase.ItemChild))
				{
					return false;
				}
				CustomTypeHandlerTestCase.ItemChild other = (CustomTypeHandlerTestCase.ItemChild)
					obj;
				if (name == null)
				{
					if (other.name != null)
					{
						return false;
					}
					return base.Equals(obj);
				}
				if (!name.Equals(other.name))
				{
					return false;
				}
				return base.Equals(obj);
			}
		}

		public class ItemGrandChild : CustomTypeHandlerTestCase.ItemChild
		{
			public int age;

			public ItemGrandChild(int age_, string name_, int[] numbers_) : base(name_, numbers_
				)
			{
				age = age_;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is CustomTypeHandlerTestCase.ItemGrandChild))
				{
					return false;
				}
				CustomTypeHandlerTestCase.ItemGrandChild other = (CustomTypeHandlerTestCase.ItemGrandChild
					)obj;
				if (age != other.age)
				{
					return false;
				}
				return base.Equals(obj);
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			RegisterTypeHandler(config, typeof(CustomTypeHandlerTestCase.Item), new CustomTypeHandlerTestCase.CustomItemTypeHandler
				(this));
			RegisterTypeHandler(config, typeof(CustomTypeHandlerTestCase.ItemGrandChild), new 
				CustomTypeHandlerTestCase.CustomItemGrandChildTypeHandler(this));
		}

		private void RegisterTypeHandler(IConfiguration config, Type clazz, ITypeHandler4
			 typeHandler)
		{
			GenericReflector reflector = ((Config4Impl)config).Reflector();
			IReflectClass itemClass = reflector.ForClass(clazz);
			ITypeHandlerPredicate predicate = new _ITypeHandlerPredicate_216(itemClass);
			config.RegisterTypeHandler(predicate, typeHandler);
		}

		private sealed class _ITypeHandlerPredicate_216 : ITypeHandlerPredicate
		{
			public _ITypeHandlerPredicate_216(IReflectClass itemClass)
			{
				this.itemClass = itemClass;
			}

			public bool Match(IReflectClass classReflector, int version)
			{
				return itemClass.Equals(classReflector);
			}

			private readonly IReflectClass itemClass;
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(StoredItem());
			Store(StoredItemChild());
			Store(StoredItemGrandChild());
		}

		public virtual void TestConfiguration()
		{
			ClassMetadata classMetadata = Stream().ClassMetadataForReflectClass(ItemClass());
			prepareComparisonCalled = false;
			classMetadata.PrepareComparison(Stream().Transaction().Context(), null);
			Assert.IsTrue(prepareComparisonCalled);
		}

		public virtual void TestRetrieveOnlyInstance()
		{
			Assert.AreEqual(StoredItem(), RetrieveItemOfClass(typeof(CustomTypeHandlerTestCase.Item
				)));
		}

		public virtual void TestChildClass()
		{
			Assert.AreEqual(StoredItemChild(), RetrieveItemOfClass(typeof(CustomTypeHandlerTestCase.ItemChild
				)));
		}

		public virtual void _testGrandChildClass()
		{
			Assert.AreEqual(StoredItemGrandChild(), RetrieveItemOfClass(typeof(CustomTypeHandlerTestCase.ItemGrandChild
				)));
		}

		private CustomTypeHandlerTestCase.Item RetrieveItemOfClass(Type class1)
		{
			IQuery q = NewQuery(class1);
			CustomTypeHandlerTestCase.Item retrievedItem = (CustomTypeHandlerTestCase.Item)q.
				Execute().Next();
			return retrievedItem;
		}

		private CustomTypeHandlerTestCase.Item StoredItem()
		{
			return new CustomTypeHandlerTestCase.Item(Data);
		}

		private CustomTypeHandlerTestCase.Item StoredItemChild()
		{
			return new CustomTypeHandlerTestCase.ItemChild("child", Data);
		}

		private CustomTypeHandlerTestCase.Item StoredItemGrandChild()
		{
			return new CustomTypeHandlerTestCase.ItemGrandChild(25, "child", Data);
		}

		internal virtual IReflectClass ItemClass()
		{
			return Reflector().ForClass(typeof(CustomTypeHandlerTestCase.Item));
		}

		internal static void AssertCurrentSlot(object context, CustomTypeHandlerTestCase.Item
			 item)
		{
			int expectedSlot = 0;
			if (item is CustomTypeHandlerTestCase.ItemChild)
			{
				expectedSlot = 1;
			}
			if (item is CustomTypeHandlerTestCase.ItemGrandChild)
			{
				expectedSlot = 1;
			}
			AssertCurrentSlot(context, expectedSlot);
		}

		internal static void AssertCurrentSlot(object context, int expectedSlot)
		{
			Assert.AreEqual(expectedSlot, ((IMarshallingInfo)context).CurrentSlot());
		}
	}
}
