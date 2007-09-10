/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class UnmarshallingContext : IFieldListInfo, IMarshallingInfo, IReadContext
	{
		private readonly Db4objects.Db4o.Internal.Transaction _transaction;

		private readonly ObjectReference _reference;

		private object _object;

		private Db4objects.Db4o.Internal.Buffer _buffer;

		private ObjectHeader _objectHeader;

		private int _addToIDTree;

		private bool _checkIDTree;

		private int _activationDepth;

		public UnmarshallingContext(Db4objects.Db4o.Internal.Transaction transaction, ObjectReference
			 @ref, int addToIDTree, bool checkIDTree)
		{
			_transaction = transaction;
			_reference = @ref;
			_addToIDTree = addToIDTree;
			_checkIDTree = checkIDTree;
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

		public virtual Db4objects.Db4o.Internal.StatefulBuffer StatefulBuffer()
		{
			Db4objects.Db4o.Internal.StatefulBuffer buffer = new Db4objects.Db4o.Internal.StatefulBuffer
				(_transaction, _buffer.Length());
			buffer.SetID(ObjectID());
			buffer.SetInstantiationDepth(ActivationDepth());
			_buffer.CopyTo(buffer, 0, 0, _buffer.Length());
			buffer.Offset(_buffer.Offset());
			return buffer;
		}

		public virtual int ObjectID()
		{
			return _reference.GetID();
		}

		public virtual ObjectContainerBase Container()
		{
			return _transaction.Container();
		}

		public virtual object Read()
		{
			if (!BeginProcessing())
			{
				return _object;
			}
			if (_buffer == null && ObjectID() > 0)
			{
				_buffer = Container().ReadReaderByID(_transaction, ObjectID());
			}
			if (_buffer == null)
			{
				EndProcessing();
				return _object;
			}
			_objectHeader = new ObjectHeader(Container(), _buffer);
			Db4objects.Db4o.Internal.ClassMetadata classMetadata = _objectHeader.ClassMetadata
				();
			if (classMetadata == null)
			{
				EndProcessing();
				return _object;
			}
			_reference.ClassMetadata(classMetadata);
			if (_checkIDTree)
			{
				object objectInCacheFromClassCreation = _transaction.ObjectForIdFromCache(ObjectID
					());
				if (objectInCacheFromClassCreation != null)
				{
					_object = objectInCacheFromClassCreation;
					EndProcessing();
					return _object;
				}
			}
			if (PeekPersisted())
			{
				_object = ClassMetadata().InstantiateTransient(this);
			}
			else
			{
				_object = ClassMetadata().Instantiate(this);
			}
			EndProcessing();
			return _object;
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
		{
			return _reference.ClassMetadata();
		}

		private bool BeginProcessing()
		{
			return _reference.BeginProcessing();
		}

		private void EndProcessing()
		{
			_reference.EndProcessing();
		}

		public virtual void SetStateClean()
		{
			_reference.SetStateClean();
		}

		public virtual object PersistentObject()
		{
			return _object;
		}

		public virtual void SetObjectWeak(object obj)
		{
			_reference.SetObjectWeak(Container(), obj);
		}

		public virtual object ReadAny()
		{
			int payloadOffset = ReadInt();
			if (payloadOffset == 0)
			{
				return null;
			}
			int savedOffSet = Offset();
			Seek(payloadOffset);
			Db4objects.Db4o.Internal.ClassMetadata classMetadata = Container().ClassMetadataForId
				(ReadInt());
			if (classMetadata == null)
			{
				Seek(savedOffSet);
				return null;
			}
			if (classMetadata is PrimitiveFieldHandler && classMetadata.IsArray())
			{
				Seek(ReadInt());
			}
			object obj = classMetadata.Read(this);
			Seek(savedOffSet);
			return obj;
		}

		public virtual object ReadObject()
		{
			int id = ReadInt();
			int depth = _activationDepth - 1;
			if (PeekPersisted())
			{
				return Container().PeekPersisted(Transaction(), id, depth);
			}
			object obj = Container().GetByID2(Transaction(), id);
			if (obj is IDb4oTypeImpl)
			{
				depth = ((IDb4oTypeImpl)obj).AdjustReadDepth(depth);
			}
			Container().StillToActivate(Transaction(), obj, depth);
			return obj;
		}

		private bool PeekPersisted()
		{
			return _addToIDTree == Const4.TRANSIENT;
		}

		public virtual object ReadObject(ITypeHandler4 handler)
		{
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

		public virtual void AdjustInstantiationDepth()
		{
			Config4Class classConfig = ClassConfig();
			if (classConfig != null)
			{
				_activationDepth = classConfig.AdjustActivationDepth(_activationDepth);
			}
		}

		public virtual Config4Class ClassConfig()
		{
			return ClassMetadata().Config();
		}

		public virtual int Offset()
		{
			return _buffer.Offset();
		}

		public virtual void Seek(int offset)
		{
			_buffer.Seek(offset);
		}

		public virtual ObjectReference Reference()
		{
			return _reference;
		}

		public virtual void AddToIDTree()
		{
			if (_addToIDTree == Const4.ADD_TO_ID_TREE)
			{
				_reference.AddExistingReferenceToIdTree(Transaction());
			}
		}

		public virtual int ActivationDepth()
		{
			return _activationDepth;
		}

		public virtual void ActivationDepth(int depth)
		{
			_activationDepth = depth;
		}

		public virtual void PersistentObject(object obj)
		{
			_object = obj;
		}

		public virtual ObjectHeaderAttributes HeaderAttributes()
		{
			return _objectHeader._headerAttributes;
		}

		public virtual bool IsNull(int fieldIndex)
		{
			return HeaderAttributes().IsNull(fieldIndex);
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

		private bool IsIndirected(ITypeHandler4 handler)
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

		public virtual int HandlerVersion()
		{
			return _objectHeader.HandlerVersion();
		}

		public virtual ITypeHandler4 CorrectHandlerVersion(ITypeHandler4 handler)
		{
			if (!OldHandlerVersion())
			{
				return handler;
			}
			return Container().Handlers().CorrectHandlerVersion(handler, HandlerVersion());
		}
	}
}
