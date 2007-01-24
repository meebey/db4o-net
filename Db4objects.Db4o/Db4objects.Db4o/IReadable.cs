namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public interface IReadable
	{
		object Read(Db4objects.Db4o.YapReader a_reader);

		int ByteCount();
	}
}
