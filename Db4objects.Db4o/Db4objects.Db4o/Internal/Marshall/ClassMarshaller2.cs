using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class ClassMarshaller2 : ClassMarshaller
	{
		protected override void ReadIndex(ObjectContainerBase stream, ClassMetadata clazz
			, Db4objects.Db4o.Internal.Buffer reader)
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
