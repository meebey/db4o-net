using System.Collections.Generic;

namespace Db4objects.Db4o.Collections
{
	public interface ISet<T> : ICollection<T>
	{
		bool IsEmpty { get; }
		bool AddAll(IEnumerable<T> ts);
		bool RemoveAll(IEnumerable<T> ts);
		bool ContainsAll(IEnumerable<T> ts);
	}
}
