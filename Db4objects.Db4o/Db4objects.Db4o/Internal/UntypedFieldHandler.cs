namespace Db4objects.Db4o.Internal
{
	public sealed class UntypedFieldHandler : Db4objects.Db4o.Internal.ClassMetadata
	{
		public UntypedFieldHandler(Db4objects.Db4o.Internal.ObjectContainerBase stream) : 
			base(stream, stream.i_handlers.ICLASS_OBJECT)
		{
		}

		public override bool CanHold(Db4objects.Db4o.Reflect.IReflectClass claxx)
		{
			return true;
		}

		public override void CascadeActivation(Db4objects.Db4o.Internal.Transaction a_trans
			, object a_object, int a_depth, bool a_activate)
		{
			Db4objects.Db4o.Internal.ClassMetadata yc = ForObject(a_trans, a_object, false);
			if (yc != null)
			{
				yc.CascadeActivation(a_trans, a_object, a_depth, a_activate);
			}
		}

		public override void DeleteEmbedded(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.StatefulBuffer reader)
		{
			mf._untyped.DeleteEmbedded(reader);
		}

		public override int GetID()
		{
			return 11;
		}

		public override bool HasField(Db4objects.Db4o.Internal.ObjectContainerBase a_stream
			, string a_path)
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

		public override Db4objects.Db4o.Foundation.TernaryBool IsSecondClass()
		{
			return Db4objects.Db4o.Foundation.TernaryBool.UNSPECIFIED;
		}

		public override bool IsStrongTyped()
		{
			return false;
		}

		public override void CalculateLengths(Db4objects.Db4o.Internal.Transaction trans, 
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes header, bool topLevel, 
			object obj, bool withIndirection)
		{
			if (topLevel)
			{
				header.AddBaseLength(Db4objects.Db4o.Internal.Const4.INT_LENGTH);
			}
			else
			{
				header.AddPayLoadLength(Db4objects.Db4o.Internal.Const4.INT_LENGTH);
			}
			Db4objects.Db4o.Internal.ClassMetadata yc = ForObject(trans, obj, true);
			if (yc == null)
			{
				return;
			}
			header.AddPayLoadLength(Db4objects.Db4o.Internal.Const4.INT_LENGTH);
			yc.CalculateLengths(trans, header, false, obj, false);
		}

		public override object Read(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf
			, Db4objects.Db4o.Internal.StatefulBuffer a_bytes, bool redirect)
		{
			if (mf._untyped.UseNormalClassRead())
			{
				return base.Read(mf, a_bytes, redirect);
			}
			return mf._untyped.Read(a_bytes);
		}

		public override Db4objects.Db4o.Internal.ITypeHandler4 ReadArrayHandler(Db4objects.Db4o.Internal.Transaction
			 a_trans, Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer[]
			 a_bytes)
		{
			return mf._untyped.ReadArrayHandler(a_trans, a_bytes);
		}

		public override object ReadQuery(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, bool withRedirection, Db4objects.Db4o.Internal.Buffer reader, bool toArray)
		{
			if (mf._untyped.UseNormalClassRead())
			{
				return base.ReadQuery(trans, mf, withRedirection, reader, toArray);
			}
			return mf._untyped.ReadQuery(trans, reader, toArray);
		}

		public override Db4objects.Db4o.Internal.Query.Processor.QCandidate ReadSubCandidate
			(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, Db4objects.Db4o.Internal.Query.Processor.QCandidates candidates, bool withIndirection
			)
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

		public override object WriteNew(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, object obj, bool topLevel, Db4objects.Db4o.Internal.StatefulBuffer writer, 
			bool withIndirection, bool restoreLinkeOffset)
		{
			return mf._untyped.WriteNew(obj, restoreLinkeOffset, writer);
		}

		public override void Defrag(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf
			, Db4objects.Db4o.Internal.ReaderPair readers, bool redirect)
		{
			if (mf._untyped.UseNormalClassRead())
			{
				base.Defrag(mf, readers, redirect);
			}
			mf._untyped.Defrag(readers);
		}
	}
}
