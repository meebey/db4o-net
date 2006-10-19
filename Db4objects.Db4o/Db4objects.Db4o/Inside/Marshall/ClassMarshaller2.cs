namespace Db4objects.Db4o.Inside.Marshall
{
	/// <exclude></exclude>
	public class ClassMarshaller2 : Db4objects.Db4o.Inside.Marshall.ClassMarshaller
	{
		protected override void ReadIndex(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.YapClass
			 clazz, Db4objects.Db4o.YapReader reader)
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
