namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class BackupStressItem
	{
		public string _name;

		public int _iteration;

		public BackupStressItem()
		{
		}

		public BackupStressItem(string name, int iteration)
		{
			_name = name;
			_iteration = iteration;
		}
	}
}
