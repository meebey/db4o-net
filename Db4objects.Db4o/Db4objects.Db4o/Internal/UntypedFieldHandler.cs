using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	public sealed class UntypedFieldHandler : ClassMetadata
	{
		public UntypedFieldHandler(ObjectContainerBase stream) : base(stream, stream.i_handlers
			.ICLASS_OBJECT)
		{
		}

		public override bool CanHold(IReflectClass claxx)
		{
			return true;
		}

		public override void CascadeActivation(Transaction a_trans, object a_object, int 
			a_depth, bool a_activate)
		{
			ClassMetadata yc = ForObject(a_trans, a_object, false);
			if (yc != null)
			{
				yc.CascadeActivation(a_trans, a_object, a_depth, a_activate);
			}
		}

		public override void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer reader)
		{
			mf._untyped.DeleteEmbedded(reader);
		}

		public override int GetID()
		{
			return 11;
		}

		public override bool HasField(ObjectContainerBase a_stream, string a_path)
		{
			return a_stream.ClassCollection().FieldExists(a_path);
		}

		public override bool HasIndex()
		{
			return false;
		}

		public override bool HasFixedLength()
		{
			return false;
		}

		public override bool HoldsAnyClass()
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
			if (topLevel)
			{
				header.AddBaseLength(Const4.INT_LENGTH);
			}
			else
			{
				header.AddPayLoadLength(Const4.INT_LENGTH);
			}
			ClassMetadata yc = ForObject(trans, obj, true);
			if (yc == null)
			{
				return;
			}
			header.AddPayLoadLength(Const4.INT_LENGTH);
			yc.CalculateLengths(trans, header, false, obj, false);
		}

		public override object Read(MarshallerFamily mf, StatefulBuffer a_bytes, bool redirect
			)
		{
			if (mf._untyped.UseNormalClassRead())
			{
				return base.Read(mf, a_bytes, redirect);
			}
			return mf._untyped.Read(a_bytes);
		}

		public override ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer[] a_bytes)
		{
			return mf._untyped.ReadArrayHandler(a_trans, a_bytes);
		}

		public override object ReadQuery(Transaction trans, MarshallerFamily mf, bool withRedirection
			, Db4objects.Db4o.Internal.Buffer reader, bool toArray)
		{
			if (mf._untyped.UseNormalClassRead())
			{
				return base.ReadQuery(trans, mf, withRedirection, reader, toArray);
			}
			return mf._untyped.ReadQuery(trans, reader, toArray);
		}

		public override QCandidate ReadSubCandidate(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates, bool withIndirection)
		{
			if (mf._untyped.UseNormalClassRead())
			{
				return base.ReadSubCandidate(mf, reader, candidates, withIndirection);
			}
			return mf._untyped.ReadSubCandidate(reader, candidates, withIndirection);
		}

		public override bool SupportsIndex()
		{
			return false;
		}

		public override object WriteNew(MarshallerFamily mf, object obj, bool topLevel, StatefulBuffer
			 writer, bool withIndirection, bool restoreLinkeOffset)
		{
			return mf._untyped.WriteNew(obj, restoreLinkeOffset, writer);
		}

		public override void Defrag(MarshallerFamily mf, ReaderPair readers, bool redirect
			)
		{
			if (mf._untyped.UseNormalClassRead())
			{
				base.Defrag(mf, readers, redirect);
			}
			mf._untyped.Defrag(readers);
		}
	}
}
