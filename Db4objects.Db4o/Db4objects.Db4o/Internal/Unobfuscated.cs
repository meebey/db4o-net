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

		internal static void PurgeUnsychronized(object a_stream, object a_object)
		{
			((ObjectContainerBase)a_stream).Purge1(a_object);
		}

		public static long RandomLong()
		{
			return _random.NextLong();
		}
	}
}
