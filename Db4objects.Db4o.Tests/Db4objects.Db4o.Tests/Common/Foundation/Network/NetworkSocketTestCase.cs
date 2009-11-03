/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

#if !SILVERLIGHT
using System;
using Db4oUnit;
using Db4objects.Db4o.CS.Internal;
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
		private IServerSocket4 _serverSocket;

		private int _port;

		internal Socket4Adapter _client;

		internal Socket4Adapter _server;

		private ISocket4Factory _plainSocketFactory = new StandardSocket4Factory();

		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(NetworkSocketTestCase)).Run();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void SetUp()
		{
			_serverSocket = _plainSocketFactory.CreateServerSocket(0);
			_port = _serverSocket.GetLocalPort();
			_client = new Socket4Adapter(_plainSocketFactory.CreateSocket("localhost", _port)
				);
			_server = new Socket4Adapter(_serverSocket.Accept());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TearDown()
		{
			_serverSocket.Close();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestReadByteArrayCloseClient()
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
				this._enclosing._server.Read(new byte[10], 0, 10);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestReadByteArrayCloseServer()
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
				this._enclosing._client.Read(new byte[10], 0, 10);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWriteByteArrayCloseClient()
		{
			AssertWriteClose(_client, new _ICodeBlock_60(this));
		}

		private sealed class _ICodeBlock_60 : ICodeBlock
		{
			public _ICodeBlock_60(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._server.Write(new byte[10]);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWriteByteArrayCloseServer()
		{
			AssertWriteClose(_server, new _ICodeBlock_68(this));
		}

		private sealed class _ICodeBlock_68 : ICodeBlock
		{
			public _ICodeBlock_68(NetworkSocketTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._client.Write(new byte[10]);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWriteByteArrayPartCloseClient()
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
				this._enclosing._server.Write(new byte[10], 0, 10);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestWriteByteArrayPartCloseServer()
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
				this._enclosing._client.Write(new byte[10], 0, 10);
			}

			private readonly NetworkSocketTestCase _enclosing;
		}

		/// <exception cref="System.Exception"></exception>
		private void AssertReadClose(Socket4Adapter socketToBeClosed, ICodeBlock codeBlock
			)
		{
			NetworkSocketTestCase.CatchAllThread thread = new NetworkSocketTestCase.CatchAllThread
				(codeBlock);
			thread.EnsureStarted();
			socketToBeClosed.Close();
			thread.Join();
			Assert.IsInstanceOf(typeof(Db4oIOException), thread.Caught());
		}

		private void AssertWriteClose(Socket4Adapter socketToBeClosed, ICodeBlock codeBlock
			)
		{
			socketToBeClosed.Close();
			Assert.Expect(typeof(Db4oIOException), new _ICodeBlock_101(codeBlock));
		}

		private sealed class _ICodeBlock_101 : ICodeBlock
		{
			public _ICodeBlock_101(ICodeBlock codeBlock)
			{
				this.codeBlock = codeBlock;
			}

			/// <exception cref="System.Exception"></exception>
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
				_thread = new Thread(new _IRunnable_125(this));
				_codeBlock = codeBlock;
			}

			private sealed class _IRunnable_125 : IRunnable
			{
				public _IRunnable_125(CatchAllThread _enclosing)
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

			/// <exception cref="System.Exception"></exception>
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
#endif // !SILVERLIGHT
