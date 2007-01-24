namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class YapHandlersTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public interface IFooInterface
		{
		}

		public virtual void _testInterfaceHandlerIsSameAsObjectHandler()
		{
			Db4oUnit.Assert.AreSame(HandlerForClass(typeof(object)), HandlerForClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.YapHandlersTestCase.IFooInterface)
				));
		}

		private Db4objects.Db4o.ITypeHandler4 HandlerForClass(System.Type clazz)
		{
			return Handlers().HandlerForClass(Stream(), ReflectClass(clazz));
		}

		private Db4objects.Db4o.YapHandlers Handlers()
		{
			return Stream().Handlers();
		}
	}
}
