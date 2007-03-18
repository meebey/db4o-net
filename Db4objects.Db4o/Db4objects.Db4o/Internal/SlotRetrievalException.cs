namespace Db4objects.Db4o.Internal
{
	[System.Serializable]
	public class SlotRetrievalException : System.Exception
	{
		private int _id;

		public SlotRetrievalException(int id) : this(null, null, id)
		{
		}

		public SlotRetrievalException(string msg, int id) : this(msg, null, id)
		{
		}

		public SlotRetrievalException(System.Exception cause, int id) : this(null, cause, 
			id)
		{
		}

		public SlotRetrievalException(string msg, System.Exception cause, int id) : base(
			EnhancedMessage(msg, id), cause)
		{
			_id = id;
		}

		public virtual int SlotID()
		{
			return _id;
		}

		private static string EnhancedMessage(string msg, int id)
		{
			string enhancedMessage = "Slot ID " + id;
			if (msg != null)
			{
				enhancedMessage += ": " + msg;
			}
			return enhancedMessage;
		}
	}
}
