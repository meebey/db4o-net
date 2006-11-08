namespace Db4objects.Db4o.CS.Messages
{
	public class MReadBlob : Db4objects.Db4o.CS.Messages.MsgBlob
	{
		public override void ProcessClient(Db4objects.Db4o.Foundation.Network.IYapSocket 
			sock)
		{
			Db4objects.Db4o.CS.Messages.Msg message = Db4objects.Db4o.CS.Messages.Msg.ReadMessage
				(Transaction(), sock);
			if (message.Equals(Db4objects.Db4o.CS.Messages.Msg.LENGTH))
			{
				try
				{
					_currentByte = 0;
					_length = message.PayLoad().ReadInt();
					_blob.GetStatusFrom(this);
					_blob.SetStatus(Db4objects.Db4o.Ext.Status.PROCESSING);
					Copy(sock, this._blob.GetClientOutputStream(), _length, true);
					message = Db4objects.Db4o.CS.Messages.Msg.ReadMessage(Transaction(), sock);
					if (message.Equals(Db4objects.Db4o.CS.Messages.Msg.OK))
					{
						this._blob.SetStatus(Db4objects.Db4o.Ext.Status.COMPLETED);
					}
					else
					{
						this._blob.SetStatus(Db4objects.Db4o.Ext.Status.ERROR);
					}
				}
				catch
				{
				}
			}
			else
			{
				if (message.Equals(Db4objects.Db4o.CS.Messages.Msg.ERROR))
				{
					this._blob.SetStatus(Db4objects.Db4o.Ext.Status.ERROR);
				}
			}
		}

		public override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Db4objects.Db4o.YapStream stream = Stream();
			try
			{
				Db4objects.Db4o.BlobImpl blobImpl = this.ServerGetBlobImpl();
				if (blobImpl != null)
				{
					blobImpl.SetTrans(Transaction());
					Sharpen.IO.File file = blobImpl.ServerFile(null, false);
					int length = (int)file.Length();
					Db4objects.Db4o.Foundation.Network.IYapSocket sock = serverThread.Socket();
					Db4objects.Db4o.CS.Messages.Msg.LENGTH.GetWriterForInt(Transaction(), length).Write
						(stream, sock);
					Sharpen.IO.FileInputStream fin = new Sharpen.IO.FileInputStream(file);
					Copy(fin, sock, false);
					sock.Flush();
					Db4objects.Db4o.CS.Messages.Msg.OK.Write(stream, sock);
				}
			}
			catch
			{
				serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.ERROR);
			}
			return true;
		}
	}
}
