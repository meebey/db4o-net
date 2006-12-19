NativeIoAdapter for db4o.

Usage:

using Db4objects.Db4o.IO;

class Test
{
	static void Main()
	{
		Db4oFactory.Configure().Io(new NativeIoAdapter());
	}
}

And that's it.
