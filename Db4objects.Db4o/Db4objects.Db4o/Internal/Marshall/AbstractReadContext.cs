/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class AbstractReadContext : IInternalReadContext
	{
		protected readonly Db4objects.Db4o.Internal.Transaction _transaction;

		protected Db4objects.Db4o.Internal.Buffer _buffer;

		protected IActivationDepth _activationDepth = UnknownActivationDepth.INSTANCE;

		protected AbstractReadContext(Db4objects.Db4o.Internal.Transaction transaction)
		{
			_transaction = transaction;
		}

		protected AbstractReadContext(Db4objects.Db4o.Internal.Transaction transaction, Db4objects.Db4o.Internal.Buffer
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

		public virtual object Read(ITypeHandler4 handlerType)
		{
			ITypeHandler4 handler = CorrectHandlerVersion(handlerType);
			if (!IsIndirected(handler))
			{
				return handler.Read(this);
			}
			int indirectedOffSet = ReadInt();
			ReadInt();
			int offset = Offset();
			Seek(indirectedOffSet);
			object obj = handler.Read(this);
			Seek(offset);
			return obj;
		}

		public virtual object ReadObject()
		{
			int id = ReadInt();
			if (id == 0)
			{
				return null;
			}
			ClassMetadata classMetadata = ClassMetadataForId(id);
			if (null == classMetadata)
			{
				return null;
			}
			IActivationDepth depth = ActivationDepth().Descend(classMetadata);
			if (PeekPersisted())
			{
				return Container().PeekPersisted(Transaction(), id, depth, false);
			}
			object obj = Container().GetByID2(Transaction(), id);
			if (null == obj)
			{
				return null;
			}
			Container().StillToActivate(Transaction(), obj, depth);
			return obj;
		}

		private ClassMetadata ClassMetadataForId(int id)
		{
			HardObjectReference hardRef = Container().GetHardObjectReferenceById(Transaction(
				), id);
			if (null == hardRef || hardRef._reference == null)
			{
				return null;
			}
			return hardRef._reference.ClassMetadata();
		}

		protected virtual bool PeekPersisted()
		{
			return false;
		}

		public virtual object ReadObject(ITypeHandler4 handlerType)
		{
			ITypeHandler4 handler = CorrectHandlerVersion(handlerType);
			if (!IsIndirected(handler))
			{
				return handler.Read(this);
			}
			int payLoadOffset = ReadInt();
			ReadInt();
			if (payLoadOffset == 0)
			{
				return null;
			}
			int savedOffset = Offset();
			Seek(payLoadOffset);
			object obj = handler.Read(this);
			Seek(savedOffset);
			return obj;
		}

		public virtual IActivationDepth ActivationDepth()
		{
			return _activationDepth;
		}

		public virtual void ActivationDepth(IActivationDepth depth)
		{
			_activationDepth = depth;
		}

		public virtual int Offset()
		{
			return _buffer.Offset();
		}

		public virtual void Seek(int offset)
		{
			_buffer.Seek(offset);
		}

		public virtual bool IsIndirected(ITypeHandler4 handler)
		{
			if (HandlerVersion() == 0)
			{
				return false;
			}
			return HandlerRegistry().IsVariableLength(handler);
		}

		private Db4objects.Db4o.Internal.HandlerRegistry HandlerRegistry()
		{
			return Container().Handlers();
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
