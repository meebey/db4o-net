namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Unobfuscated
	{
		private static readonly Sharpen.Util.Random _random = new Sharpen.Util.Random();

		public static bool CreateDb4oList(object a_stream)
		{
			((Db4objects.Db4o.Internal.ObjectContainerBase)a_stream).CheckClosed();
			return !((Db4objects.Db4o.Internal.ObjectContainerBase)a_stream).IsInstantiating(
				);
		}

		public static byte[] GenerateSignature()
		{
			Db4objects.Db4o.Internal.StatefulBuffer writer = new Db4objects.Db4o.Internal.StatefulBuffer
				(null, 300);
			writer.WriteLong(Sharpen.Runtime.CurrentTimeMillis());
			writer.WriteLong(RandomLong());
			writer.WriteLong(RandomLong() + 1);
			return writer.GetWrittenBytes();
		}

		internal static void PurgeUnsychronized(object a_stream, object a_object)
		{
			((Db4objects.Db4o.Internal.ObjectContainerBase)a_stream).Purge1(a_object);
		}

		public static long RandomLong()
		{
			return _random.NextLong();
		}
	}
}
