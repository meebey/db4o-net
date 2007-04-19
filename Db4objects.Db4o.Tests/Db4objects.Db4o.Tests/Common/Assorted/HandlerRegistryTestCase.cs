using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal;
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
			Assert.AreSame(HandlerForClass(typeof(object)), HandlerForClass(typeof(HandlerRegistryTestCase.IFooInterface)
				));
		}

		private ITypeHandler4 HandlerForClass(Type clazz)
		{
			return Handlers().HandlerForClass(Stream(), ReflectClass(clazz));
		}

		private HandlerRegistry Handlers()
		{
			return Stream().Handlers();
		}
	}
}
