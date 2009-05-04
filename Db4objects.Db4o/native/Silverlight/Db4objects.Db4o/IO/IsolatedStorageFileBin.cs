/* Copyright (C) 2009  Versant Inc.  http://www.db4o.com */

#if SILVERLIGHT

using System.IO;
using System.IO.IsolatedStorage;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.IO
{
        class IsolatedStorageFileBin : IBin
		{
        	private readonly IsolatedStorageFileStream _fileStream;

            internal IsolatedStorageFileBin(BinConfiguration config, IsolatedStorageFile store)
			{
            	_fileStream = new IsolatedStorageFileStream(config.Uri(), FileMode.OpenOrCreate, store);
            }

            #region IBin Members

            public long Length()
            {
                return _fileStream.Length;
            }

            public int Read(long position, byte[] bytes, int bytesToRead)
            {
                try
                {
                    Seek(position);
                    return _fileStream.Read(bytes, 0, bytesToRead);
                }
                catch (IOException e)
                {
                    throw new Db4oIOException(e);
                }
            }

            public void Write(long position, byte[] bytes, int bytesToWrite)
            {
                try
                {
                    Seek(position);
                    _fileStream.Write(bytes, 0, bytesToWrite);
                }
                catch (IOException e)
                {
                    throw new Db4oIOException(e);
                }
            }

            public void Sync()
            {
                _fileStream.Flush();                
            }

            public int SyncRead(long position, byte[] bytes, int bytesToRead)
            {
                return Read(position, bytes, bytesToRead);
            }

            public void Close()
            {
                _fileStream.Close();
            }

            #endregion

            private void Seek(long position)
            {
                if (DTrace.enabled)
                {
                    DTrace.RegularSeek.Log(position);
                }
                _fileStream.Seek(position, SeekOrigin.Begin);
            }
        }
}

#endif
