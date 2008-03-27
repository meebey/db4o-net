/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Handlers;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class CustomTypeHandlerTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] arguments)
		{
			new CustomTypeHandlerTestCase().RunSolo();
		}

		private static bool prepareComparisonCalled;

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

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			ITypeHandler4 customTypeHandler = new _ITypeHandler4_59();
			// TODO Auto-generated method stub
			// need to write something, to prevent NPE
			// TODO Auto-generated method stub
			// TODO Auto-generated method stub
			// TODO Auto-generated method stub
			IReflectClass claxx = ((Config4Impl)config).Reflector().ForClass(typeof(CustomTypeHandlerTestCase.Item
				));
			ITypeHandlerPredicate predicate = new _ITypeHandlerPredicate_93(claxx);
			config.RegisterTypeHandler(predicate, customTypeHandler);
		}

		private sealed class _ITypeHandler4_59 : ITypeHandler4
		{
			public _ITypeHandler4_59()
			{
			}

			public IPreparedComparison PrepareComparison(IContext context, object obj)
			{
				CustomTypeHandlerTestCase.prepareComparisonCalled = true;
				return null;
			}

			public void Write(IWriteContext context, object obj)
			{
				context.WriteInt(0);
			}

			public object Read(IReadContext context)
			{
				return null;
			}

			/// <exception cref="Db4oIOException"></exception>
			public void Delete(IDeleteContext context)
			{
			}

			public void Defragment(IDefragmentContext context)
			{
			}
		}

		private sealed class _ITypeHandlerPredicate_93 : ITypeHandlerPredicate
		{
			public _ITypeHandlerPredicate_93(IReflectClass claxx)
			{
				this.claxx = claxx;
			}

			public bool Match(IReflectClass classReflector, int version)
			{
				return claxx.Equals(classReflector);
			}

			private readonly IReflectClass claxx;
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(StoredItem());
		}

		public virtual void TestConfiguration()
		{
			ClassMetadata classMetadata = Stream().ClassMetadataForReflectClass(ItemClass());
			prepareComparisonCalled = false;
			classMetadata.PrepareComparison(Stream().Transaction().Context(), null);
			Assert.IsTrue(prepareComparisonCalled);
		}

		public virtual void _test()
		{
			CustomTypeHandlerTestCase.Item retrievedItem = (CustomTypeHandlerTestCase.Item)RetrieveOnlyInstance
				(typeof(CustomTypeHandlerTestCase.Item));
			Assert.AreEqual(StoredItem(), retrievedItem);
		}

		private CustomTypeHandlerTestCase.Item StoredItem()
		{
			return new CustomTypeHandlerTestCase.Item(new int[] { 1, 2 });
		}

		internal virtual IReflectClass ItemClass()
		{
			return Reflector().ForClass(typeof(CustomTypeHandlerTestCase.Item));
		}
	}
}
