/* Copyright (C) 2004 - 2007  db4objects Inc.   http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o.Ext;

namespace Sharpen.IO
{
    public class RandomAccessFile
    {
        private FileStream _stream;

#if !CF && !MONO
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        static extern int FlushFileBuffers(IntPtr fileHandle);
#endif
        public RandomAccessFile(String file, bool readOnly, bool lockFile)
        {
            _stream = new FileStream(file, FileMode.OpenOrCreate,
                readOnly ? FileAccess.Read : FileAccess.ReadWrite,
                lockFile ? FileShare.None : FileShare.ReadWrite
            );
        }

        public RandomAccessFile(String file, String fileMode)
            : this(file, fileMode.Equals("r"), true)
        {
        }

        public FileStream Stream
        {
            get { return _stream; }
        }

        public void Close()
        {
            _stream.Close();
        }

        public long Length()
        {
            return _stream.Length;
        }

        public int Read(byte[] bytes, int offset, int length)
        {
            return _stream.Read(bytes, offset, length);
        }

        public void Read(byte[] bytes)
        {
            _stream.Read(bytes, 0, bytes.Length);
        }

        public void Seek(long pos)
        {
            _stream.Seek(pos, SeekOrigin.Begin);
        }

        public void Sync()
        {
            _stream.Flush();

#if !CF && !MONO
            FlushFileBuffers(_stream.SafeFileHandle.DangerousGetHandle());
#endif
        }

        public RandomAccessFile GetFD()
        {
            return this;
        }

        public void Write(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }

        public void Write(byte[] bytes, int offset, int length)
        {
            try
            {
                _stream.Write(bytes, offset, length);
            }
            catch (System.NotSupportedException e)
            {
                throw new IOException("Not supported", e);
            }
        }
    }
}
