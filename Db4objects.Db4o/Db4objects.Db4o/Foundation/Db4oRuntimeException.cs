namespace Db4objects.Db4o.Foundation
{
	[System.Serializable]
	public class Db4oRuntimeException : System.Exception
	{
		public Db4oRuntimeException() : base()
		{
		}

		public Db4oRuntimeException(System.Exception cause) : this(cause.Message, cause)
		{
		}

		public Db4oRuntimeException(string msg) : base(msg)
		{
		}

		public Db4oRuntimeException(string msg, System.Exception cause) : base(msg, cause
			)
		{
		}
	}
}
