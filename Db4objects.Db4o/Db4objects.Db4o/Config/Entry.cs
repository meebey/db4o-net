namespace Db4objects.Db4o.Config
{
	/// <exclude></exclude>
	public class Entry : Db4objects.Db4o.Config.ICompare, Db4objects.Db4o.Types.ISecondClass
	{
		public object key;

		public object value;

		public virtual object Compare()
		{
			return key;
		}
	}
}
