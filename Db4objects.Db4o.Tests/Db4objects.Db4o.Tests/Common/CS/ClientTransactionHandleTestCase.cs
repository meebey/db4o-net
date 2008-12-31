/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ClientTransactionHandleTestCase : ITestLifeCycle
	{
		public virtual void TestHandles()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			config.Storage = new MemoryStorage();
			LocalObjectContainer db = (LocalObjectContainer)Db4oFactory.OpenFile(config, SwitchingFilesFromClientUtil
				.MainfileName);
			ClientTransactionPool pool = new ClientTransactionPool(db);
			try
			{
				ClientTransactionHandle handleA = new ClientTransactionHandle(pool);
				Assert.AreEqual(db, handleA.Transaction().Container());
				ClientTransactionHandle handleB = new ClientTransactionHandle(pool);
				Assert.AreNotEqual(handleA.Transaction(), handleB.Transaction());
				Assert.AreEqual(db, handleB.Transaction().Container());
				Assert.AreEqual(2, pool.OpenTransactionCount());
				Assert.AreEqual(1, pool.OpenFileCount());
				handleA.AcquireTransactionForFile(SwitchingFilesFromClientUtil.FilenameA);
				Assert.AreEqual(3, pool.OpenTransactionCount());
				Assert.AreEqual(2, pool.OpenFileCount());
				Assert.AreNotEqual(db, handleA.Transaction().Container());
				handleB.AcquireTransactionForFile(SwitchingFilesFromClientUtil.FilenameA);
				Assert.AreEqual(4, pool.OpenTransactionCount());
				Assert.AreEqual(2, pool.OpenFileCount());
				Assert.AreNotEqual(handleA.Transaction(), handleB.Transaction());
				Assert.AreEqual(handleA.Transaction().Container(), handleB.Transaction().Container
					());
				handleA.ReleaseTransaction();
				Assert.AreEqual(db, handleA.Transaction().Container());
				Assert.AreNotEqual(db, handleB.Transaction().Container());
				Assert.AreEqual(3, pool.OpenTransactionCount());
				Assert.AreEqual(2, pool.OpenFileCount());
				handleB.ReleaseTransaction();
				Assert.AreEqual(db, handleB.Transaction().Container());
				Assert.AreEqual(2, pool.OpenTransactionCount());
				Assert.AreEqual(1, pool.OpenFileCount());
				handleB.Close();
				Assert.AreEqual(1, pool.OpenTransactionCount());
				handleA.Close();
				Assert.AreEqual(0, pool.OpenTransactionCount());
			}
			finally
			{
				pool.Close();
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetUp()
		{
			SwitchingFilesFromClientUtil.DeleteFiles();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TearDown()
		{
			SwitchingFilesFromClientUtil.DeleteFiles();
		}
	}
}
