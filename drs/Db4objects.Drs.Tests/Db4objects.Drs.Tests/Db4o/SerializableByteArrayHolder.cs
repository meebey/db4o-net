/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests.Db4o
{
	[System.Serializable]
	public class SerializableByteArrayHolder : IIByteArrayHolder
	{
		private const long serialVersionUID = 1L;

		private byte[] _bytes;

		public SerializableByteArrayHolder()
		{
		}

		public SerializableByteArrayHolder(byte[] bytes)
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
