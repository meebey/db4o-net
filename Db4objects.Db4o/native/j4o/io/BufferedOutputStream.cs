/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o;

namespace Sharpen.IO {

	public class BufferedOutputStream : OutputStream {

		public BufferedOutputStream(OutputStream stream) : base(stream.Buffered()) {
		}

		public BufferedOutputStream(OutputStream stream, int bufferSize) : base(stream.Buffered(bufferSize)) {
		}

	}
}
