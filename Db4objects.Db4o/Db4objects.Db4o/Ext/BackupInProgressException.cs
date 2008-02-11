/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>db4o-specific exception.</summary>
	/// <remarks>
	/// db4o-specific exception. &lt;br&gt;&lt;br&gt;
	/// This exception is thrown when the current
	/// <see cref="IExtObjectContainer.Backup">backup</see>
	/// process encounters another backup process already running.
	/// </remarks>
	[System.Serializable]
	public class BackupInProgressException : Db4oException
	{
	}
}
