namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IReadWriteable : Db4objects.Db4o.Internal.IReadable
	{
		void Write(Db4objects.Db4o.Internal.Buffer a_writer);
	}
}
