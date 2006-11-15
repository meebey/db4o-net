namespace Db4objects.Db4o
{
	internal class ShutDownRunnable : Db4objects.Db4o.Foundation.Collection4, Sharpen.Lang.IRunnable
	{
		public volatile bool dontRemove = false;

		public virtual void Run()
		{
			dontRemove = true;
			Db4objects.Db4o.Foundation.Collection4 copy = new Db4objects.Db4o.Foundation.Collection4
				(this);
			System.Collections.IEnumerator i = copy.GetEnumerator();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.YapStream)i.Current).FailedToShutDown();
			}
		}
	}
}
