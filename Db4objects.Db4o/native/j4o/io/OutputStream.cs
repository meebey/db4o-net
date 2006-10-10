/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.IO;

namespace Sharpen.IO {

	public class OutputStream : StreamAdaptor {

		public OutputStream(Stream stream) : base(stream) {
		}

		public void Write(int b) {
			_stream.WriteByte((byte) b);
		}

		public void Write(byte[] bytes) {
			_stream.Write(bytes, 0, bytes.Length);
		}

        public void Write(byte[] bytes, int offset, int length) {
            _stream.Write(bytes, offset, length);
        }

		public void Flush() {
			_stream.Flush();
		}
	}
}
