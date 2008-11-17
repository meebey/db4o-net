
using System.Collections;

namespace Sharpen.Util
{
	public class Collections
	{
		public static object Remove(IDictionary dictionary, object key)
		{
			object removed = dictionary[key];
			dictionary.Remove(key);
			return removed;
		}
	}
}
