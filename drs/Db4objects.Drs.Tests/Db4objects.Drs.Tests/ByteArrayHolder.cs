/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class ByteArrayHolder : Db4objects.Drs.Tests.IIByteArrayHolder
	{
		private byte[] _bytes;

		public ByteArrayHolder()
		{
		}

		public ByteArrayHolder(byte[] bytes)
		{
			this._bytes = bytes;
		}

		public virtual byte[] GetBytes()
		{
			return _bytes;
		}

		public virtual void SetBytes(byte[] bytes)
		{
			_bytes = bytes;
		}
	}
}
