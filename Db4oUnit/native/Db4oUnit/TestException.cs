namespace Db4oUnit
{
	public class TestException : System.Exception
	{
		public TestException(System.Exception reason) : base(reason.Message, reason)
		{
		}

		public System.Exception GetReason()
		{
			return this.InnerException;
		}
	}
}
