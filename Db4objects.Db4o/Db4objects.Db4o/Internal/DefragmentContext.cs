/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	public class DefragmentContext
	{
		private Db4objects.Db4o.Internal.Marshall.MarshallerFamily _mf;

		private BufferPair _readers;

		internal bool _redirect;

		public DefragmentContext(Db4objects.Db4o.Internal.Marshall.MarshallerFamily mf, BufferPair
			 readers, bool redirect)
		{
			_mf = mf;
			_readers = readers;
			_redirect = redirect;
		}

		public virtual Db4objects.Db4o.Internal.Marshall.MarshallerFamily MarshallerFamily
			()
		{
			return _mf;
		}

		public virtual BufferPair Readers()
		{
			return _readers;
		}

		public virtual bool Redirect()
		{
			return _redirect;
		}
	}
}
