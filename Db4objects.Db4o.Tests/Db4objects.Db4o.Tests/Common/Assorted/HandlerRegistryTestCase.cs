namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class HandlerRegistryTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public interface IFooInterface
		{
		}

		public virtual void _testInterfaceHandlerIsSameAsObjectHandler()
		{
			Db4oUnit.Assert.AreSame(HandlerForClass(typeof(object)), HandlerForClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.HandlerRegistryTestCase.IFooInterface)
				));
		}

		private Db4objects.Db4o.Internal.ITypeHandler4 HandlerForClass(System.Type clazz)
		{
			return Handlers().HandlerForClass(Stream(), ReflectClass(clazz));
		}

		private Db4objects.Db4o.Internal.HandlerRegistry Handlers()
		{
			return Stream().Handlers();
		}
	}
}
