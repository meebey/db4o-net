using System;
using System.Collections;
using System.Collections.Generic;

namespace Sharpen.Util
{
	public interface ISet : System.Collections.ICollection
	{
		void Add(object value);
		bool Contains(object value);
	}

	public class HashSet : ISet
	{
		private IDictionary<object, bool> _set = new Dictionary<object, bool>();

		public void Add(object value)
		{
			if (_set.ContainsKey(value)) return;
			_set.Add(value, true);
		}

		public bool Contains(object value)
		{
			return _set.ContainsKey(value);
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		public object SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsSynchronized
		{
			get { throw new NotImplementedException(); }
		}

		public IEnumerator GetEnumerator()
		{
			return _set.Keys.GetEnumerator();
		}
	}
}
