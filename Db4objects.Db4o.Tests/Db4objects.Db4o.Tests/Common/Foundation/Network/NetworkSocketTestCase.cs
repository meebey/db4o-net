/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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
			new TestRunner(typeof(NetworkSocketTestCase)).Run();
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
			AssertReadClose(_client, new _ICodeBlock_44(this));
		}

		private sealed class _ICodeBlock_44 : ICodeBlock
		{
			public _ICodeBlock_44(NetworkSocketTestCase _enclosing)
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
			AssertReadClose(_server, new _ICodeBlock_52(this));
		}

		private sealed class _ICodeBlock_52 : ICodeBlock
		{
			public _ICodeBlock_52(NetworkSocketTestCase _enclosing)
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
			AssertReadClose(_client, new _ICodeBlock_60(this));
		}

		private sealed class _ICodeBlock_60 : ICodeBlock
		{
			public _ICodeBlock_60(NetworkSocketTestCase _enclosing)
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
			AssertReadClose(_server, new _ICodeBlock_68(this));
		}

		private sealed class _ICodeBlock_68 : ICodeBlock
		{
			public _ICodeBlock_68(NetworkSocketTestCase _enclosing)
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
			AssertWriteClose(_client, new _ICodeBlock_77(this));
		}

		private sealed class _ICodeBlock_77 : ICodeBlock
		{
			public _ICodeBlock_77(NetworkSocketTestCase _enclosing)
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
			AssertWriteClose(_server, new _ICodeBlock_85(this));
		}

		private sealed class _ICodeBlock_85 : ICodeBlock
		{
			public _ICodeBlock_85(NetworkSocketTestCase _enclosing)
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
			AssertWriteClose(_client, new _ICodeBlock_93(this));
		}

		private sealed class _ICodeBlock_93 : ICodeBlock
		{
			public _ICodeBlock_93(NetworkSocketTestCase _enclosing)
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
			AssertWriteClose(_server, new _ICodeBlock_101(this));
		}

		private sealed class _ICodeBlock_101 : ICodeBlock
		{
			public _ICodeBlock_101(NetworkSocketTestCase _enclosing)
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
			AssertWriteClose(_client, new _ICodeBlock_109(this));
		}

		private sealed class _ICodeBlock_109 : ICodeBlock
		{
			public _ICodeBlock_109(NetworkSocketTestCase _enclosing)
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
			AssertWriteClose(_server, new _ICodeBlock_117(this));
		}

		private sealed class _ICodeBlock_117 : ICodeBlock
		{
			public _ICodeBlock_117(NetworkSocketTestCase _enclosing)
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
			Assert.Expect(typeof(Db4oIOException), new _ICodeBlock_134(this, codeBlock));
		}

		private sealed class _ICodeBlock_134 : ICodeBlock
		{
			public _ICodeBlock_134(NetworkSocketTestCase _enclosing, ICodeBlock codeBlock)
			{
				this._enclosing = _enclosing;
				this.codeBlock = codeBlock;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				for (int i = 0; i < 20; i++)
				{
					codeBlock.Run();
				}
			}

			private readonly NetworkSocketTestCase _enclosing;

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
				_thread = new Thread(new _IRunnable_158(this));
				_codeBlock = codeBlock;
			}

			private sealed class _IRunnable_158 : IRunnable
			{
				public _IRunnable_158(CatchAllThread _enclosing)
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
