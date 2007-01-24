namespace Db4objects.Db4o.Tests.Common.IO
{
	public class IoAdapterTest : Db4oUnit.ITestCase, Db4oUnit.ITestLifeCycle
	{
		private string _cachedIoAdapterFile = "CachedIoAdapter.dat";

		private string _randomAccessFileAdapterFile = "_randomAccessFileAdapter.dat";

		private Db4objects.Db4o.IO.IoAdapter[] _adapters;

		public virtual void SetUp()
		{
			DeleteAllTestFiles();
			_adapters = new Db4objects.Db4o.IO.IoAdapter[] { InitCachedRandomAccessAdapter(), 
				InitRandomAccessAdapter() };
		}

		public virtual void TearDown()
		{
			CloseAllAdapters();
			DeleteAllTestFiles();
		}

		public virtual void TestReadWrite()
		{
			for (int i = 0; i < _adapters.Length; ++i)
			{
				AssertReadWrite(_adapters[i]);
			}
		}

		private void AssertReadWrite(Db4objects.Db4o.IO.IoAdapter adapter)
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
				Db4oUnit.Assert.AreEqual(data[i], readBytes[i]);
			}
		}

		public virtual void TestSeek()
		{
			for (int i = 0; i < _adapters.Length; ++i)
			{
				AssertSeek(_adapters[i]);
			}
		}

		private void AssertSeek(Db4objects.Db4o.IO.IoAdapter adapter)
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
				Db4oUnit.Assert.AreEqual(data[i], readBytes[i]);
			}
			adapter.Seek(20);
			adapter.Read(readBytes);
			for (int i = 0; i < count - 20; i++)
			{
				Db4oUnit.Assert.AreEqual(data[i + 20], readBytes[i]);
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
			Db4oUnit.Assert.AreEqual(10, readCount);
			for (int i = 0; i < readCount; ++i)
			{
				Db4oUnit.Assert.AreEqual(i, readBytes[i]);
			}
		}

		private Db4objects.Db4o.IO.IoAdapter InitCachedRandomAccessAdapter()
		{
			Db4objects.Db4o.IO.IoAdapter adapter = new Db4objects.Db4o.IO.CachedIoAdapter(new 
				Db4objects.Db4o.IO.RandomAccessFileAdapter());
			adapter = adapter.Open(_cachedIoAdapterFile, false, 0);
			return adapter;
		}

		private Db4objects.Db4o.IO.IoAdapter InitRandomAccessAdapter()
		{
			Db4objects.Db4o.IO.IoAdapter adapter = new Db4objects.Db4o.IO.RandomAccessFileAdapter
				();
			adapter = adapter.Open(_randomAccessFileAdapterFile, false, 0);
			return adapter;
		}

		private void DeleteAllTestFiles()
		{
			new Sharpen.IO.File(_cachedIoAdapterFile).Delete();
			new Sharpen.IO.File(_randomAccessFileAdapterFile).Delete();
		}

		private void CloseAllAdapters()
		{
			for (int i = 0; i < _adapters.Length; ++i)
			{
				try
				{
					_adapters[i].Close();
				}
				catch (System.IO.IOException)
				{
				}
			}
		}
	}
}
