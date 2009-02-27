/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Reflect;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Tests.Common.Handlers.Framework;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Tests.Common.Handlers.Framework
{
	public class TypeHandlerCanInstantiateTestCase : AbstractDb4oTestCase
	{
		public class Item
		{
			private int _id;

			public Item(int id)
			{
				_id = id;
			}

			public virtual int Id()
			{
				return _id;
			}
		}

		public class ItemHandler : ITypeHandler4
		{
			public virtual void Defragment(IDefragmentContext context)
			{
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			public virtual void Delete(IDeleteContext context)
			{
			}

			public virtual object Read(IReadContext context)
			{
				int id = context.ReadInt();
				return null;
			}

			public virtual void Write(IWriteContext context, object obj)
			{
				context.WriteInt(((TypeHandlerCanInstantiateTestCase.Item)obj).Id());
			}

			public virtual IPreparedComparison PrepareComparison(IContext context, object obj
				)
			{
				// TODO Auto-generated method stub
				return null;
			}

			public virtual bool CanHold(IReflectClass type)
			{
				return ReflectClasses.AreEqual(typeof(TypeHandlerCanInstantiateTestCase.Item), type
					);
			}
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.RegisterTypeHandler(new SingleClassTypeHandlerPredicate(typeof(TypeHandlerCanInstantiateTestCase.Item
				)), new TypeHandlerCanInstantiateTestCase.ItemHandler());
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Store()
		{
			Store(new TypeHandlerCanInstantiateTestCase.Item(42));
		}

		public virtual void TestField()
		{
			Assert.AreEqual(42, ((TypeHandlerCanInstantiateTestCase.Item)RetrieveOnlyInstance
				(typeof(TypeHandlerCanInstantiateTestCase.Item))).Id());
		}

		public virtual void TestIdentity()
		{
			Assert.AreSame(((TypeHandlerCanInstantiateTestCase.Item)RetrieveOnlyInstance(typeof(
				TypeHandlerCanInstantiateTestCase.Item))), ((TypeHandlerCanInstantiateTestCase.Item
				)RetrieveOnlyInstance(typeof(TypeHandlerCanInstantiateTestCase.Item))));
		}

		public virtual void _testQuery()
		{
			TypeHandlerCanInstantiateTestCase.Item found = ItemById(42);
			Assert.AreSame(((TypeHandlerCanInstantiateTestCase.Item)RetrieveOnlyInstance(typeof(
				TypeHandlerCanInstantiateTestCase.Item))), found);
		}

		private TypeHandlerCanInstantiateTestCase.Item ItemById(int id)
		{
			IQuery query = NewQuery(typeof(TypeHandlerCanInstantiateTestCase.Item));
			query.Descend("_id").Constrain(id);
			IObjectSet found = query.Execute();
			return found.HasNext() ? (TypeHandlerCanInstantiateTestCase.Item)found.Next() : null;
		}
	}
}
