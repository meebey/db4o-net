namespace Db4objects.Db4o.IO
{
	/// <exclude></exclude>
	[System.Serializable]
	public class UncheckedIOException : System.Exception
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

		public UncheckedIOException(System.Exception cause) : base(cause.Message, cause)
		{
		}
	}
}
