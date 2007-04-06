using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	internal sealed class CustomMarshallerFieldMetadata : FieldMetadata
	{
		private readonly IObjectMarshaller _marshaller;

		public CustomMarshallerFieldMetadata(ClassMetadata containingClass, IObjectMarshaller
			 marshaller) : base(containingClass, marshaller)
		{
			_marshaller = marshaller;
		}

		public override void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, object obj)
		{
			header.AddBaseLength(LinkLength());
		}

		public override void DefragField(MarshallerFamily mf, ReaderPair readers)
		{
			readers.IncrementOffset(LinkLength());
		}

		public override void Delete(MarshallerFamily mf, StatefulBuffer a_bytes, bool isUpdate
			)
		{
			IncrementOffset(a_bytes);
		}

		public override bool HasIndex()
		{
			return false;
		}

		public override object GetOn(Transaction a_trans, object obj)
		{
			return obj;
		}

		public override object GetOrCreate(Transaction trans, object onObject)
		{
			return onObject;
		}

		public override void Set(object onObject, object obj)
		{
		}

		public override void Instantiate(MarshallerFamily mf, ObjectReference @ref, object
			 onObject, StatefulBuffer reader)
		{
			_marshaller.ReadFields(onObject, reader._buffer, reader._offset);
			IncrementOffset(reader);
		}

		public override void Marshall(ObjectReference yo, object obj, MarshallerFamily mf
			, StatefulBuffer writer, Config4Class config, bool isNew)
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
