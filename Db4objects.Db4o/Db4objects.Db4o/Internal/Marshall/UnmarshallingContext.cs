/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <summary>Wraps the low-level details of reading a Buffer, which in turn is a glorified byte array.
	/// 	</summary>
	/// <remarks>Wraps the low-level details of reading a Buffer, which in turn is a glorified byte array.
	/// 	</remarks>
	/// <exclude></exclude>
	public class UnmarshallingContext : AbstractReadContext, IFieldListInfo, IMarshallingInfo
	{
		private readonly ObjectReference _reference;

		private object _object;

		private ObjectHeader _objectHeader;

		private int _addToIDTree;

		private bool _checkIDTree;

		public UnmarshallingContext(Transaction transaction, BufferImpl buffer, ObjectReference
			 @ref, int addToIDTree, bool checkIDTree) : base(transaction, buffer)
		{
			_reference = @ref;
			_addToIDTree = addToIDTree;
			_checkIDTree = checkIDTree;
		}

		public UnmarshallingContext(Transaction transaction, ObjectReference @ref, int addToIDTree
			, bool checkIDTree) : this(transaction, null, @ref, addToIDTree, checkIDTree)
		{
		}

		public virtual Db4objects.Db4o.Internal.StatefulBuffer StatefulBuffer()
		{
			Db4objects.Db4o.Internal.StatefulBuffer buffer = new Db4objects.Db4o.Internal.StatefulBuffer
				(_transaction, _buffer.Length());
			buffer.SetID(ObjectID());
			buffer.SetInstantiationDepth(ActivationDepth());
			((BufferImpl)_buffer).CopyTo(buffer, 0, 0, _buffer.Length());
			buffer.Seek(_buffer.Offset());
			return buffer;
		}

		public virtual int ObjectID()
		{
			return _reference.GetID();
		}

		public virtual object Read()
		{
			return ReadInternal(false);
		}

		public virtual object ReadPrefetch()
		{
			return ReadInternal(true);
		}

		private object ReadInternal(bool doAdjustActivationDepthForPrefetch)
		{
			if (!BeginProcessing())
			{
				return _object;
			}
			ReadBuffer(ObjectID());
			if (_buffer == null)
			{
				EndProcessing();
				return _object;
			}
			Db4objects.Db4o.Internal.ClassMetadata classMetadata = ReadObjectHeader();
			if (classMetadata == null)
			{
				EndProcessing();
				return _object;
			}
			_reference.ClassMetadata(classMetadata);
			AdjustActivationDepth(doAdjustActivationDepthForPrefetch);
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

		private void AdjustActivationDepth(bool doAdjustActivationDepthForPrefetch)
		{
			if (doAdjustActivationDepthForPrefetch)
			{
				AdjustActivationDepthForPrefetch();
			}
			else
			{
				if (UnknownActivationDepth.Instance == _activationDepth)
				{
					_activationDepth = Container().DefaultActivationDepth(ClassMetadata());
				}
			}
		}

		private void AdjustActivationDepthForPrefetch()
		{
			ActivationDepth(ActivationDepthProvider().ActivationDepthFor(ClassMetadata(), ActivationMode
				.Prefetch));
		}

		private IActivationDepthProvider ActivationDepthProvider()
		{
			return Container().ActivationDepthProvider();
		}

		public virtual object ReadFieldValue(FieldMetadata field)
		{
			ReadBuffer(ObjectID());
			if (_buffer == null)
			{
				return null;
			}
			Db4objects.Db4o.Internal.ClassMetadata classMetadata = ReadObjectHeader();
			if (classMetadata == null)
			{
				return null;
			}
			if (!_objectHeader.ObjectMarshaller().FindOffset(classMetadata, _objectHeader._headerAttributes
				, (BufferImpl)_buffer, field))
			{
				return null;
			}
			return field.Read(this);
		}

		private Db4objects.Db4o.Internal.ClassMetadata ReadObjectHeader()
		{
			_objectHeader = new ObjectHeader(Container(), _buffer);
			Db4objects.Db4o.Internal.ClassMetadata classMetadata = _objectHeader.ClassMetadata
				();
			if (classMetadata == null)
			{
				return null;
			}
			return classMetadata;
		}

		private void ReadBuffer(int id)
		{
			if (_buffer == null && id > 0)
			{
				_buffer = Container().ReadReaderByID(_transaction, id);
			}
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

		protected override bool PeekPersisted()
		{
			return _addToIDTree == Const4.Transient;
		}

		public override object ReadObject(ITypeHandler4 handlerType)
		{
			ITypeHandler4 handler = CorrectHandlerVersion(handlerType);
			if (!IsIndirected(handler))
			{
				return handler.Read(this);
			}
			int payLoadOffset = ReadInt();
			ReadInt();
			// length - never used
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

		public virtual Config4Class ClassConfig()
		{
			return ClassMetadata().Config();
		}

		public virtual ObjectReference Reference()
		{
			return _reference;
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

		public override int HandlerVersion()
		{
			return _objectHeader.HandlerVersion();
		}
	}
}
