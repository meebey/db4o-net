namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MReadBytes : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override Db4objects.Db4o.Internal.Buffer GetByteLoad()
		{
			int address = _payLoad.ReadInt();
			int length = _payLoad.GetLength() - (Db4objects.Db4o.Internal.Const4.INT_LENGTH);
			_payLoad.RemoveFirstBytes(Db4objects.Db4o.Internal.Const4.INT_LENGTH);
			_payLoad.UseSlot(address, length);
			return this._payLoad;
		}

		public sealed override Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriter(Db4objects.Db4o.Internal.StatefulBuffer
			 bytes)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(bytes.GetTransaction
				(), bytes.GetLength() + Db4objects.Db4o.Internal.Const4.INT_LENGTH);
			message._payLoad.WriteInt(bytes.GetAddress());
			message._payLoad.Append(bytes._buffer);
			return message;
		}

		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			int address = ReadInt();
			int length = ReadInt();
			lock (StreamLock())
			{
				Db4objects.Db4o.Internal.StatefulBuffer bytes = new Db4objects.Db4o.Internal.StatefulBuffer
					(this.Transaction(), address, length);
				try
				{
					Stream().ReadBytes(bytes._buffer, address, length);
					serverThread.Write(GetWriter(bytes));
				}
				catch
				{
					serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.NULL);
				}
			}
			return true;
		}
	}
}
