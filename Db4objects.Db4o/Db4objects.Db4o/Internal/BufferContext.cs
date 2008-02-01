/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class BufferContext : IReadBuffer
	{
		protected IReadWriteBuffer _buffer;

		protected readonly Db4objects.Db4o.Internal.Transaction _transaction;

		public BufferContext(Db4objects.Db4o.Internal.Transaction transaction, IReadWriteBuffer
			 buffer)
		{
			_transaction = transaction;
			_buffer = buffer;
		}

		public virtual IReadWriteBuffer Buffer(IReadWriteBuffer buffer)
		{
			IReadWriteBuffer temp = _buffer;
			_buffer = buffer;
			return temp;
		}

		public virtual IReadWriteBuffer Buffer()
		{
			return _buffer;
		}

		public virtual byte ReadByte()
		{
			return _buffer.ReadByte();
		}

		public virtual void ReadBytes(byte[] bytes)
		{
			_buffer.ReadBytes(bytes);
		}

		public virtual int ReadInt()
		{
			return _buffer.ReadInt();
		}

		public virtual long ReadLong()
		{
			return _buffer.ReadLong();
		}

		public virtual int Offset()
		{
			return _buffer.Offset();
		}

		public virtual void Seek(int offset)
		{
			_buffer.Seek(offset);
		}

		public virtual ObjectContainerBase Container()
		{
			return _transaction.Container();
		}

		public virtual IObjectContainer ObjectContainer()
		{
			return (IObjectContainer)Container();
		}

		public virtual Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return _transaction;
		}

		public virtual ITypeHandler4 CorrectHandlerVersion(ITypeHandler4 handler)
		{
			return Container().Handlers().CorrectHandlerVersion(handler, HandlerVersion());
		}

		public abstract int HandlerVersion();

		public virtual bool IsLegacyHandlerVersion()
		{
			return HandlerVersion() == 0;
		}
	}
}
