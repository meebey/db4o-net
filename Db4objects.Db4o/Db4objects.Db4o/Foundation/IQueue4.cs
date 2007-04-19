using System.Collections;

namespace Db4objects.Db4o.Foundation
{
	public interface IQueue4
	{
		void Add(object obj);

		object Next();

		bool HasNext();

		IEnumerator Iterator();
	}
}
