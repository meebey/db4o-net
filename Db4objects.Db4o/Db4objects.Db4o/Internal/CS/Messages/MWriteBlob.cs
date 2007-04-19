using System;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.IO;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MWriteBlob : MsgBlob, IServerSideMessage
	{
		public override void ProcessClient(ISocket4 sock)
		{
			Msg message = Msg.ReadMessage(MessageDispatcher(), Transaction(), sock);
			if (message.Equals(Msg.OK))
			{
				try
				{
					_currentByte = 0;
					_length = this._blob.GetLength();
					_blob.GetStatusFrom(this);
					_blob.SetStatus(Status.PROCESSING);
					FileInputStream inBlob = this._blob.GetClientInputStream();
					Copy(inBlob, sock, true);
					sock.Flush();
					ObjectContainerBase stream = Stream();
					message = Msg.ReadMessage(MessageDispatcher(), Transaction(), sock);
					if (message.Equals(Msg.OK))
					{
						stream.Deactivate(_blob, int.MaxValue);
						stream.Activate(_blob, int.MaxValue);
						this._blob.SetStatus(Status.COMPLETED);
					}
					else
					{
						this._blob.SetStatus(Status.ERROR);
					}
				}
				catch (Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
		}

		public virtual bool ProcessAtServer()
		{
			try
			{
				ObjectContainerBase stream = Stream();
				BlobImpl blobImpl = this.ServerGetBlobImpl();
				if (blobImpl != null)
				{
					blobImpl.SetTrans(Transaction());
					Sharpen.IO.File file = blobImpl.ServerFile(null, true);
					ISocket4 sock = ServerMessageDispatcher().Socket();
					Msg.OK.Write(stream, sock);
					FileOutputStream fout = new FileOutputStream(file);
					Copy(sock, fout, blobImpl.GetLength(), false);
					Msg.OK.Write(stream, sock);
				}
			}
			catch (Exception)
			{
			}
			return true;
		}
	}
}
