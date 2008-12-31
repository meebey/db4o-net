/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.IO
{
	/// <exclude></exclude>
	public class BinConfiguration
	{
		private readonly string _uri;

		private readonly bool _lockFile;

		private readonly long _initialLength;

		private readonly bool _readOnly;

		public BinConfiguration(string uri, bool lockFile, long initialLength, bool readOnly
			)
		{
			_uri = uri;
			_lockFile = lockFile;
			_initialLength = initialLength;
			_readOnly = readOnly;
		}

		public virtual string Uri()
		{
			return _uri;
		}

		public virtual bool LockFile()
		{
			return _lockFile;
		}

		public virtual long InitialLength()
		{
			return _initialLength;
		}

		public virtual bool ReadOnly()
		{
			return _readOnly;
		}
	}
}
