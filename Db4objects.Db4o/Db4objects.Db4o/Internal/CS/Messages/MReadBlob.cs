/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.IO;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MReadBlob : MsgBlob, IServerSideMessage
	{
		public override void ProcessClient(ISocket4 sock)
		{
			Msg message = Msg.ReadMessage(MessageDispatcher(), Transaction(), sock);
			if (message.Equals(Msg.LENGTH))
			{
				try
				{
					_currentByte = 0;
					_length = message.PayLoad().ReadInt();
					_blob.GetStatusFrom(this);
					_blob.SetStatus(Status.PROCESSING);
					Copy(sock, this._blob.GetClientOutputStream(), _length, true);
					message = Msg.ReadMessage(MessageDispatcher(), Transaction(), sock);
					if (message.Equals(Msg.OK))
					{
						this._blob.SetStatus(Status.COMPLETED);
					}
					else
					{
						this._blob.SetStatus(Status.ERROR);
					}
				}
				catch (Exception)
				{
				}
			}
			else
			{
				if (message.Equals(Msg.ERROR))
				{
					this._blob.SetStatus(Status.ERROR);
				}
			}
		}

		public virtual bool ProcessAtServer()
		{
			try
			{
				BlobImpl blobImpl = this.ServerGetBlobImpl();
				if (blobImpl != null)
				{
					blobImpl.SetTrans(Transaction());
					Sharpen.IO.File file = blobImpl.ServerFile(null, false);
					int length = (int)file.Length();
					ISocket4 sock = ServerMessageDispatcher().Socket();
					Msg.LENGTH.GetWriterForInt(Transaction(), length).Write(sock);
					FileInputStream fin = new FileInputStream(file);
					Copy(fin, sock, false);
					sock.Flush();
					Msg.OK.Write(sock);
				}
			}
			catch (Exception)
			{
				Write(Msg.ERROR);
			}
			return true;
		}
	}
}
