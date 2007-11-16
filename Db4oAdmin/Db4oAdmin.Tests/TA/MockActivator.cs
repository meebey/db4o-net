using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4oAdmin.Tests.TA
{
	public class MockActivator : IActivator
	{
		public static MockActivator ActivatorFor(object obj)
		{
			MockActivator activator = new MockActivator();
			((IActivatable) obj).Bind(activator);
			return activator;
		}

		private int _count;

		public int Count
		{
			get { return _count; }
		}

		public void Activate()
		{
			++_count;
		}
	}


}
