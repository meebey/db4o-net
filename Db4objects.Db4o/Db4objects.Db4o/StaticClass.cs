namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class StaticClass : Db4objects.Db4o.IInternal4
	{
		public string name;

		public Db4objects.Db4o.StaticField[] fields;

		public StaticClass()
		{
		}

		public StaticClass(string name_, Db4objects.Db4o.StaticField[] fields_)
		{
			name = name_;
			fields = fields_;
		}
	}
}
