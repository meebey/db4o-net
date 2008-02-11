/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// db4o-specific exception.&lt;br&gt;&lt;br&gt;
	/// This exception is thrown when an old file format was detected
	/// and
	/// <see cref="IConfiguration.AllowVersionUpdates">IConfiguration.AllowVersionUpdates
	/// 	</see>
	/// is set to false.
	/// </summary>
	[System.Serializable]
	public class OldFormatException : Db4oException
	{
		/// <summary>Constructor with the default message.</summary>
		/// <remarks>Constructor with the default message.</remarks>
		public OldFormatException() : base(Db4objects.Db4o.Internal.Messages.OldDatabaseFormat
			)
		{
		}
	}
}
