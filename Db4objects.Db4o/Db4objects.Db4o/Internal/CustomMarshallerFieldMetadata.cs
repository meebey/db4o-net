namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	internal sealed class CustomMarshallerFieldMetadata : Db4objects.Db4o.Internal.FieldMetadata
	{
		private readonly Db4objects.Db4o.Config.IObjectMarshaller _marshaller;

		public CustomMarshallerFieldMetadata(Db4objects.Db4o.Internal.ClassMetadata containingClass
			, Db4objects.Db4o.Config.IObjectMarshaller marshaller) : base(containingClass, marshaller
			)
		{
			_marshaller = marshaller;
		}

		public override void CalculateLengths(Db4objects.Db4o.Internal.Transaction trans, 
			Db4objects.Db4o.Internal.Marshall.ObjectHeaderAttributes header, object obj)
		{
			header.AddBaseLength(LinkLength());
		}

		public override void DefragField(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.ReaderPair readers)
		{
			readers.IncrementOffset(LinkLength());
		}

		public override void Delete(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf
			, Db4objects.Db4o.Internal.StatefulBuffer a_bytes, bool isUpdate)
		{
			IncrementOffset(a_bytes);
		}

		public override bool HasIndex()
		{
			return false;
		}

		public override object GetOrCreate(Db4objects.Db4o.Internal.Transaction trans, object
			 onObject)
		{
			return onObject;
		}

		public override void Set(object onObject, object obj)
		{
		}

		public override void Instantiate(Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Internal.ObjectReference @ref, object onObject, Db4objects.Db4o.Internal.StatefulBuffer
			 reader)
		{
			_marshaller.ReadFields(onObject, reader._buffer, reader._offset);
			IncrementOffset(reader);
		}

		public override void Marshall(Db4objects.Db4o.Internal.ObjectReference yo, object
			 obj, Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, Db4objects.Db4o.Internal.StatefulBuffer
			 writer, Db4objects.Db4o.Internal.Config4Class config, bool isNew)
		{
			_marshaller.WriteFields(obj, writer._buffer, writer._offset);
			IncrementOffset(writer);
		}

		public override int LinkLength()
		{
			return _marshaller.MarshalledFieldLength();
		}
	}
}
