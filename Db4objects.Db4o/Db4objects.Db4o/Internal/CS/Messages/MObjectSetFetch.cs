namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetFetch : Db4objects.Db4o.Internal.CS.Messages.MObjectSet, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			int queryResultID = ReadInt();
			int fetchSize = ReadInt();
			Db4objects.Db4o.Foundation.IIntIterator4 idIterator = Stub(queryResultID).IdIterator
				();
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = ID_LIST.GetWriterForLength(Transaction
				(), BufferLength(fetchSize));
			Db4objects.Db4o.Internal.StatefulBuffer writer = message.PayLoad();
			writer.WriteIDs(idIterator, fetchSize);
			Write(message);
			return true;
		}

		private int BufferLength(int fetchSize)
		{
			return Db4objects.Db4o.Internal.Const4.INT_LENGTH * (fetchSize + 1);
		}
	}
}
