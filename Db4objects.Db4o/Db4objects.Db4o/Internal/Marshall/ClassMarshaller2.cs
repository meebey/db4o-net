namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class ClassMarshaller2 : Db4objects.Db4o.Internal.Marshall.ClassMarshaller
	{
		protected override void ReadIndex(Db4objects.Db4o.Internal.ObjectContainerBase stream
			, Db4objects.Db4o.Internal.ClassMetadata clazz, Db4objects.Db4o.Internal.Buffer 
			reader)
		{
			int indexID = reader.ReadInt();
			clazz.Index().Read(stream, indexID);
		}

		protected override int IndexIDForWriting(int indexID)
		{
			return indexID;
		}
	}
}
