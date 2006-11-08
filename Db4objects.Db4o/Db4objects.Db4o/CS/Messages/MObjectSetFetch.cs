namespace Db4objects.Db4o.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetFetch : Db4objects.Db4o.CS.Messages.MObjectSet
	{
		public override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			int queryResultID = ReadInt();
			int fetchSize = ReadInt();
			Db4objects.Db4o.Foundation.IIntIterator4 idIterator = Stub(serverThread, queryResultID
				).IdIterator();
			Db4objects.Db4o.CS.Messages.MsgD message = ID_LIST.GetWriterForLength(Transaction
				(), BufferLength(fetchSize));
			Db4objects.Db4o.YapWriter writer = message.PayLoad();
			writer.WriteIDs(idIterator, fetchSize);
			serverThread.Write(message);
			return true;
		}

		private int BufferLength(int fetchSize)
		{
			return Db4objects.Db4o.YapConst.INT_LENGTH * (fetchSize + 1);
		}
	}
}
