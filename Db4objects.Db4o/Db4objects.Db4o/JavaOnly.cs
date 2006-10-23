namespace Db4objects.Db4o
{
	internal class JavaOnly
	{
		internal static int CollectionUpdateDepth(System.Type a_class)
		{
			return 0;
		}

		internal static bool IsCollection(System.Type a_class)
		{
			return false;
		}

		internal static bool IsCollectionTranslator(Db4objects.Db4o.Config4Class a_config
			)
		{
			return false;
		}

		public static Db4objects.Db4o.JDK Jdk()
		{
			return new Db4objects.Db4o.JDK();
		}

		public static void Link()
		{
		}

		public static void RunFinalizersOnExit()
		{
		}

		internal static readonly System.Type[] SIMPLE_CLASSES = null;
	}
}
