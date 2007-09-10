/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	public sealed class UntypedFieldHandler : ClassMetadata
	{
		public UntypedFieldHandler(ObjectContainerBase stream) : base(stream, stream._handlers
			.ICLASS_OBJECT)
		{
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

		public override bool HasClassIndex()
		{
			return false;
		}

		public override bool HoldsAnyClass()
		{
			return true;
		}

		public override bool IsStrongTyped()
		{
			return false;
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

		public override void Defrag(MarshallerFamily mf, BufferPair readers, bool redirect
			)
		{
			if (mf._untyped.UseNormalClassRead())
			{
				base.Defrag(mf, readers, redirect);
			}
			mf._untyped.Defrag(readers);
		}

		public override object Read(IReadContext context)
		{
			return ((UnmarshallingContext)context).ReadAny();
		}

		public override void Write(IWriteContext context, object obj)
		{
			((MarshallingContext)context).WriteAny(obj);
		}
	}
}
