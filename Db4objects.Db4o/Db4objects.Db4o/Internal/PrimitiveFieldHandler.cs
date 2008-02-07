/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Fieldhandlers;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class PrimitiveFieldHandler : ClassMetadata, IFieldHandler
	{
		private readonly ITypeHandler4 _handler;

		internal PrimitiveFieldHandler(ObjectContainerBase container, ITypeHandler4 handler
			, int handlerID, IReflectClass classReflector) : base(container, classReflector)
		{
			i_fields = FieldMetadata.EmptyArray;
			_handler = handler;
			_id = handlerID;
		}

		internal PrimitiveFieldHandler(Db4objects.Db4o.Internal.PrimitiveFieldHandler prototype
			, HandlerRegistry registry, int version) : this(prototype.Container(), registry.
			CorrectHandlerVersion(prototype._handler, version), prototype._id, prototype.ClassReflector
			())
		{
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
				new ArrayHandler(a_bytes.GetStream(), this, true).DeletePrimitiveEmbedded(a_bytes
					, this);
			}
			else
			{
				if (a_type == Const4.TypeNarray)
				{
					new MultidimensionalArrayHandler(a_bytes.GetStream(), this, true).DeletePrimitiveEmbedded
						(a_bytes, this);
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

		internal override void InstantiateFields(UnmarshallingContext context)
		{
			object obj = context.Read(_handler);
			if (obj != null && (_handler is Db4objects.Db4o.Internal.Handlers.DateHandler))
			{
				object existing = context.PersistentObject();
				context.PersistentObject(DateHandler().CopyValue(obj, existing));
			}
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

		public override IPreparedComparison PrepareComparison(object source)
		{
			return _handler.PrepareComparison(source);
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
	}
}
