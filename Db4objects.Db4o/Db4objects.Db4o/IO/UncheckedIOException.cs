namespace Db4objects.Db4o.IO
{
	[System.Serializable]
	public class UncheckedIOException : Db4objects.Db4o.Foundation.Db4oRuntimeException
	{
		public UncheckedIOException() : base()
		{
		}

		public UncheckedIOException(string msg, System.Exception cause) : base(msg, cause
			)
		{
		}

		public UncheckedIOException(string msg) : base(msg)
		{
		}

		public UncheckedIOException(System.Exception cause) : base(cause)
		{
		}
	}
}
