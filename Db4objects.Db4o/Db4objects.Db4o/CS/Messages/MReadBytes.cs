namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MReadBytes : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override Db4objects.Db4o.YapReader GetByteLoad()
		{
			int address = this._payLoad.ReadInt();
			int length = this._payLoad.GetLength() - (Db4objects.Db4o.YapConst.INT_LENGTH);
			this._payLoad.RemoveFirstBytes(Db4objects.Db4o.YapConst.INT_LENGTH);
			this._payLoad.UseSlot(address, length);
			return this._payLoad;
		}

		public sealed override Db4objects.Db4o.CS.Messages.MsgD GetWriter(Db4objects.Db4o.YapWriter
			 bytes)
		{
			Db4objects.Db4o.CS.Messages.MsgD message = this.GetWriterForLength(bytes.GetTransaction
				(), bytes.GetLength() + Db4objects.Db4o.YapConst.INT_LENGTH);
			message._payLoad.WriteInt(bytes.GetAddress());
			message._payLoad.Append(bytes._buffer);
			return message;
		}

		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapStream stream = GetStream();
			int address = this.ReadInt();
			int length = this.ReadInt();
			lock (stream.i_lock)
			{
				Db4objects.Db4o.YapWriter bytes = new Db4objects.Db4o.YapWriter(this.GetTransaction
					(), address, length);
				try
				{
					stream.ReadBytes(bytes._buffer, address, length);
					GetWriter(bytes).Write(stream, sock);
				}
				catch
				{
					Db4objects.Db4o.CS.Messages.Msg.NULL.Write(stream, sock);
				}
			}
			return true;
		}
	}
}
