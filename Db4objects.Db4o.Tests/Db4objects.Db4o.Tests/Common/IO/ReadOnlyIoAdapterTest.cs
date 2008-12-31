/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class ReadOnlyIoAdapterTest : IoAdapterTestUnitBase
	{
		public virtual void Test()
		{
			ReopenAsReadOnly();
			AssertReadOnly(_adapter);
		}

		private void ReopenAsReadOnly()
		{
			Close();
			Open(true);
		}

		private void AssertReadOnly(IoAdapter adapter)
		{
			Assert.Expect(typeof(Db4oIOException), new _ICodeBlock_21(adapter));
		}

		private sealed class _ICodeBlock_21 : ICodeBlock
		{
			public _ICodeBlock_21(IoAdapter adapter)
			{
				this.adapter = adapter;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				adapter.Write(new byte[] { 0 });
			}

			private readonly IoAdapter adapter;
		}
	}
}
