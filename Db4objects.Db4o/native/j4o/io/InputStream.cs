/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.IO;

namespace Sharpen.IO {

	public class InputStream : StreamAdaptor {

		public InputStream(Stream stream) : base(stream) {
		}

		public int Available() {
			return (int)(_stream.Length - _stream.Position);
		}

		public int Read() {
			return _stream.ReadByte();
		}

        public int Read(byte[] bytes){
            int read = _stream.Read(bytes, 0, bytes.Length);
            return (0 == read) ? -1 : read;
        }

		public int Read(byte[] bytes, int offset, int length) {
            int read = _stream.Read(bytes, offset, length);
            return (0 == read) ? -1 : read;
		}
	}
}
