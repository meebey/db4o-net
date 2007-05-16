/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when an old file format was detected
	/// and
	/// <see cref="IConfiguration.AllowVersionUpdates">IConfiguration.AllowVersionUpdates
	/// 	</see>
	/// is set to false.
	/// </summary>
	[System.Serializable]
	public class OldFormatException : Db4oException
	{
		public OldFormatException() : base(Db4objects.Db4o.Internal.Messages.OLD_DATABASE_FORMAT
			)
		{
		}
	}
}
