namespace Db4objects.Db4o.Internal
{
	[System.Serializable]
	public class ClassIndexException : System.Exception
	{
		private Db4objects.Db4o.Internal.ClassMetadata _class;

		public ClassIndexException(Db4objects.Db4o.Internal.ClassMetadata clazz) : this(null
			, null, clazz)
		{
		}

		public ClassIndexException(string msg, Db4objects.Db4o.Internal.ClassMetadata clazz
			) : this(msg, null, clazz)
		{
		}

		public ClassIndexException(System.Exception cause, Db4objects.Db4o.Internal.ClassMetadata
			 clazz) : this(null, cause, clazz)
		{
		}

		public ClassIndexException(string msg, System.Exception cause, Db4objects.Db4o.Internal.ClassMetadata
			 clazz) : base(EnhancedMessage(msg, clazz), cause)
		{
			_class = clazz;
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata Clazz()
		{
			return _class;
		}

		private static string EnhancedMessage(string msg, Db4objects.Db4o.Internal.ClassMetadata
			 clazz)
		{
			string enhancedMessage = "Class index for " + clazz.GetName();
			if (msg != null)
			{
				enhancedMessage += ": " + msg;
			}
			return enhancedMessage;
		}
	}
}
