/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.IO;

namespace Sharpen.IO 
{

	public abstract class StreamAdaptor 
	{

		protected Stream _stream;

		public StreamAdaptor(Stream stream) 
		{
			_stream = stream;
		}

		internal Stream UnderlyingStream 
		{
			get 
			{
				return _stream;
			}
		}

		public void Close() 
		{
			_stream.Close();
		}

#if CF_1_0 || CF_2_0
		internal Stream Buffered()
		{
			return _stream;
		}

		internal Stream Buffered(int bufferSize)
		{
			return _stream;
		}
#else
		internal Stream Buffered()
		{
			return new BufferedStream(_stream);
		}

		internal Stream Buffered(int bufferSize)
		{
			return new BufferedStream(_stream, bufferSize);
		}
#endif
	}
}
