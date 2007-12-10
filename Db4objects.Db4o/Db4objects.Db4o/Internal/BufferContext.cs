/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class BufferContext : IReadBuffer
	{
		protected readonly Db4objects.Db4o.Internal.Transaction _transaction;

		protected Db4objects.Db4o.Internal.Buffer _buffer;

		public BufferContext(Db4objects.Db4o.Internal.Transaction transaction)
		{
			_transaction = transaction;
		}

		public BufferContext(Db4objects.Db4o.Internal.Transaction transaction, Db4objects.Db4o.Internal.Buffer
			 buffer)
		{
			_transaction = transaction;
			_buffer = buffer;
		}

		public virtual Db4objects.Db4o.Internal.Buffer Buffer(Db4objects.Db4o.Internal.Buffer
			 buffer)
		{
			Db4objects.Db4o.Internal.Buffer temp = _buffer;
			_buffer = buffer;
			return temp;
		}

		public virtual Db4objects.Db4o.Internal.Buffer Buffer()
		{
			return _buffer;
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

		public virtual bool OldHandlerVersion()
		{
			return HandlerVersion() != MarshallingContext.HANDLER_VERSION;
		}

		public virtual ITypeHandler4 CorrectHandlerVersion(ITypeHandler4 handler)
		{
			if (!OldHandlerVersion())
			{
				return handler;
			}
			return Container().Handlers().CorrectHandlerVersion(handler, HandlerVersion());
		}

		public abstract int HandlerVersion();
	}
}
