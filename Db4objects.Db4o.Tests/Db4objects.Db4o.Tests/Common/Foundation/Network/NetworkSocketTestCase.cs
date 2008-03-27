/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Tests.Common.Foundation.Network;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Foundation.Network
{
	/// <exclude></exclude>
	public class NetworkSocketTestCase : ITestLifeCycle
	{
		private ServerSocket4 _serverSocket;

		private int _port;

		internal ISocket4 _client;

		internal ISocket4 _server;

		private PlainSocketFactory _plainSocketFactory = new PlainSocketFactory();

		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(NetworkSocketTestCase)).Run();
		}

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			_serverSocket = new ServerSocket4(_plainSocketFactory, 0);
			_port = _serverSocket.GetLocalPort();
			_client = new NetworkSocket(_plainSocketFactory, "localhost", _port);
			_server = _serverSocket.Accept();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
			_serverSocket.Close();
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestReadIntCloseClient()
		{
			AssertReadClose(_client, new _ICodeBlock_43(this));
		}

		private sealed class _ICodeBlock_43 : ICodeBlock
		{
			public _ICodeBlock_43(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._server.Read();
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestReadIntCloseServer()
		{
			AssertReadClose(_server, new _ICodeBlock_51(this));
		}

		private sealed class _ICodeBlock_51 : ICodeBlock
		{
			public _ICodeBlock_51(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._client.Read();
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestReadByteArrayCloseClient()
		{
			AssertReadClose(_client, new _ICodeBlock_59(this));
		}

		private sealed class _ICodeBlock_59 : ICodeBlock
		{
			public _ICodeBlock_59(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._server.Read(new byte[10], 0, 10);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestReadByteArrayCloseServer()
		{
			AssertReadClose(_server, new _ICodeBlock_67(this));
		}

		private sealed class _ICodeBlock_67 : ICodeBlock
		{
			public _ICodeBlock_67(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._client.Read(new byte[10], 0, 10);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWriteByteArrayCloseClient()
		{
			AssertWriteClose(_client, new _ICodeBlock_76(this));
		}

		private sealed class _ICodeBlock_76 : ICodeBlock
		{
			public _ICodeBlock_76(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._server.Write(new byte[10]);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWriteByteArrayCloseServer()
		{
			AssertWriteClose(_server, new _ICodeBlock_84(this));
		}

		private sealed class _ICodeBlock_84 : ICodeBlock
		{
			public _ICodeBlock_84(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._client.Write(new byte[10]);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWriteByteArrayPartCloseClient()
		{
			AssertWriteClose(_client, new _ICodeBlock_92(this));
		}

		private sealed class _ICodeBlock_92 : ICodeBlock
		{
			public _ICodeBlock_92(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._server.Write(new byte[10], 0, 10);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWriteByteArrayPartCloseServer()
		{
			AssertWriteClose(_server, new _ICodeBlock_100(this));
		}

		private sealed class _ICodeBlock_100 : ICodeBlock
		{
			public _ICodeBlock_100(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._client.Write(new byte[10], 0, 10);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWriteIntCloseClient()
		{
			AssertWriteClose(_client, new _ICodeBlock_108(this));
		}

		private sealed class _ICodeBlock_108 : ICodeBlock
		{
			public _ICodeBlock_108(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._server.Write(unchecked((int)(0xff)));
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestWriteIntCloseServer()
		{
			AssertWriteClose(_server, new _ICodeBlock_116(this));
		}

		private sealed class _ICodeBlock_116 : ICodeBlock
		{
			public _ICodeBlock_116(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._client.Write(unchecked((int)(0xff)));
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="Exception"></exception>
		private void AssertReadClose(ISocket4 socketToBeClosed, ICodeBlock codeBlock)
		{
			NetworkSocketTestCase.CatchAllThread thread = new NetworkSocketTestCase.CatchAllThread
				(codeBlock);
			thread.EnsureStarted();
			socketToBeClosed.Close();
			thread.Join();
			Assert.IsInstanceOf(typeof(Db4oIOException), thread.Caught());
		}

		private void AssertWriteClose(ISocket4 socketToBeClosed, ICodeBlock codeBlock)
		{
			socketToBeClosed.Close();
			Assert.Expect(typeof(Db4oIOException), new _ICodeBlock_133(codeBlock));
		}

		private sealed class _ICodeBlock_133 : ICodeBlock
		{
			public _ICodeBlock_133(ICodeBlock codeBlock)
			{
				this.codeBlock = codeBlock;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				// This is a magic number: 
				// On my machine all tests start to pass when I write at least 7 times.
				// Trying with 20 on the build machine.
				for (int i = 0; i < 20; i++)
				{
					codeBlock.Run();
				}
			}

			private readonly ICodeBlock codeBlock;
		}

		internal class CatchAllThread
		{
			private readonly Thread _thread;

			internal bool _isRunning;

			internal readonly ICodeBlock _codeBlock;

			internal Exception _throwable;

			public CatchAllThread(ICodeBlock codeBlock)
			{
				_thread = new Thread(new _IRunnable_157(this));
				_codeBlock = codeBlock;
			}

			private sealed class _IRunnable_157 : IRunnable
			{
				public _IRunnable_157(CatchAllThread _enclosing)
				{
					this._enclosing = _enclosing;
				}

				public void Run()
				{
					try
					{
						lock (this)
						{
							this._enclosing._isRunning = true;
						}
						this._enclosing._codeBlock.Run();
					}
					catch (Exception t)
					{
						this._enclosing._throwable = t;
					}
				}

				private readonly CatchAllThread _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public virtual void Join()
			{
				_thread.Join();
			}

			private bool IsRunning()
			{
				lock (this)
				{
					return _isRunning;
				}
			}

			public virtual void EnsureStarted()
			{
				_thread.Start();
				while (!IsRunning())
				{
					Cool.SleepIgnoringInterruption(10);
				}
				Cool.SleepIgnoringInterruption(10);
			}

			public virtual Exception Caught()
			{
				return _throwable;
			}
		}
	}
}
