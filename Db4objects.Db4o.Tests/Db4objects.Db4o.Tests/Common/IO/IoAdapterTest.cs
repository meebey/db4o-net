/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class IoAdapterTest : ITestLifeCycle
	{
		public static void Main(string[] args)
		{
			new TestRunner(typeof(IoAdapterTest)).Run();
		}

		private string _cachedIoAdapterFile = Path.GetTempFileName();

		private string _randomAccessFileAdapterFile = Path.GetTempFileName();

		private IoAdapter[] _adapters;

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			DeleteAllTestFiles();
			InitAdapters(false);
		}

		/// <exception cref="Exception"></exception>
		private void InitAdapters(bool readOnly)
		{
			_adapters = new IoAdapter[] { InitRandomAccessAdapter(readOnly), InitCachedRandomAccessAdapter
				(readOnly) };
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
			CloseAdapters();
			DeleteAllTestFiles();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestReadWrite()
		{
			for (int i = 0; i < _adapters.Length; ++i)
			{
				AssertReadWrite(_adapters[i]);
			}
		}

		/// <exception cref="IOException"></exception>
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

		/// <exception cref="Exception"></exception>
		public virtual void TestSeek()
		{
			for (int i = 0; i < _adapters.Length; ++i)
			{
				AssertSeek(_adapters[i]);
			}
		}

		/// <exception cref="Exception"></exception>
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

		/// <exception cref="Exception"></exception>
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

		/// <exception cref="Exception"></exception>
		public virtual void TestReadOnly()
		{
			CloseAdapters();
			InitAdapters(true);
			for (int i = 0; i < _adapters.Length; i++)
			{
				AssertReadOnly(_adapters[i]);
			}
		}

		private void AssertReadOnly(IoAdapter adapter)
		{
			Assert.Expect(typeof(Db4oIOException), new _ICodeBlock_98(this, adapter));
		}

		private sealed class _ICodeBlock_98 : ICodeBlock
		{
			public _ICodeBlock_98(IoAdapterTest _enclosing, IoAdapter adapter)
			{
				this._enclosing = _enclosing;
				this.adapter = adapter;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				adapter.Write(new byte[] { 0 });
			}

			private readonly IoAdapterTest _enclosing;

			private readonly IoAdapter adapter;
		}

		/// <exception cref="Exception"></exception>
		public virtual void _testReadWriteAheadFileEnd()
		{
			string str = "this is a really long string, just to make sure that all IoAdapters work correctly. ";
			for (int i = 0; i < _adapters.Length; i++)
			{
				AssertReadWriteAheadFileEnd(_adapters[i], str);
			}
		}

		/// <exception cref="Exception"></exception>
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

		/// <exception cref="Exception"></exception>
		public virtual void TestReopen()
		{
			TestReadWrite();
			CloseAdapters();
			InitAdapters(false);
			TestReadWrite();
		}

		/// <exception cref="Exception"></exception>
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

		/// <exception cref="Exception"></exception>
		private IoAdapter InitCachedRandomAccessAdapter(bool readOnly)
		{
			IoAdapter adapter = new CachedIoAdapter(new RandomAccessFileAdapter());
			adapter = adapter.Open(_cachedIoAdapterFile, false, 0, readOnly);
			return adapter;
		}

		/// <exception cref="Exception"></exception>
		private IoAdapter InitRandomAccessAdapter(bool readOnly)
		{
			IoAdapter adapter = new RandomAccessFileAdapter();
			adapter = adapter.Open(_randomAccessFileAdapterFile, false, 0, readOnly);
			return adapter;
		}

		/// <exception cref="Exception"></exception>
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
				catch (Db4oIOException)
				{
				}
			}
		}
	}
}
