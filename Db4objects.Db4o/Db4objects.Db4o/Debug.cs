/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public abstract class Debug : Debug4
	{
		public const bool indexAllFields = false;

		public const bool queries = false;

		public const bool atHome = false;

		public const bool longTimeOuts = false;

		public const bool freespace = Deploy.debug;

		public const bool xbytes = freespace;

		public const bool checkSychronization = false;

		public const bool configureAllClasses = indexAllFields;

		public const bool configureAllFields = indexAllFields;

		public const bool weakReferences = true;

		public const bool messages = false;

		public const bool nio = true;

		public const bool lockFile = true;

		public static void Expect(bool cond)
		{
			if (!cond)
			{
				throw new Exception("Should never happen");
			}
		}

		public static void EnsureLock(object obj)
		{
		}

		public static bool ExceedsMaximumBlockSize(int a_length)
		{
			if (a_length > Const4.MAXIMUM_BLOCK_SIZE)
			{
				return true;
			}
			return false;
		}

		public static bool ExceedsMaximumArrayEntries(int a_entries, bool a_primitive)
		{
			if (a_entries > (a_primitive ? Const4.MAXIMUM_ARRAY_ENTRIES_PRIMITIVE : Const4.MAXIMUM_ARRAY_ENTRIES
				))
			{
				return true;
			}
			return false;
		}
	}
}
