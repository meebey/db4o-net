/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Tests.Common.Foundation.Network;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Foundation.Network
{
	/// <exclude></exclude>
	public class NetworkSocketTestCase : ITestLifeCycle
	{
		private ServerSocket4 _server;

		private int _port;

		internal ISocket4 client;

		internal ISocket4 serverSide;

		private PlainSocketFactory _plainSocketFactory = new PlainSocketFactory();

		public static void Main(string[] args)
		{
			new TestRunner(typeof(NetworkSocketTestCase)).Run();
		}

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			_server = new ServerSocket4(_plainSocketFactory, 0);
			_port = _server.GetLocalPort();
			client = new NetworkSocket(_plainSocketFactory, "localhost", _port);
			serverSide = _server.Accept();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
			_server.Close();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestRead_Close1()
		{
			AssertReadClose(client, new _ICodeBlock_44(this));
		}

		private sealed class _ICodeBlock_44 : ICodeBlock
		{
			public _ICodeBlock_44(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.serverSide.Read();
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestRead_Close2()
		{
			AssertReadClose(serverSide, new _ICodeBlock_52(this));
		}

		private sealed class _ICodeBlock_52 : ICodeBlock
		{
			public _ICodeBlock_52(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.client.Read();
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestReadBII_Close1()
		{
			AssertReadClose(client, new _ICodeBlock_60(this));
		}

		private sealed class _ICodeBlock_60 : ICodeBlock
		{
			public _ICodeBlock_60(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.serverSide.Read(new byte[10], 0, 10);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestReadBII_Close2()
		{
			AssertReadClose(serverSide, new _ICodeBlock_68(this));
		}

		private sealed class _ICodeBlock_68 : ICodeBlock
		{
			public _ICodeBlock_68(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.client.Read(new byte[10], 0, 10);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWriteB_Close1()
		{
			AssertWriteClose(client, new _ICodeBlock_77(this));
		}

		private sealed class _ICodeBlock_77 : ICodeBlock
		{
			public _ICodeBlock_77(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.serverSide.Write(new byte[10]);
				this._enclosing.serverSide.Write(new byte[10]);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWriteB_Close2()
		{
			AssertWriteClose(serverSide, new _ICodeBlock_86(this));
		}

		private sealed class _ICodeBlock_86 : ICodeBlock
		{
			public _ICodeBlock_86(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.client.Write(new byte[10]);
				this._enclosing.client.Write(new byte[10]);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWriteBII_Close1()
		{
			AssertWriteClose(client, new _ICodeBlock_95(this));
		}

		private sealed class _ICodeBlock_95 : ICodeBlock
		{
			public _ICodeBlock_95(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.serverSide.Write(new byte[10], 0, 10);
				this._enclosing.serverSide.Write(new byte[10], 0, 10);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWriteBII_Close2()
		{
			AssertWriteClose(serverSide, new _ICodeBlock_104(this));
		}

		private sealed class _ICodeBlock_104 : ICodeBlock
		{
			public _ICodeBlock_104(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.client.Write(new byte[10], 0, 10);
				this._enclosing.client.Write(new byte[10], 0, 10);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWriteI_Close1()
		{
			AssertWriteClose(client, new _ICodeBlock_113(this));
		}

		private sealed class _ICodeBlock_113 : ICodeBlock
		{
			public _ICodeBlock_113(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.serverSide.Write(unchecked((int)(0xff)));
				this._enclosing.serverSide.Write(unchecked((int)(0xff)));
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWriteI_Close2()
		{
			AssertWriteClose(serverSide, new _ICodeBlock_122(this));
		}

		private sealed class _ICodeBlock_122 : ICodeBlock
		{
			public _ICodeBlock_122(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.client.Write(unchecked((int)(0xff)));
				this._enclosing.client.Write(unchecked((int)(0xff)));
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		private void AssertReadClose(ISocket4 socketToBeClosed, ICodeBlock codeBlock)
		{
			new _Thread_132(this, socketToBeClosed).Start();
			Assert.Expect(typeof(Db4oIOException), codeBlock);
		}

		private sealed class _Thread_132 : Thread
		{
			public _Thread_132(NetworkSocketTestCase _enclosing, ISocket4 socketToBeClosed)
			{
				this._enclosing = _enclosing;
				this.socketToBeClosed = socketToBeClosed;
			}

			public override void Run()
			{
				Cool.SleepIgnoringInterruption(500);
				socketToBeClosed.Close();
			}

			private readonly NetworkSocketTestCase _enclosing;

			private readonly ISocket4 socketToBeClosed;
		}

		private void AssertWriteClose(ISocket4 socketToBeClosed, ICodeBlock codeBlock)
		{
			socketToBeClosed.Close();
			Assert.Expect(typeof(Db4oIOException), codeBlock);
		}
	}
}
