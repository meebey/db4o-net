namespace Db4objects.Db4o
{
	internal class IDGenerator
	{
		private int id = 0;

		internal virtual int Next()
		{
			id++;
			if (id > 0)
			{
				return id;
			}
			id = 1;
			return 1;
		}
	}
}
