namespace Db4objects.Db4o.Internal
{
	[System.Serializable]
	public class FieldIndexException : System.Exception
	{
		private Db4objects.Db4o.Internal.FieldMetadata _field;

		public FieldIndexException(Db4objects.Db4o.Internal.FieldMetadata field) : this(null
			, null, field)
		{
		}

		public FieldIndexException(string msg, Db4objects.Db4o.Internal.FieldMetadata field
			) : this(msg, null, field)
		{
		}

		public FieldIndexException(System.Exception cause, Db4objects.Db4o.Internal.FieldMetadata
			 field) : this(null, cause, field)
		{
		}

		public FieldIndexException(string msg, System.Exception cause, Db4objects.Db4o.Internal.FieldMetadata
			 field) : base(EnhancedMessage(msg, field), cause)
		{
			_field = field;
		}

		public virtual Db4objects.Db4o.Internal.FieldMetadata Field()
		{
			return _field;
		}

		private static string EnhancedMessage(string msg, Db4objects.Db4o.Internal.FieldMetadata
			 field)
		{
			string enhancedMessage = "Field index for " + field.GetParentYapClass().GetName()
				 + "#" + field.GetName();
			if (msg != null)
			{
				enhancedMessage += ": " + msg;
			}
			return enhancedMessage;
		}
	}
}
