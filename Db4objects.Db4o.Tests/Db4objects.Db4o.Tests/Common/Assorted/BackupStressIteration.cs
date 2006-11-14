namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class BackupStressIteration
	{
		public int _count;

		public BackupStressIteration()
		{
		}

		public virtual void SetCount(int count)
		{
			_count = count;
		}

		public virtual int GetCount()
		{
			return _count;
		}
	}
}
