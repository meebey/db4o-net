/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.CS.Messages;
using Sharpen.IO;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public abstract class MsgBlob : MsgD, IBlobStatus
	{
		public BlobImpl _blob;

		internal int _currentByte;

		internal int _length;

		public virtual double GetStatus()
		{
			if (_length != 0)
			{
				return (double)_currentByte / (double)_length;
			}
			return Status.Error;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public abstract void ProcessClient(ISocket4 sock);

		internal virtual BlobImpl ServerGetBlobImpl()
		{
			BlobImpl blobImpl = null;
			int id = _payLoad.ReadInt();
			ObjectContainerBase stream = Stream();
			lock (stream.Lock())
			{
				blobImpl = (BlobImpl)stream.GetByID(Transaction(), id);
				stream.Activate(Transaction(), blobImpl, new FixedActivationDepth(3));
			}
			return blobImpl;
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected virtual void Copy(ISocket4 sock, IOutputStream rawout, int length, bool
			 update)
		{
			BufferedOutputStream @out = new BufferedOutputStream(rawout);
			byte[] buffer = new byte[BlobImpl.CopybufferLength];
			int totalread = 0;
			while (totalread < length)
			{
				int stilltoread = length - totalread;
				int readsize = (stilltoread < buffer.Length ? stilltoread : buffer.Length);
				int curread = sock.Read(buffer, 0, readsize);
				if (curread < 0)
				{
					throw new IOException();
				}
				@out.Write(buffer, 0, curread);
				totalread += curread;
				if (update)
				{
					_currentByte += curread;
				}
			}
			@out.Flush();
			@out.Close();
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected virtual void Copy(IInputStream rawin, ISocket4 sock, bool update)
		{
			BufferedInputStream @in = new BufferedInputStream(rawin);
			byte[] buffer = new byte[BlobImpl.CopybufferLength];
			int bytesread = -1;
			while ((bytesread = rawin.Read(buffer)) >= 0)
			{
				sock.Write(buffer, 0, bytesread);
				if (update)
				{
					_currentByte += bytesread;
				}
			}
			@in.Close();
		}
	}
}
