namespace Db4objects.Db4o.Tests.Common.Acid
{
	public class CrashSimulatingBatch
	{
		internal Db4objects.Db4o.Foundation.Collection4 writes = new Db4objects.Db4o.Foundation.Collection4
			();

		internal Db4objects.Db4o.Foundation.Collection4 currentWrite = new Db4objects.Db4o.Foundation.Collection4
			();

		public virtual void Add(byte[] bytes, long offset, int length)
		{
			currentWrite.Add(new Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingWrite(bytes
				, offset, length));
		}

		public virtual void Sync()
		{
			writes.Add(currentWrite);
			currentWrite = new Db4objects.Db4o.Foundation.Collection4();
		}

		public virtual int NumSyncs()
		{
			return writes.Size();
		}

		public virtual int WriteVersions(string file)
		{
			int count = 0;
			int rcount = 0;
			string lastFileName = file + "0";
			string rightFileName = file + "R";
			Db4objects.Db4o.Foundation.IO.File4.Copy(lastFileName, rightFileName);
			System.Collections.IEnumerator syncIter = writes.GetEnumerator();
			while (syncIter.MoveNext())
			{
				rcount++;
				Db4objects.Db4o.Foundation.Collection4 writesBetweenSync = (Db4objects.Db4o.Foundation.Collection4
					)syncIter.Current;
				Sharpen.IO.RandomAccessFile rightRaf = new Sharpen.IO.RandomAccessFile(rightFileName
					, "rw");
				System.Collections.IEnumerator singleForwardIter = writesBetweenSync.GetEnumerator
					();
				while (singleForwardIter.MoveNext())
				{
					Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingWrite csw = (Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingWrite
						)singleForwardIter.Current;
					csw.Write(rightRaf);
				}
				rightRaf.Close();
				System.Collections.IEnumerator singleBackwardIter = writesBetweenSync.GetEnumerator
					();
				while (singleBackwardIter.MoveNext())
				{
					count++;
					Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingWrite csw = (Db4objects.Db4o.Tests.Common.Acid.CrashSimulatingWrite
						)singleBackwardIter.Current;
					string currentFileName = file + "W" + count;
					Db4objects.Db4o.Foundation.IO.File4.Copy(lastFileName, currentFileName);
					Sharpen.IO.RandomAccessFile raf = new Sharpen.IO.RandomAccessFile(currentFileName
						, "rw");
					csw.Write(raf);
					raf.Close();
					lastFileName = currentFileName;
				}
				Db4objects.Db4o.Foundation.IO.File4.Copy(rightFileName, rightFileName + rcount);
				lastFileName = rightFileName;
			}
			return count;
		}
	}
}
