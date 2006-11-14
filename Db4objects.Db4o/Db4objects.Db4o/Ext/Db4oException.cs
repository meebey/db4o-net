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
	/// <see cref="Db4objects.Db4o.Ext.Db4oException.Cause">Db4objects.Db4o.Ext.Db4oException.Cause
	/// 	</see>
	/// .
	/// </remarks>
	[System.Serializable]
	public class Db4oException : System.Exception
	{
		private System.Exception _cause;

		public Db4oException(string msg) : base(msg)
		{
		}

		public Db4oException(System.Exception cause) : this(cause.ToString())
		{
			_cause = cause;
		}

		public Db4oException(int messageConstant) : this(Db4objects.Db4o.Messages.Get(messageConstant
			))
		{
		}

		/// <returns>The originating exception, if any</returns>
		public virtual System.Exception Cause()
		{
			return _cause;
		}
	}
}
