namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IReadable
	{
		object Read(Db4objects.Db4o.Internal.Buffer a_reader);

		int ByteCount();
	}
}
