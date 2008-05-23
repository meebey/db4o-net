/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Fieldhandlers;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class PrimitiveFieldHandler : ClassMetadata, IFieldHandler, ICompositeTypeHandler
	{
		private const int HashcodeForNull = 283636383;

		private readonly ITypeHandler4 _handler;

		public PrimitiveFieldHandler(ObjectContainerBase container, ITypeHandler4 handler
			, int handlerID, IReflectClass classReflector) : base(container, classReflector)
		{
			i_fields = FieldMetadata.EmptyArray;
			_handler = handler;
			_id = handlerID;
		}

		public PrimitiveFieldHandler() : base(null, null)
		{
			_handler = null;
		}

		internal override void ActivateFields(Transaction trans, object obj, IActivationDepth
			 depth)
		{
		}

		// Override
		// do nothing
		internal sealed override void AddToIndex(LocalObjectContainer container, Transaction
			 trans, int id)
		{
		}

		// Override
		// Primitive Indices will be created later.
		internal override bool AllowsQueries()
		{
			return false;
		}

		internal override void CacheDirty(Collection4 col)
		{
		}

		// do nothing
		protected override bool DescendOnCascadingActivation()
		{
			return false;
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void Delete(IDeleteContext context)
		{
			if (context.IsLegacyHandlerVersion())
			{
				context.ReadInt();
				context.DefragmentRecommended();
			}
		}

		internal override void DeleteMembers(MarshallerFamily mf, ObjectHeaderAttributes 
			attributes, StatefulBuffer a_bytes, int a_type, bool isUpdate)
		{
			if (a_type == Const4.TypeArray)
			{
				new ArrayHandler(this, true).DeletePrimitiveEmbedded(a_bytes, this);
			}
			else
			{
				if (a_type == Const4.TypeNarray)
				{
					new MultidimensionalArrayHandler(this, true).DeletePrimitiveEmbedded(a_bytes, this
						);
				}
			}
		}

		internal void Free(StatefulBuffer a_bytes, int a_id)
		{
			a_bytes.GetTransaction().SlotFreePointerOnCommit(a_id, a_bytes.Slot());
		}

		public override bool HasClassIndex()
		{
			return false;
		}

		public override object Instantiate(UnmarshallingContext context)
		{
			object obj = context.PersistentObject();
			if (obj == null)
			{
				obj = context.Read(_handler);
				context.SetObjectWeak(obj);
			}
			context.SetStateClean();
			return obj;
		}

		public override object InstantiateTransient(UnmarshallingContext context)
		{
			return _handler.Read(context);
		}

		internal override object InstantiateFields(UnmarshallingContext context)
		{
			object obj = context.Read(_handler);
			if (obj == null || !(_handler is Db4objects.Db4o.Internal.Handlers.DateHandler))
			{
				return obj;
			}
			object existing = context.PersistentObject();
			object newValue = DateHandler().CopyValue(obj, existing);
			// FIXME: It should not be necessary to set persistentObject here
			context.PersistentObject(newValue);
			return newValue;
		}

		private Db4objects.Db4o.Internal.Handlers.DateHandler DateHandler()
		{
			return ((Db4objects.Db4o.Internal.Handlers.DateHandler)_handler);
		}

		public override bool IsArray()
		{
			return _id == Handlers4.AnyArrayId || _id == Handlers4.AnyArrayNId;
		}

		public override bool IsPrimitive()
		{
			return true;
		}

		public override bool IsStrongTyped()
		{
			return false;
		}

		public override IPreparedComparison PrepareComparison(IContext context, object source
			)
		{
			return _handler.PrepareComparison(context, source);
		}

		public override ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, ByteArrayBuffer[] a_bytes)
		{
			if (IsArray())
			{
				return _handler;
			}
			return null;
		}

		public override ObjectID ReadObjectID(IInternalReadContext context)
		{
			if (_handler is ClassMetadata)
			{
				return ((ClassMetadata)_handler).ReadObjectID(context);
			}
			if (_handler is ArrayHandler)
			{
				// TODO: Here we should theoretically read through the array and collect candidates.
				// The respective construct is wild: "Contains query through an array in an array."
				// Ignore for now.
				return ObjectID.Ignore;
			}
			return ObjectID.NotPossible;
		}

		internal override void RemoveFromIndex(Transaction ta, int id)
		{
		}

		// do nothing
		public sealed override bool WriteObjectBegin()
		{
			return false;
		}

		public override string ToString()
		{
			return "Wraps " + _handler.ToString() + " in YapClassPrimitive";
		}

		public override void Defragment(IDefragmentContext context)
		{
			_handler.Defragment(context);
		}

		public override object WrapWithTransactionContext(Transaction transaction, object
			 value)
		{
			return value;
		}

		public override object Read(IReadContext context)
		{
			return _handler.Read(context);
		}

		public override void Write(IWriteContext context, object obj)
		{
			_handler.Write(context, obj);
		}

		public override ITypeHandler4 TypeHandler()
		{
			return _handler;
		}

		public override ITypeHandler4 DelegateTypeHandler()
		{
			return _handler;
		}

		public virtual ITypeHandler4 GenericTemplate()
		{
			return new Db4objects.Db4o.Internal.PrimitiveFieldHandler(null, null, 0, null);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Db4objects.Db4o.Internal.PrimitiveFieldHandler))
			{
				return false;
			}
			Db4objects.Db4o.Internal.PrimitiveFieldHandler other = (Db4objects.Db4o.Internal.PrimitiveFieldHandler
				)obj;
			if (_handler == null)
			{
				return other._handler == null;
			}
			return _handler.Equals(other._handler);
		}

		public override int GetHashCode()
		{
			if (_handler == null)
			{
				return HashcodeForNull;
			}
			return _handler.GetHashCode();
		}

		public virtual object DeepClone(object context)
		{
			TypeHandlerCloneContext typeHandlerCloneContext = (TypeHandlerCloneContext)context;
			Db4objects.Db4o.Internal.PrimitiveFieldHandler original = (Db4objects.Db4o.Internal.PrimitiveFieldHandler
				)typeHandlerCloneContext.original;
			ITypeHandler4 delegateTypeHandler = typeHandlerCloneContext.CorrectHandlerVersion
				(original.DelegateTypeHandler());
			return new Db4objects.Db4o.Internal.PrimitiveFieldHandler(original.Container(), delegateTypeHandler
				, original._id, original.ClassReflector());
		}

		public override bool IsSecondClass()
		{
			return IsSecondClass(_handler);
		}
	}
}
