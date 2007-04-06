using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IReadWriteable : IReadable
	{
		void Write(Db4objects.Db4o.Internal.Buffer a_writer);
	}
}
