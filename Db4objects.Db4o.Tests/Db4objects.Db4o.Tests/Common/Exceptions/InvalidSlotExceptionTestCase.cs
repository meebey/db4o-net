/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Exceptions;
using Sharpen;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class InvalidSlotExceptionTestCase : AbstractDb4oTestCase
	{
		private const int InvalidId = 3;

		private const int OutOfMemoryId = 4;

		public static void Main(string[] args)
		{
			new InvalidSlotExceptionTestCase().RunAll();
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.Io(new InvalidSlotExceptionTestCase.MockIoAdapter());
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestInvalidSlotException()
		{
			Assert.Expect(typeof(InvalidIDException), typeof(InvalidSlotException), new _ICodeBlock_30
				(this));
		}

		private sealed class _ICodeBlock_30 : ICodeBlock
		{
			public _ICodeBlock_30(InvalidSlotExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().GetByID(InvalidSlotExceptionTestCase.InvalidId);
			}

			private readonly InvalidSlotExceptionTestCase _enclosing;
		}

		public virtual void TestDbNotClosedOnOutOfMemory()
		{
			Type expectedException = IsClientServer() && !IsEmbeddedClientServer() ? typeof(InvalidIDException
				) : typeof(OutOfMemoryException);
			Assert.Expect(expectedException, new _ICodeBlock_40(this));
			Assert.IsFalse(Db().IsClosed());
		}

		private sealed class _ICodeBlock_40 : ICodeBlock
		{
			public _ICodeBlock_40(InvalidSlotExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().GetByID(InvalidSlotExceptionTestCase.OutOfMemoryId);
			}

			private readonly InvalidSlotExceptionTestCase _enclosing;
		}

		public class A
		{
			internal InvalidSlotExceptionTestCase.A _a;

			public A(InvalidSlotExceptionTestCase.A a)
			{
				this._a = a;
			}
		}

		public class MockIoAdapter : VanillaIoAdapter
		{
			private bool _deliverInvalidSlot;

			public MockIoAdapter() : base(new RandomAccessFileAdapter())
			{
			}

			/// <exception cref="Db4oIOException"></exception>
			protected MockIoAdapter(string path, bool lockFile, long initialLength, bool readOnly
				) : base(new RandomAccessFileAdapter(), path, lockFile, initialLength, readOnly)
			{
			}

			/// <exception cref="Db4oIOException"></exception>
			public override IoAdapter Open(string path, bool lockFile, long initialLength, bool
				 readOnly)
			{
				// TODO Auto-generated method stub
				return new InvalidSlotExceptionTestCase.MockIoAdapter(path, lockFile, initialLength
					, readOnly);
			}

			/// <exception cref="Db4oIOException"></exception>
			public override void Seek(long pos)
			{
				if (pos == OutOfMemoryId)
				{
					throw new OutOfMemoryException();
				}
				if (pos == InvalidId)
				{
					_deliverInvalidSlot = true;
					return;
				}
				_deliverInvalidSlot = false;
				base.Seek(pos);
			}

			/// <exception cref="Db4oIOException"></exception>
			public override int Read(byte[] bytes, int length)
			{
				if (_deliverInvalidSlot)
				{
					ByteArrayBuffer buffer = new ByteArrayBuffer(Const4.PointerLength);
					buffer.WriteInt(1);
					buffer.WriteInt(int.MaxValue);
					System.Array.Copy(buffer._buffer, 0, bytes, 0, Const4.PointerLength);
					return length;
				}
				return base.Read(bytes, length);
			}
		}
	}
}
