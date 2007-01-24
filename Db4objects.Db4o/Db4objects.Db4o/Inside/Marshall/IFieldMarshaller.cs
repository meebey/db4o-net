namespace Db4objects.Db4o.Inside.Marshall
{
	/// <exclude></exclude>
	public interface IFieldMarshaller
	{
		void Write(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapClass clazz, Db4objects.Db4o.YapField
			 field, Db4objects.Db4o.YapReader writer);

		Db4objects.Db4o.Inside.Marshall.RawFieldSpec ReadSpec(Db4objects.Db4o.YapStream stream
			, Db4objects.Db4o.YapReader reader);

		Db4objects.Db4o.YapField Read(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapField
			 field, Db4objects.Db4o.YapReader reader);

		int MarshalledLength(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapField field
			);

		void Defrag(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.YapField yapField, 
			Db4objects.Db4o.YapStringIO sio, Db4objects.Db4o.ReaderPair readers);
	}
}
