namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// db4o exception wrapper: Exceptions occurring during internal processing
	/// will be proliferated to the client calling code encapsulated in an exception
	/// of rhis type.
	/// </summary>
	/// <remarks>
	/// db4o exception wrapper: Exceptions occurring during internal processing
	/// will be proliferated to the client calling code encapsulated in an exception
	/// of rhis type. The original exception, if any, is available through
	/// <see cref="Db4oException#cause()">Db4oException#cause()</see>
	/// .
	/// </remarks>
	[System.Serializable]
	public class Db4oException : System.Exception
	{
		public Db4oException(string msg) : base(msg)
		{
		}

		public Db4oException(System.Exception cause) : base(cause.Message, cause)
		{
		}

		public Db4oException(int messageConstant) : this(Db4objects.Db4o.Internal.Messages
			.Get(messageConstant))
		{
		}
	}
}
