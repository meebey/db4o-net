/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using System.IO;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Internal.Transactionlog;
using Db4objects.Db4o.Tests.Common.Acid;
using Sharpen.IO;

namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class CrashSimulatingBatch
	{
		internal Collection4 writes = new Collection4();

		internal Collection4 currentWrite = new Collection4();

		public virtual void Add(string path, byte[] bytes, long offset, int length)
		{
			byte[] lockFileBuffer = null;
			byte[] logFileBuffer = null;
			if (System.IO.File.Exists(FileBasedTransactionLogHandler.LockFileName(path)))
			{
				try
				{
					lockFileBuffer = ReadAllBytes(FileBasedTransactionLogHandler.LockFileName(path));
					logFileBuffer = ReadAllBytes(FileBasedTransactionLogHandler.LogFileName(path));
				}
				catch (IOException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
			CrashSimulatingWrite crashSimulatingWrite = new CrashSimulatingWrite(bytes, offset
				, length, lockFileBuffer, logFileBuffer);
			currentWrite.Add(crashSimulatingWrite);
		}

		/// <exception cref="IOException"></exception>
		private byte[] ReadAllBytes(string fileName)
		{
			int length = (int)new Sharpen.IO.File(fileName).Length();
			RandomAccessFile raf = new RandomAccessFile(fileName, "rw");
			byte[] buffer = new byte[length];
			raf.Read(buffer);
			raf.Close();
			return buffer;
		}

		public virtual void Sync()
		{
			writes.Add(currentWrite);
			currentWrite = new Collection4();
		}

		public virtual int NumSyncs()
		{
			return writes.Size();
		}

		/// <exception cref="IOException"></exception>
		public virtual int WriteVersions(string file)
		{
			int count = 0;
			int rcount = 0;
			string lastFileName = file + "0";
			string rightFileName = file + "R";
			File4.Copy(lastFileName, rightFileName);
			IEnumerator syncIter = writes.GetEnumerator();
			while (syncIter.MoveNext())
			{
				rcount++;
				Collection4 writesBetweenSync = (Collection4)syncIter.Current;
				RandomAccessFile rightRaf = new RandomAccessFile(rightFileName, "rw");
				IEnumerator singleForwardIter = writesBetweenSync.GetEnumerator();
				while (singleForwardIter.MoveNext())
				{
					CrashSimulatingWrite csw = (CrashSimulatingWrite)singleForwardIter.Current;
					csw.Write(rightFileName, rightRaf);
				}
				rightRaf.Close();
				IEnumerator singleBackwardIter = writesBetweenSync.GetEnumerator();
				while (singleBackwardIter.MoveNext())
				{
					count++;
					CrashSimulatingWrite csw = (CrashSimulatingWrite)singleBackwardIter.Current;
					string currentFileName = file + "W" + count;
					File4.Copy(lastFileName, currentFileName);
					RandomAccessFile raf = new RandomAccessFile(currentFileName, "rw");
					csw.Write(currentFileName, raf);
					raf.Close();
					lastFileName = currentFileName;
				}
				File4.Copy(rightFileName, rightFileName + rcount);
				lastFileName = rightFileName;
			}
			return count;
		}
	}
}
