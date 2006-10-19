namespace Db4objects.Db4o
{
	internal class MReadBlob : Db4objects.Db4o.MsgBlob
	{
		internal override void ProcessClient(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.Msg message = Db4objects.Db4o.Msg.ReadMessage(GetTransaction(), sock
				);
			if (message.Equals(Db4objects.Db4o.Msg.LENGTH))
			{
				try
				{
					_currentByte = 0;
					_length = message.GetPayLoad().ReadInt();
					_blob.GetStatusFrom(this);
					_blob.SetStatus(Db4objects.Db4o.Ext.Status.PROCESSING);
					Copy(sock, this._blob.GetClientOutputStream(), _length, true);
					message = Db4objects.Db4o.Msg.ReadMessage(GetTransaction(), sock);
					if (message.Equals(Db4objects.Db4o.Msg.OK))
					{
						this._blob.SetStatus(Db4objects.Db4o.Ext.Status.COMPLETED);
					}
					else
					{
						this._blob.SetStatus(Db4objects.Db4o.Ext.Status.ERROR);
					}
				}
				catch (System.Exception e)
				{
				}
			}
			else
			{
				if (message.Equals(Db4objects.Db4o.Msg.ERROR))
				{
					this._blob.SetStatus(Db4objects.Db4o.Ext.Status.ERROR);
				}
			}
		}

		internal override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapStream stream = GetStream();
			try
			{
				Db4objects.Db4o.BlobImpl blobImpl = this.ServerGetBlobImpl();
				if (blobImpl != null)
				{
					blobImpl.SetTrans(GetTransaction());
					Sharpen.IO.File file = blobImpl.ServerFile(null, false);
					int length = (int)file.Length();
					Db4objects.Db4o.Msg.LENGTH.GetWriterForInt(GetTransaction(), length).Write(stream
						, sock);
					Sharpen.IO.FileInputStream fin = new Sharpen.IO.FileInputStream(file);
					Copy(fin, sock, false);
					sock.Flush();
					Db4objects.Db4o.Msg.OK.Write(stream, sock);
				}
			}
			catch (System.Exception e)
			{
				Db4objects.Db4o.Msg.ERROR.Write(stream, sock);
			}
			return true;
		}
	}
}
