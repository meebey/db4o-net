/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o;

namespace Sharpen.IO {

	public class BufferedInputStream : InputStream {

		public BufferedInputStream(InputStream stream) : base(stream.Buffered()) {
		}

		public BufferedInputStream(InputStream stream, int bufferSize) : base(stream.Buffered(bufferSize)) {
		}

	}
}
