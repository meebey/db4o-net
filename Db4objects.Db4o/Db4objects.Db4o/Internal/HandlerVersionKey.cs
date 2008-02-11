/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Fieldhandlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class HandlerVersionKey
	{
		private readonly IFieldHandler _handler;

		private readonly int _version;

		public HandlerVersionKey(IFieldHandler handler, int version)
		{
			_handler = handler;
			_version = version;
		}

		public override int GetHashCode()
		{
			return _handler.GetHashCode() + _version * 4271;
		}

		public override bool Equals(object obj)
		{
			Db4objects.Db4o.Internal.HandlerVersionKey other = (Db4objects.Db4o.Internal.HandlerVersionKey
				)obj;
			return _handler.Equals(other._handler) && _version == other._version;
		}
	}
}
