namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class StaticField : Db4objects.Db4o.IInternal4
	{
		public string name;

		public object value;

		public StaticField()
		{
		}

		public StaticField(string name_, object value_)
		{
			name = name_;
			value = value_;
		}
	}
}
