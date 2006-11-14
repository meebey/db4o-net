namespace Db4objects.Db4o.Tests.Common.Types.Arrays
{
	public class ByteArrayTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public interface IIByteArrayHolder
		{
			byte[] GetBytes();
		}

		[System.Serializable]
		public class SerializableByteArrayHolder : Db4objects.Db4o.Tests.Common.Types.Arrays.ByteArrayTestCase.IIByteArrayHolder
		{
			private const long serialVersionUID = 1L;

			internal byte[] _bytes;

			public SerializableByteArrayHolder(byte[] bytes)
			{
				this._bytes = bytes;
			}

			public virtual byte[] GetBytes()
			{
				return _bytes;
			}
		}

		public class ByteArrayHolder : Db4objects.Db4o.Tests.Common.Types.Arrays.ByteArrayTestCase.IIByteArrayHolder
		{
			public byte[] _bytes;

			public ByteArrayHolder(byte[] bytes)
			{
				this._bytes = bytes;
			}

			public virtual byte[] GetBytes()
			{
				return _bytes;
			}
		}

		internal const int INSTANCES = 2;

		internal const int ARRAY_LENGTH = 1024 * 512;

		#if !CF_1_0 && !CF_2_0
		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.ByteArrayTestCase.SerializableByteArrayHolder)
				).Translate(new Db4objects.Db4o.Config.TSerializable());
		}
		#endif // !CF_1_0 && !CF_2_0

		protected override void Store()
		{
			for (int i = 0; i < INSTANCES; ++i)
			{
				Db().Set(new Db4objects.Db4o.Tests.Common.Types.Arrays.ByteArrayTestCase.ByteArrayHolder
					(CreateByteArray()));
				Db().Set(new Db4objects.Db4o.Tests.Common.Types.Arrays.ByteArrayTestCase.SerializableByteArrayHolder
					(CreateByteArray()));
			}
		}

		#if !CF_1_0 && !CF_2_0
		public virtual void TestByteArrayHolder()
		{
			TimeQueryLoop("raw byte array", typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.ByteArrayTestCase.ByteArrayHolder)
				);
		}
		#endif // !CF_1_0 && !CF_2_0

		#if !CF_1_0 && !CF_2_0
		public virtual void TestSerializableByteArrayHolder()
		{
			TimeQueryLoop("TSerializable", typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.ByteArrayTestCase.SerializableByteArrayHolder)
				);
		}
		#endif // !CF_1_0 && !CF_2_0

		private void TimeQueryLoop(string label, System.Type clazz)
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(clazz);
			Db4objects.Db4o.IObjectSet os = query.Execute();
			Db4oUnit.Assert.AreEqual(INSTANCES, os.Size());
			while (os.HasNext())
			{
				Db4oUnit.Assert.AreEqual(ARRAY_LENGTH, ((Db4objects.Db4o.Tests.Common.Types.Arrays.ByteArrayTestCase.IIByteArrayHolder
					)os.Next()).GetBytes().Length, label);
			}
		}

		internal virtual byte[] CreateByteArray()
		{
			byte[] bytes = new byte[ARRAY_LENGTH];
			for (int i = 0; i < bytes.Length; ++i)
			{
				bytes[i] = (byte)(i % 256);
			}
			return bytes;
		}
	}
}
