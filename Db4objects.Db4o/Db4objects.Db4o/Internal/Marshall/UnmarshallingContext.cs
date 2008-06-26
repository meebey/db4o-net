/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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
	public class UnmarshallingContext : ObjectHeaderContext, IFieldListInfo, IMarshallingInfo
	{
		private readonly ObjectReference _reference;

		private object _object;

		private int _addToIDTree;

		private bool _checkIDTree;

		public UnmarshallingContext(Transaction transaction, Db4objects.Db4o.Internal.ByteArrayBuffer
			 buffer, ObjectReference @ref, int addToIDTree, bool checkIDTree) : base(transaction
			, buffer, null)
		{
			_reference = @ref;
			_addToIDTree = addToIDTree;
			_checkIDTree = checkIDTree;
		}

		public UnmarshallingContext(Transaction transaction, ObjectReference @ref, int addToIDTree
			, bool checkIDTree) : this(transaction, null, @ref, addToIDTree, checkIDTree)
		{
		}

		private Db4objects.Db4o.Internal.ByteArrayBuffer ByteArrayBuffer()
		{
			return (Db4objects.Db4o.Internal.ByteArrayBuffer)Buffer();
		}

		public virtual Db4objects.Db4o.Internal.StatefulBuffer StatefulBuffer()
		{
			Db4objects.Db4o.Internal.StatefulBuffer statefulBuffer = new Db4objects.Db4o.Internal.StatefulBuffer
				(_transaction, ByteArrayBuffer().Length());
			statefulBuffer.SetID(ObjectID());
			statefulBuffer.SetInstantiationDepth(ActivationDepth());
			ByteArrayBuffer().CopyTo(statefulBuffer, 0, 0, ByteArrayBuffer().Length());
			statefulBuffer.Seek(ByteArrayBuffer().Offset());
			return statefulBuffer;
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
			if (Buffer() == null)
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
			if (Buffer() == null)
			{
				return null;
			}
			Db4objects.Db4o.Internal.ClassMetadata classMetadata = ReadObjectHeader();
			if (classMetadata == null)
			{
				return null;
			}
			return ReadFieldValue(classMetadata, field);
		}

		private Db4objects.Db4o.Internal.ClassMetadata ReadObjectHeader()
		{
			_objectHeader = new ObjectHeader(Container(), ByteArrayBuffer());
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
			if (Buffer() == null && id > 0)
			{
				Buffer(Container().ReadReaderByID(_transaction, id));
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
	}
}
