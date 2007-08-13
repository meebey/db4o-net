/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ClientTransactionPoolTestCase : ITestLifeCycle
	{
		public virtual void TestPool()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			config.Io(new MemoryIoAdapter());
			LocalObjectContainer db = (LocalObjectContainer)Db4oFactory.OpenFile(config, SwitchingFilesFromClientUtil
				.MAINFILE_NAME);
			ClientTransactionPool pool = new ClientTransactionPool(db);
			try
			{
				Assert.AreEqual(1, pool.OpenFileCount());
				Transaction trans1 = pool.Acquire(SwitchingFilesFromClientUtil.MAINFILE_NAME);
				Assert.AreEqual(db, trans1.Container());
				Assert.AreEqual(1, pool.OpenFileCount());
				Transaction trans2 = pool.Acquire(SwitchingFilesFromClientUtil.FILENAME_A);
				Assert.AreNotEqual(db, trans2.Container());
				Assert.AreEqual(2, pool.OpenFileCount());
				Transaction trans3 = pool.Acquire(SwitchingFilesFromClientUtil.FILENAME_A);
				Assert.AreEqual(trans2.Container(), trans3.Container());
				Assert.AreEqual(2, pool.OpenFileCount());
				pool.Release(trans3, true);
				Assert.AreEqual(2, pool.OpenFileCount());
				pool.Release(trans2, true);
				Assert.AreEqual(1, pool.OpenFileCount());
				pool.Release(trans1, true);
				Assert.AreEqual(1, pool.OpenFileCount());
			}
			finally
			{
				Assert.IsFalse(db.IsClosed());
				Assert.IsFalse(pool.IsClosed());
				pool.Close();
				Assert.IsTrue(db.IsClosed());
				Assert.IsTrue(pool.IsClosed());
				Assert.AreEqual(0, pool.OpenFileCount());
			}
		}

		public virtual void SetUp()
		{
			SwitchingFilesFromClientUtil.DeleteFiles();
		}

		public virtual void TearDown()
		{
			SwitchingFilesFromClientUtil.DeleteFiles();
		}
	}
}
