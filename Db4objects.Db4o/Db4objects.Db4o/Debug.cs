namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public abstract class Debug : Db4objects.Db4o.Foundation.Debug4
	{
		public const bool indexAllFields = false;

		public const bool queries = false;

		public const bool atHome = false;

		public static bool longTimeOuts = false;

		public const bool freespace = Db4objects.Db4o.Deploy.debug ? true : false;

		public const bool xbytes = freespace ? true : false;

		public const bool freespaceChecker = false;

		public const bool checkSychronization = false;

		public const bool configureAllClasses = indexAllFields;

		public const bool configureAllFields = indexAllFields;

		public const bool weakReferences = true;

		public const bool fakeServer = false;

		public const bool messages = false;

		public const bool nio = true;

		public const bool lockFile = true;

		public static void Expect(bool cond)
		{
			if (!cond)
			{
				throw new System.Exception("Should never happen");
			}
		}

		public static void EnsureLock(object obj)
		{
		}

		public static bool ExceedsMaximumBlockSize(int a_length)
		{
			if (a_length > Db4objects.Db4o.YapConst.MAXIMUM_BLOCK_SIZE)
			{
				return true;
			}
			return false;
		}

		public static bool ExceedsMaximumArrayEntries(int a_entries, bool a_primitive)
		{
			if (a_entries > (a_primitive ? Db4objects.Db4o.YapConst.MAXIMUM_ARRAY_ENTRIES_PRIMITIVE
				 : Db4objects.Db4o.YapConst.MAXIMUM_ARRAY_ENTRIES))
			{
				return true;
			}
			return false;
		}
	}
}
