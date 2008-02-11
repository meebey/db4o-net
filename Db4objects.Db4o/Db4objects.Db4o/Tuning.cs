/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o
{
	/// <summary>Tuning switches for customized versions.</summary>
	/// <remarks>Tuning switches for customized versions.</remarks>
	/// <exclude></exclude>
	public class Tuning
	{
		[System.ObsoleteAttribute(@"Use Db4o.configure().io(new com.db4o.io.SymbianIoAdapter()) instead"
			)]
		public const bool symbianSeek = false;

		public const bool readableMessages = true;
	}
}
