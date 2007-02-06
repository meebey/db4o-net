namespace Db4objects.Db4o.Internal
{
	internal sealed class Message
	{
		internal readonly System.IO.TextWriter stream;

		internal Message(Db4objects.Db4o.Internal.ObjectContainerBase a_stream, string msg
			)
		{
			stream = a_stream.ConfigImpl().OutStream();
			Print(msg, true);
		}

		internal Message(string a_StringParam, int a_intParam, System.IO.TextWriter a_stream
			, bool header)
		{
			stream = a_stream;
			Print(Db4objects.Db4o.Internal.Messages.Get(a_intParam, a_StringParam), header);
		}

		internal Message(string a_StringParam, int a_intParam, System.IO.TextWriter a_stream
			) : this(a_StringParam, a_intParam, a_stream, true)
		{
		}

		private void Print(string msg, bool header)
		{
			if (stream != null)
			{
				if (header)
				{
					stream.WriteLine("[" + Db4objects.Db4o.Db4oFactory.Version() + "   " + Db4objects.Db4o.Internal.Handlers.DateHandler
						.Now() + "] ");
				}
				stream.WriteLine(" " + msg);
			}
		}
	}
}
