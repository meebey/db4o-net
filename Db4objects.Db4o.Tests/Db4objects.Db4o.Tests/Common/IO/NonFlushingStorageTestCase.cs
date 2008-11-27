/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Mocking;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class NonFlushingStorageTestCase : ITestCase
	{
		public virtual void Test()
		{
			MockBin mock = new MockBin();
			IBin storage = new NonFlushingStorage(new _IStorage_15(mock)).Open("uri", true, 42
				, false);
			byte[] buffer = new byte[5];
			storage.Read(1, buffer, 4);
			storage.Write(2, buffer, 3);
			mock.ReturnValueForNextCall(42);
			Assert.AreEqual(42, mock.Length());
			storage.Sync();
			storage.Close();
			mock.Verify(new MethodCall[] { new MethodCall("open", new object[] { "uri", true, 
				42L, false }), new MethodCall("read", new object[] { 1L, buffer, 4 }), new MethodCall
				("write", new object[] { 2L, buffer, 3 }), new MethodCall("length", new object[]
				 {  }), new MethodCall("close", new object[] {  }) });
		}

		private sealed class _IStorage_15 : IStorage
		{
			public _IStorage_15(MockBin mock)
			{
				this.mock = mock;
			}

			public bool Exists(string uri)
			{
				throw new NotImplementedException();
			}

			/// <exception cref="Db4oIOException"></exception>
			public IBin Open(string uri, bool lockFile, long initialLength, bool readOnly)
			{
				mock.Record(new MethodCall("open", new object[] { uri, lockFile, initialLength, readOnly
					 }));
				return mock;
			}

			private readonly MockBin mock;
		}
	}
}
