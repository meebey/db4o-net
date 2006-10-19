namespace Db4objects.Db4o
{
	internal class ShutDownRunnable : Db4objects.Db4o.Foundation.Collection4, Sharpen.Lang.IRunnable
	{
		public volatile bool dontRemove = false;

		public virtual void Run()
		{
			dontRemove = true;
			System.Collections.IEnumerator i = GetEnumerator();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.YapStream)i.Current).FailedToShutDown();
			}
		}
	}
}
