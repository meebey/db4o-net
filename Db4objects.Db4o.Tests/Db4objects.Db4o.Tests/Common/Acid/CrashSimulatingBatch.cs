/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using System.IO;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Tests.Common.Acid;
using Sharpen.IO;

namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class CrashSimulatingBatch
	{
		internal Collection4 writes = new Collection4();

		internal Collection4 currentWrite = new Collection4();

		public virtual void Add(byte[] bytes, long offset, int length)
		{
			currentWrite.Add(new CrashSimulatingWrite(bytes, offset, length));
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
					csw.Write(rightRaf);
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
					csw.Write(raf);
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
