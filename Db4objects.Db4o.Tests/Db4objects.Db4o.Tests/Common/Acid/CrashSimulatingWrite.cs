/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal.Transactionlog;
using Sharpen.IO;

namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class CrashSimulatingWrite
	{
		internal byte[] _data;

		internal long _offset;

		internal int _length;

		internal byte[] _lockFileData;

		internal byte[] _logFileData;

		public CrashSimulatingWrite(byte[] data, long offset, int length, byte[] lockFileData
			, byte[] logFileData)
		{
			_data = data;
			_offset = offset;
			_length = length;
			_lockFileData = lockFileData;
			_logFileData = logFileData;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Write(string path, RandomAccessFile raf)
		{
			raf.Seek(_offset);
			raf.Write(_data, 0, _length);
			Write(FileBasedTransactionLogHandler.LockFileName(path), _lockFileData);
			Write(FileBasedTransactionLogHandler.LogFileName(path), _logFileData);
		}

		public override string ToString()
		{
			return "A " + _offset + " L " + _length;
		}

		private void Write(string fileName, byte[] bytes)
		{
			if (bytes == null)
			{
				return;
			}
			try
			{
				RandomAccessFile raf = new RandomAccessFile(fileName, "rw");
				raf.Write(bytes);
				raf.Close();
			}
			catch (IOException e)
			{
				throw new Db4oException(e);
			}
		}
	}
}
