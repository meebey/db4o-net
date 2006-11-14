namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class SimplestPossibleItem
	{
		public string name;

		public SimplestPossibleItem()
		{
		}

		public SimplestPossibleItem(string name_)
		{
			this.name = name_;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}
	}
}
