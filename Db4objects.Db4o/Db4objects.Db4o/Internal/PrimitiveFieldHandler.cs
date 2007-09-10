/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class PrimitiveFieldHandler : ClassMetadata
	{
		public readonly ITypeHandler4 i_handler;

		internal PrimitiveFieldHandler(ObjectContainerBase container, ITypeHandler4 handler
			) : base(container, handler.ClassReflector())
		{
			i_fields = FieldMetadata.EMPTY_ARRAY;
			i_handler = handler;
		}

		internal override void ActivateFields(Transaction trans, object obj, int depth)
		{
		}

		internal sealed override void AddToIndex(LocalObjectContainer container, Transaction
			 trans, int id)
		{
		}

		internal override bool AllowsQueries()
		{
			return false;
		}

		internal override void CacheDirty(Collection4 col)
		{
		}

		public override IReflectClass ClassReflector()
		{
			return i_handler.ClassReflector();
		}

		public override void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer a_bytes)
		{
			if (mf._primitive.UseNormalClassRead())
			{
				base.DeleteEmbedded(mf, a_bytes);
				return;
			}
		}

		public override void DeleteEmbedded1(MarshallerFamily mf, StatefulBuffer a_bytes, 
			int a_id)
		{
			if (i_handler is ArrayHandler)
			{
				ArrayHandler ya = (ArrayHandler)i_handler;
				if (ya._isPrimitive)
				{
					ya.DeletePrimitiveEmbedded(a_bytes, this);
					a_bytes.SlotDelete();
					return;
				}
			}
			if (i_handler is UntypedFieldHandler)
			{
				a_bytes.IncrementOffset(i_handler.LinkLength());
			}
			else
			{
				i_handler.DeleteEmbedded(mf, a_bytes);
			}
			Free(a_bytes, a_id);
		}

		internal override void DeleteMembers(MarshallerFamily mf, ObjectHeaderAttributes 
			attributes, StatefulBuffer a_bytes, int a_type, bool isUpdate)
		{
			if (a_type == Const4.TYPE_ARRAY)
			{
				new ArrayHandler(a_bytes.GetStream(), this, true).DeletePrimitiveEmbedded(a_bytes
					, this);
			}
			else
			{
				if (a_type == Const4.TYPE_NARRAY)
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

		internal override object Instantiate(ObjectReference @ref, object obj, MarshallerFamily
			 mf, ObjectHeaderAttributes attributes, StatefulBuffer buffer, bool addToIDTree)
		{
			if (obj == null)
			{
				try
				{
					obj = i_handler.Read(mf, buffer, true);
				}
				catch (CorruptionException)
				{
					return null;
				}
				@ref.SetObjectWeak(buffer.GetStream(), obj);
			}
			@ref.SetStateClean();
			return obj;
		}

		internal override object InstantiateTransient(ObjectReference a_yapObject, object
			 a_object, MarshallerFamily mf, ObjectHeaderAttributes attributes, StatefulBuffer
			 a_bytes)
		{
			try
			{
				return i_handler.Read(mf, a_bytes, true);
			}
			catch (CorruptionException)
			{
				return null;
			}
		}

		public override object Instantiate(UnmarshallingContext context)
		{
			object obj = context.PersistentObject();
			if (obj == null)
			{
				obj = context.Read(i_handler);
				context.SetObjectWeak(obj);
			}
			context.SetStateClean();
			return obj;
		}

		public override object InstantiateTransient(UnmarshallingContext context)
		{
			return i_handler.Read(context);
		}

		internal override void InstantiateFields(ObjectReference a_yapObject, object a_onObject
			, MarshallerFamily mf, ObjectHeaderAttributes attributes, StatefulBuffer a_bytes
			)
		{
			object obj = null;
			try
			{
				obj = i_handler.Read(mf, a_bytes, true);
			}
			catch (CorruptionException)
			{
			}
			if (obj != null && (i_handler is DateHandler))
			{
				((DateHandler)i_handler).CopyValue(obj, a_onObject);
			}
		}

		internal override void InstantiateFields(UnmarshallingContext context)
		{
			object obj = context.Read(i_handler);
			if (obj != null && (i_handler is DateHandler))
			{
				((DateHandler)i_handler).CopyValue(obj, context.PersistentObject());
			}
		}

		public override bool IsArray()
		{
			return _id == HandlerRegistry.ANY_ARRAY_ID || _id == HandlerRegistry.ANY_ARRAY_N_ID;
		}

		public override bool IsPrimitive()
		{
			return true;
		}

		public override bool IsStrongTyped()
		{
			return false;
		}

		public override IComparable4 PrepareComparison(object a_constraint)
		{
			i_handler.PrepareComparison(a_constraint);
			return i_handler;
		}

		public override object Read(MarshallerFamily mf, StatefulBuffer a_bytes, bool redirect
			)
		{
			if (mf._primitive.UseNormalClassRead())
			{
				return base.Read(mf, a_bytes, redirect);
			}
			return i_handler.Read(mf, a_bytes, false);
		}

		public override ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer[] a_bytes)
		{
			if (IsArray())
			{
				return i_handler;
			}
			return null;
		}

		public override object ReadQuery(Transaction trans, MarshallerFamily mf, bool withRedirection
			, Db4objects.Db4o.Internal.Buffer reader, bool toArray)
		{
			if (mf._primitive.UseNormalClassRead())
			{
				return base.ReadQuery(trans, mf, withRedirection, reader, toArray);
			}
			return i_handler.ReadQuery(trans, mf, withRedirection, reader, toArray);
		}

		public override QCandidate ReadSubCandidate(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates, bool withIndirection)
		{
			return i_handler.ReadSubCandidate(mf, reader, candidates, withIndirection);
		}

		internal override void RemoveFromIndex(Transaction ta, int id)
		{
		}

		public sealed override bool WriteObjectBegin()
		{
			return false;
		}

		public override string ToString()
		{
			return "Wraps " + i_handler.ToString() + " in YapClassPrimitive";
		}

		public override void Defrag(MarshallerFamily mf, BufferPair readers, bool redirect
			)
		{
			if (mf._primitive.UseNormalClassRead())
			{
				base.Defrag(mf, readers, redirect);
			}
			else
			{
				i_handler.Defrag(mf, readers, false);
			}
		}

		public override object WrapWithTransactionContext(Transaction transaction, object
			 value)
		{
			return value;
		}

		public override object Read(IReadContext context)
		{
			return i_handler.Read(context);
		}

		public override void Write(IWriteContext context, object obj)
		{
			i_handler.Write(context, obj);
		}
	}
}
