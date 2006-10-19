namespace Db4objects.Db4o
{
	internal abstract class MsgBlob : Db4objects.Db4o.MsgD
	{
		internal Db4objects.Db4o.BlobImpl _blob;

		internal int _currentByte;

		internal int _length;

		internal virtual double GetStatus()
		{
			if (_length != 0)
			{
				return (double)_currentByte / (double)_length;
			}
			return Db4objects.Db4o.Ext.Status.ERROR;
		}

		internal abstract void ProcessClient(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock);

		internal virtual Db4objects.Db4o.BlobImpl ServerGetBlobImpl()
		{
			Db4objects.Db4o.BlobImpl blobImpl = null;
			int id = _payLoad.ReadInt();
			Db4objects.Db4o.YapStream stream = GetStream();
			lock (stream.i_lock)
			{
				blobImpl = (Db4objects.Db4o.BlobImpl)stream.GetByID1(GetTransaction(), id);
				stream.Activate1(GetTransaction(), blobImpl, 3);
			}
			return blobImpl;
		}

		protected virtual void Copy(Db4objects.Db4o.Foundation.Network.IYapSocket sock, Sharpen.IO.OutputStream
			 rawout, int length, bool update)
		{
			Sharpen.IO.BufferedOutputStream @out = new Sharpen.IO.BufferedOutputStream(rawout
				);
			byte[] buffer = new byte[Db4objects.Db4o.BlobImpl.COPYBUFFER_LENGTH];
			int totalread = 0;
			while (totalread < length)
			{
				int stilltoread = length - totalread;
				int readsize = (stilltoread < buffer.Length ? stilltoread : buffer.Length);
				int curread = sock.Read(buffer, 0, readsize);
				if (curread < 0)
				{
					throw new System.IO.IOException();
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

		protected virtual void Copy(Sharpen.IO.InputStream rawin, Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock, bool update)
		{
			Sharpen.IO.BufferedInputStream @in = new Sharpen.IO.BufferedInputStream(rawin);
			byte[] buffer = new byte[Db4objects.Db4o.BlobImpl.COPYBUFFER_LENGTH];
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
