using System.Collections;

namespace Db4objects.Drs.Tests.Regression
{
	public class Container
	{
		public string _name;
		public IEnumerable _items;

		public Container(string name, IEnumerable items)
		{
			_name = name;
			_items = items;
		}

		public override string ToString()
		{
			return string.Format("Container({0})", _name);
		}

		public override bool Equals(object obj)
		{
			Container other = obj as Container;
			if (null == other) return false;
			return _name == other._name;
		}

		public override int GetHashCode()
		{
			return _name.GetHashCode();
		}
	}
}