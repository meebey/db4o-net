namespace Db4objects.Db4o
{
	internal class MWriteBlob : Db4objects.Db4o.MsgBlob
	{
		internal override void ProcessClient(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.Msg message = Db4objects.Db4o.Msg.ReadMessage(GetTransaction(), sock
				);
			if (message.Equals(Db4objects.Db4o.Msg.OK))
			{
				try
				{
					_currentByte = 0;
					_length = this._blob.GetLength();
					_blob.GetStatusFrom(this);
					_blob.SetStatus(Db4objects.Db4o.Ext.Status.PROCESSING);
					Sharpen.IO.FileInputStream inBlob = this._blob.GetClientInputStream();
					Copy(inBlob, sock, true);
					sock.Flush();
					Db4objects.Db4o.YapStream stream = GetStream();
					message = Db4objects.Db4o.Msg.ReadMessage(GetTransaction(), sock);
					if (message.Equals(Db4objects.Db4o.Msg.OK))
					{
						stream.Deactivate(_blob, int.MaxValue);
						stream.Activate(_blob, int.MaxValue);
						this._blob.SetStatus(Db4objects.Db4o.Ext.Status.COMPLETED);
					}
					else
					{
						this._blob.SetStatus(Db4objects.Db4o.Ext.Status.ERROR);
					}
				}
				catch (System.Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
		}

		internal override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			try
			{
				Db4objects.Db4o.YapStream stream = GetStream();
				Db4objects.Db4o.BlobImpl blobImpl = this.ServerGetBlobImpl();
				if (blobImpl != null)
				{
					blobImpl.SetTrans(GetTransaction());
					Sharpen.IO.File file = blobImpl.ServerFile(null, true);
					Db4objects.Db4o.Msg.OK.Write(stream, sock);
					Sharpen.IO.FileOutputStream fout = new Sharpen.IO.FileOutputStream(file);
					Copy(sock, fout, blobImpl.GetLength(), false);
					Db4objects.Db4o.Msg.OK.Write(stream, sock);
				}
			}
			catch (System.Exception e)
			{
			}
			return true;
		}
	}
}
