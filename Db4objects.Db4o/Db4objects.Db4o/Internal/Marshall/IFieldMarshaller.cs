using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public interface IFieldMarshaller
	{
		void Write(Transaction trans, ClassMetadata clazz, FieldMetadata field, Db4objects.Db4o.Internal.Buffer
			 writer);

		RawFieldSpec ReadSpec(ObjectContainerBase stream, Db4objects.Db4o.Internal.Buffer
			 reader);

		FieldMetadata Read(ObjectContainerBase stream, FieldMetadata field, Db4objects.Db4o.Internal.Buffer
			 reader);

		int MarshalledLength(ObjectContainerBase stream, FieldMetadata field);

		void Defrag(ClassMetadata yapClass, FieldMetadata yapField, LatinStringIO sio, ReaderPair
			 readers);
	}
}
