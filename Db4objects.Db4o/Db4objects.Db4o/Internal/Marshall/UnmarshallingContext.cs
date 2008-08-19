/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <summary>Wraps the low-level details of reading a Buffer, which in turn is a glorified byte array.
	/// 	</summary>
	/// <remarks>Wraps the low-level details of reading a Buffer, which in turn is a glorified byte array.
	/// 	</remarks>
	/// <exclude></exclude>
	public class UnmarshallingContext : ObjectReferenceContext, IHandlerVersionContext
	{
		private object _object;

		private int _addToIDTree;

		private bool _checkIDTree;

		public UnmarshallingContext(Transaction transaction, ByteArrayBuffer buffer, ObjectReference
			 @ref, int addToIDTree, bool checkIDTree) : base(transaction, buffer, null, @ref
			)
		{
			_addToIDTree = addToIDTree;
			_checkIDTree = checkIDTree;
		}

		public UnmarshallingContext(Transaction transaction, ObjectReference @ref, int addToIDTree
			, bool checkIDTree) : this(transaction, null, @ref, addToIDTree, checkIDTree)
		{
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
			ClassMetadata classMetadata = ReadObjectHeader();
			if (classMetadata == null)
			{
				EndProcessing();
				return _object;
			}
			_reference.ClassMetadata(classMetadata);
			AdjustActivationDepth(doAdjustActivationDepthForPrefetch);
			if (_checkIDTree)
			{
				object objectInCacheFromClassCreation = Transaction().ObjectForIdFromCache(ObjectID
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

		public virtual object ReadActivatedObject(ITypeHandler4 handler)
		{
			IActivationDepth tempDepth = ActivationDepth();
			ActivationDepthProvider().ActivationDepth(int.MaxValue, ActivationMode.Activate);
			object obj = ReadObject(handler);
			Container().Activate(Transaction(), obj, ActivationDepth());
			ActivationDepth(tempDepth);
			return obj;
		}

		public virtual object ReadFieldValue(FieldMetadata field)
		{
			ReadBuffer(ObjectID());
			if (Buffer() == null)
			{
				return null;
			}
			ClassMetadata classMetadata = ReadObjectHeader();
			if (classMetadata == null)
			{
				return null;
			}
			return ReadFieldValue(classMetadata, field);
		}

		private ClassMetadata ReadObjectHeader()
		{
			_objectHeader = new ObjectHeader(Container(), ByteArrayBuffer());
			ClassMetadata classMetadata = _objectHeader.ClassMetadata();
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
				Buffer(Container().ReadReaderByID(Transaction(), id));
			}
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

		public virtual void PersistentObject(object obj)
		{
			_object = obj;
		}
	}
}
