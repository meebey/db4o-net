/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Sharpen;
using Sharpen.Util;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Unobfuscated
	{
		private static readonly Random _random = new Random();

		public static bool CreateDb4oList(object a_stream)
		{
			((ObjectContainerBase)a_stream).CheckClosed();
			return !((ObjectContainerBase)a_stream).IsInstantiating();
		}

		public static byte[] GenerateSignature()
		{
			StatefulBuffer writer = new StatefulBuffer(null, 300);
			writer.WriteLong(Runtime.CurrentTimeMillis());
			writer.WriteLong(RandomLong());
			writer.WriteLong(RandomLong() + 1);
			return writer.GetWrittenBytes();
		}

		public static long RandomLong()
		{
			return _random.NextLong();
		}
	}
}
