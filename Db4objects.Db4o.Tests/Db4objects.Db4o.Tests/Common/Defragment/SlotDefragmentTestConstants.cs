/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public abstract class SlotDefragmentTestConstants
	{
		public static readonly string Filename = Path.GetTempFileName();

		public static readonly string Backupfilename = Filename + ".backup";

		private SlotDefragmentTestConstants()
		{
		}
	}
}
