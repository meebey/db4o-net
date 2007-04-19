using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class PrimitiveFieldHandler : ClassMetadata
	{
		public readonly ITypeHandler4 i_handler;

		internal PrimitiveFieldHandler(ObjectContainerBase a_stream, ITypeHandler4 a_handler
			) : base(a_stream, a_handler.ClassReflector())
		{
			i_fields = FieldMetadata.EMPTY_ARRAY;
			i_handler = a_handler;
		}

		internal override void ActivateFields(Transaction a_trans, object a_object, int a_depth
			)
		{
		}

		internal sealed override void AddToIndex(LocalObjectContainer a_stream, Transaction
			 a_trans, int a_id)
		{
		}

		internal override bool AllowsQueries()
		{
			return false;
		}

		internal override void CacheDirty(Collection4 col)
		{
		}

		public override bool CanHold(IReflectClass claxx)
		{
			return i_handler.CanHold(claxx);
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
				if (ya.i_isPrimitive)
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

		internal void Free(Transaction a_trans, int a_id, int a_address, int a_length)
		{
			a_trans.SlotFreePointerOnCommit(a_id, a_address, a_length);
		}

		internal void Free(StatefulBuffer a_bytes, int a_id)
		{
			a_bytes.GetTransaction().SlotFreePointerOnCommit(a_id, a_bytes.GetAddress(), a_bytes
				.GetLength());
		}

		public override bool HasIndex()
		{
			return false;
		}

		internal override object Instantiate(ObjectReference a_yapObject, object a_object
			, MarshallerFamily mf, ObjectHeaderAttributes attributes, StatefulBuffer a_bytes
			, bool a_addToIDTree)
		{
			if (a_object == null)
			{
				try
				{
					a_object = i_handler.Read(mf, a_bytes, true);
				}
				catch (CorruptionException)
				{
					return null;
				}
				catch (IOException)
				{
					return null;
				}
				a_yapObject.SetObjectWeak(a_bytes.GetStream(), a_object);
			}
			a_yapObject.SetStateClean();
			return a_object;
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
			catch (IOException)
			{
				return null;
			}
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
			catch (IOException)
			{
			}
			if (obj != null)
			{
				i_handler.CopyValue(obj, a_onObject);
			}
		}

		public override bool IsArray()
		{
			return i_id == HandlerRegistry.ANY_ARRAY_ID || i_id == HandlerRegistry.ANY_ARRAY_N_ID;
		}

		public override bool IsPrimitive()
		{
			return true;
		}

		public override TernaryBool IsSecondClass()
		{
			return TernaryBool.UNSPECIFIED;
		}

		public override bool IsStrongTyped()
		{
			return false;
		}

		public override void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, bool topLevel, object obj, bool withIndirection)
		{
			i_handler.CalculateLengths(trans, header, topLevel, obj, withIndirection);
		}

		public override IComparable4 PrepareComparison(object a_constraint)
		{
			i_handler.PrepareComparison(a_constraint);
			return i_handler;
		}

		public sealed override IReflectClass PrimitiveClassReflector()
		{
			return i_handler.PrimitiveClassReflector();
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

		public override bool SupportsIndex()
		{
			return true;
		}

		public sealed override bool WriteObjectBegin()
		{
			return false;
		}

		public override object WriteNew(MarshallerFamily mf, object a_object, bool topLevel
			, StatefulBuffer a_bytes, bool withIndirection, bool restoreLinkOffset)
		{
			mf._primitive.WriteNew(a_bytes.GetTransaction(), this, a_object, topLevel, a_bytes
				, withIndirection, restoreLinkOffset);
			return a_object;
		}

		public override string ToString()
		{
			return "Wraps " + i_handler.ToString() + " in YapClassPrimitive";
		}

		public override void Defrag(MarshallerFamily mf, ReaderPair readers, bool redirect
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
	}
}
