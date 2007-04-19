using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal
{
	internal class ShutDownRunnable : Collection4, IRunnable
	{
		public volatile bool dontRemove = false;

		public virtual void Run()
		{
			dontRemove = true;
			Collection4 copy = new Collection4(this);
			IEnumerator i = copy.GetEnumerator();
			while (i.MoveNext())
			{
				((ObjectContainerBase)i.Current).ShutdownHook();
			}
		}
	}
}
