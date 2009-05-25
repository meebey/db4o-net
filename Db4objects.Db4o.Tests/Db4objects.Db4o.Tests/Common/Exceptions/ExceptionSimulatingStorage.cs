/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class ExceptionSimulatingStorage : StorageDecorator
	{
		private IoAdapter _delegate = new RandomAccessFileAdapter();

		private readonly IExceptionFactory _exceptionFactory;

		private readonly BooleanByRef _triggersException = new BooleanByRef(false);

		public ExceptionSimulatingStorage(IStorage storage, IExceptionFactory exceptionFactory
			) : base(storage)
		{
			_exceptionFactory = exceptionFactory;
		}

		public override void Delete(string path)
		{
			if (TriggersException())
			{
				return;
			}
			_delegate.Delete(path);
		}

		public override bool Exists(string path)
		{
			if (TriggersException())
			{
				return false;
			}
			else
			{
				return _delegate.Exists(path);
			}
		}

		protected override IBin Decorate(IBin bin)
		{
			return new ExceptionSimulatingStorage.ExceptionSimulatingBin(bin, _exceptionFactory
				, _triggersException);
		}

		public virtual void TriggerException(bool exception)
		{
			this._triggersException.value = exception;
		}

		public virtual bool TriggersException()
		{
			return this._triggersException.value;
		}

		internal class ExceptionSimulatingBin : BinDecorator
		{
			private readonly IExceptionFactory _exceptionFactory;

			private readonly BooleanByRef _triggersException;

			public ExceptionSimulatingBin(IBin bin, IExceptionFactory exceptionFactory, BooleanByRef
				 triggersException) : base(bin)
			{
				_exceptionFactory = exceptionFactory;
				_triggersException = triggersException;
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			public override int Read(long pos, byte[] bytes, int length)
			{
				if (TriggersException())
				{
					_exceptionFactory.ThrowException();
					return 0;
				}
				else
				{
					return _bin.Read(pos, bytes, length);
				}
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			public override void Sync()
			{
				if (TriggersException())
				{
					_exceptionFactory.ThrowException();
				}
				else
				{
					_bin.Sync();
				}
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			public override void Write(long pos, byte[] buffer, int length)
			{
				if (TriggersException())
				{
					_exceptionFactory.ThrowException();
				}
				else
				{
					_bin.Write(pos, buffer, length);
				}
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			public override void Close()
			{
				if (TriggersException())
				{
					_exceptionFactory.ThrowException();
				}
				else
				{
					_bin.Close();
				}
			}

			/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
			public override long Length()
			{
				if (TriggersException())
				{
					_exceptionFactory.ThrowException();
					return 0;
				}
				else
				{
					return _bin.Length();
				}
			}

			private bool TriggersException()
			{
				return _triggersException.value;
			}
		}
	}
}
