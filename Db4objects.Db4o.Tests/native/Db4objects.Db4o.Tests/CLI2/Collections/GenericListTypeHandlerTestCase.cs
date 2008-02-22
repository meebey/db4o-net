using System;
using System.Collections.Generic;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Net;
using Db4objects.Db4o.Typehandlers;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.Collections
{
	class GenericListTypeHandlerTestCase : AbstractDb4oTestCase
	{
		class Item
		{
			public List<int> value = new List<int>();
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.RegisterTypeHandler(new GenericListPredicate(), new GenericListTypeHandler());
		}

		protected override void Store()
		{
			Store(new Item());
		}

		public void Test()
		{

		}

		internal class GenericListTypeHandler : ITypeHandler4
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
				throw new NotImplementedException();
			}

			public void Write(IWriteContext context, object obj)
			{
				throw new NotImplementedException();
			}

			public IPreparedComparison PrepareComparison(object obj)
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
