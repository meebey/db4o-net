using System;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class IoAdapterTest : ITestCase, ITestLifeCycle
	{
		private string _cachedIoAdapterFile = "CachedIoAdapter.dat";

		private string _randomAccessFileAdapterFile = "_randomAccessFileAdapter.dat";

		private IoAdapter[] _adapters;

		public virtual void SetUp()
		{
			DeleteAllTestFiles();
			InitAdapters();
		}

		private void InitAdapters()
		{
			_adapters = new IoAdapter[] { InitRandomAccessAdapter(), InitCachedRandomAccessAdapter
				() };
		}

		public virtual void TearDown()
		{
			CloseAdapters();
			DeleteAllTestFiles();
		}

		public virtual void TestReadWrite()
		{
			for (int i = 0; i < _adapters.Length; ++i)
			{
				AssertReadWrite(_adapters[i]);
			}
		}

		private void AssertReadWrite(IoAdapter adapter)
		{
			adapter.Seek(0);
			int count = 1024 * 8 + 10;
			byte[] data = new byte[count];
			for (int i = 0; i < count; ++i)
			{
				data[i] = (byte)(i % 256);
			}
			adapter.Write(data);
			adapter.Seek(0);
			byte[] readBytes = new byte[count];
			adapter.Read(readBytes);
			for (int i = 0; i < count; i++)
			{
				Assert.AreEqual(data[i], readBytes[i]);
			}
		}

		public virtual void TestSeek()
		{
			for (int i = 0; i < _adapters.Length; ++i)
			{
				AssertSeek(_adapters[i]);
			}
		}

		public virtual void TestReadWriteBytes()
		{
			string[] strs = new string[] { "short string", "this is a really long string, just to make sure that all IoAdapters work correctly. "
				 };
			for (int i = 0; i < _adapters.Length; i++)
			{
				for (int j = 0; j < strs.Length; j++)
				{
					AssertReadWriteString(_adapters[i], strs[j]);
				}
			}
		}

		private void AssertReadWriteString(IoAdapter adapter, string str)
		{
			byte[] data = Sharpen.Runtime.GetBytesForString(str);
			byte[] read = new byte[2048];
			adapter.Seek(0);
			adapter.Write(data);
			adapter.Seek(0);
			adapter.Read(read);
			Assert.AreEqual(str, Sharpen.Runtime.GetStringForBytes(read, 0, data.Length));
		}

		public virtual void TestReadWriteAheadFileEnd()
		{
			string str = "this is a really long string, just to make sure that all IoAdapters work correctly. ";
			for (int i = 0; i < _adapters.Length; i++)
			{
				AssertReadWriteAheadFileEnd(_adapters[i], str);
			}
		}

		private void AssertReadWriteAheadFileEnd(IoAdapter adapter, string str)
		{
			byte[] data = Sharpen.Runtime.GetBytesForString(str);
			byte[] read = new byte[2048];
			adapter.Seek(10);
			int readBytes = adapter.Read(data);
			Assert.AreEqual(-1, readBytes);
			Assert.AreEqual(0, adapter.GetLength());
			adapter.Seek(0);
			readBytes = adapter.Read(data);
			Assert.AreEqual(-1, readBytes);
			Assert.AreEqual(0, adapter.GetLength());
			adapter.Seek(10);
			adapter.Write(data);
			Assert.AreEqual(10 + data.Length, adapter.GetLength());
			adapter.Seek(0);
			readBytes = adapter.Read(read);
			Assert.AreEqual(10 + data.Length, readBytes);
			adapter.Seek(20 + data.Length);
			readBytes = adapter.Read(read);
			Assert.AreEqual(-1, readBytes);
			adapter.Seek(1024 + data.Length);
			readBytes = adapter.Read(read);
			Assert.AreEqual(-1, readBytes);
			adapter.Seek(1200);
			adapter.Write(data);
			adapter.Seek(0);
			readBytes = adapter.Read(read);
			Assert.AreEqual(1200 + data.Length, readBytes);
		}

		public virtual void TestReadWriteTooMuch()
		{
			for (int i = 0; i < _adapters.Length; i++)
			{
				AssertReadWriteTooMuch(_adapters[i]);
			}
		}

		private void AssertReadWriteTooMuch(IoAdapter adapter)
		{
			byte[] data = Sharpen.Runtime.GetBytesForString("hello");
			byte[] buffer = new byte[1];
			adapter.Seek(0);
			adapter.Write(data);
			adapter.Seek(0);
			Assert.Expect(typeof(Exception), new _AnonymousInnerClass143(this, adapter, buffer
				));
			Assert.Expect(typeof(Exception), new _AnonymousInnerClass148(this, adapter, buffer
				));
		}

		private sealed class _AnonymousInnerClass143 : ICodeBlock
		{
			public _AnonymousInnerClass143(IoAdapterTest _enclosing, IoAdapter adapter, byte[]
				 buffer)
			{
				this._enclosing = _enclosing;
				this.adapter = adapter;
				this.buffer = buffer;
			}

			public void Run()
			{
				adapter.Read(buffer, buffer.Length + 1);
			}

			private readonly IoAdapterTest _enclosing;

			private readonly IoAdapter adapter;

			private readonly byte[] buffer;
		}

		private sealed class _AnonymousInnerClass148 : ICodeBlock
		{
			public _AnonymousInnerClass148(IoAdapterTest _enclosing, IoAdapter adapter, byte[]
				 buffer)
			{
				this._enclosing = _enclosing;
				this.adapter = adapter;
				this.buffer = buffer;
			}

			public void Run()
			{
				adapter.Write(buffer, buffer.Length + 1);
			}

			private readonly IoAdapterTest _enclosing;

			private readonly IoAdapter adapter;

			private readonly byte[] buffer;
		}

		public virtual void TestReopen()
		{
			TestReadWrite();
			CloseAdapters();
			InitAdapters();
			TestReadWrite();
		}

		private void AssertSeek(IoAdapter adapter)
		{
			int count = 1024 * 2 + 10;
			byte[] data = new byte[count];
			for (int i = 0; i < data.Length; ++i)
			{
				data[i] = (byte)(i % 256);
			}
			adapter.Write(data);
			byte[] readBytes = new byte[count];
			adapter.Seek(0);
			adapter.Read(readBytes);
			for (int i = 0; i < count; i++)
			{
				Assert.AreEqual(data[i], readBytes[i]);
			}
			adapter.Seek(20);
			adapter.Read(readBytes);
			for (int i = 0; i < count - 20; i++)
			{
				Assert.AreEqual(data[i + 20], readBytes[i]);
			}
			byte[] writtenData = new byte[10];
			for (int i = 0; i < writtenData.Length; ++i)
			{
				writtenData[i] = (byte)i;
			}
			adapter.Seek(1000);
			adapter.Write(writtenData);
			adapter.Seek(1000);
			int readCount = adapter.Read(readBytes, 10);
			Assert.AreEqual(10, readCount);
			for (int i = 0; i < readCount; ++i)
			{
				Assert.AreEqual(i, readBytes[i]);
			}
		}

		private IoAdapter InitCachedRandomAccessAdapter()
		{
			IoAdapter adapter = new CachedIoAdapter(new RandomAccessFileAdapter());
			adapter = adapter.Open(_cachedIoAdapterFile, false, 0);
			return adapter;
		}

		private IoAdapter InitRandomAccessAdapter()
		{
			IoAdapter adapter = new RandomAccessFileAdapter();
			adapter = adapter.Open(_randomAccessFileAdapterFile, false, 0);
			return adapter;
		}

		private void DeleteAllTestFiles()
		{
			new Sharpen.IO.File(_cachedIoAdapterFile).Delete();
			new Sharpen.IO.File(_randomAccessFileAdapterFile).Delete();
		}

		private void CloseAdapters()
		{
			for (int i = 0; i < _adapters.Length; ++i)
			{
				try
				{
					_adapters[i].Close();
				}
				catch (IOException)
				{
				}
			}
		}
	}
}
