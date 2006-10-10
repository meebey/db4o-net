/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.IO;

namespace Sharpen.IO {

    public class RandomAccessFile {

        private FileStream fileStream;

#if NET || NET_2_0
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError=true)] 
        static extern int FlushFileBuffers(IntPtr fileHandle); 
#endif

        public RandomAccessFile(String file, String fileMode) {
            fileStream = new FileStream(file, FileMode.OpenOrCreate,
                fileMode.Equals("rw") ? FileAccess.ReadWrite : FileAccess.Read);
            LockFileStream(this.fileStream);
        }

    	private void LockFileStream(FileStream stream)
    	{
#if !CF_1_0 && !CF_2_0
			stream.Lock(0, 1);
#endif
    	}

    	public void Close() {
            fileStream.Close();
        }

        public long Length() {
            return fileStream.Length;
        }

        public int Read(byte[] bytes, int offset, int length) {
            return fileStream.Read(bytes, offset, length);
        }

        public void Read(byte[] bytes) {
            fileStream.Read(bytes, 0, bytes.Length);
        }

        public void Seek(long pos) {
            fileStream.Seek(pos, SeekOrigin.Begin);
        }

        public void Sync() {
            fileStream.Flush();

#if NET || NET_2_0
			FlushFileBuffers(fileStream.Handle);
#endif

        }
        
        public RandomAccessFile GetFD() {
        	return this;
        }

        public void Write(byte[] bytes) {
            this.Write(bytes, 0, bytes.Length);
        }

        public void Write(byte[] bytes, int offset, int length) {
            fileStream.Write(bytes, offset, length);
        }
    }
}
