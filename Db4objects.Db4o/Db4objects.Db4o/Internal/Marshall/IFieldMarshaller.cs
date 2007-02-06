namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public interface IFieldMarshaller
	{
		void Write(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.ClassMetadata
			 clazz, Db4objects.Db4o.Internal.FieldMetadata field, Db4objects.Db4o.Internal.Buffer
			 writer);

		Db4objects.Db4o.Internal.Marshall.RawFieldSpec ReadSpec(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, Db4objects.Db4o.Internal.Buffer reader);

		Db4objects.Db4o.Internal.FieldMetadata Read(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, Db4objects.Db4o.Internal.FieldMetadata field, Db4objects.Db4o.Internal.Buffer
			 reader);

		int MarshalledLength(Db4objects.Db4o.Internal.ObjectContainerBase stream, Db4objects.Db4o.Internal.FieldMetadata
			 field);

		void Defrag(Db4objects.Db4o.Internal.ClassMetadata yapClass, Db4objects.Db4o.Internal.FieldMetadata
			 yapField, Db4objects.Db4o.Internal.LatinStringIO sio, Db4objects.Db4o.Internal.ReaderPair
			 readers);
	}
}
