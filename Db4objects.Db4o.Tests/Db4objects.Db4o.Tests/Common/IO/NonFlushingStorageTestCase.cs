/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Mocking;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class NonFlushingStorageTestCase : ITestCase
	{
		public virtual void Test()
		{
			MockBin mock = new MockBin();
			BinConfiguration binConfig = new BinConfiguration("uri", true, 42L, false);
			IBin storage = new NonFlushingStorage(new _IStorage_17(mock)).Open(binConfig);
			byte[] buffer = new byte[5];
			storage.Read(1, buffer, 4);
			storage.Write(2, buffer, 3);
			mock.ReturnValueForNextCall(42);
			Assert.AreEqual(42, mock.Length());
			storage.Sync();
			storage.Close();
			mock.Verify(new MethodCall[] { new MethodCall("open", new object[] { binConfig })
				, new MethodCall("read", new object[] { 1L, buffer, 4 }), new MethodCall("write"
				, new object[] { 2L, buffer, 3 }), new MethodCall("length", new object[] {  }), 
				new MethodCall("close", new object[] {  }) });
		}

		private sealed class _IStorage_17 : IStorage
		{
			public _IStorage_17(MockBin mock)
			{
				this.mock = mock;
			}

			public bool Exists(string uri)
			{
				throw new NotImplementedException();
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			public IBin Open(BinConfiguration config)
			{
				mock.Record(new MethodCall("open", new object[] { config }));
				return mock;
			}

			private readonly MockBin mock;
		}
	}
}
