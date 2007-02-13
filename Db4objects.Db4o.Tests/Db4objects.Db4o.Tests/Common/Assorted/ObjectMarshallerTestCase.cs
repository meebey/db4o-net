namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ObjectMarshallerTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase().RunSoloAndClientServer
				();
		}

		public class Item
		{
			public int _one;

			public long _two;

			public int _three;

			public Item(int one, long two, int three)
			{
				_one = one;
				_two = two;
				_three = three;
			}

			public Item()
			{
			}
		}

		public class ItemMarshaller : Db4objects.Db4o.Config.IObjectMarshaller
		{
			public bool readCalled;

			public bool writeCalled;

			public virtual void Reset()
			{
				readCalled = false;
				writeCalled = false;
			}

			public virtual void WriteFields(object obj, byte[] slot, int offset)
			{
				writeCalled = true;
				Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.Item item = (Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.Item
					)obj;
				Db4objects.Db4o.Foundation.PrimitiveCodec.WriteInt(slot, offset, item._one);
				offset += Db4objects.Db4o.Foundation.PrimitiveCodec.INT_LENGTH;
				Db4objects.Db4o.Foundation.PrimitiveCodec.WriteLong(slot, offset, item._two);
				offset += Db4objects.Db4o.Foundation.PrimitiveCodec.LONG_LENGTH;
				Db4objects.Db4o.Foundation.PrimitiveCodec.WriteInt(slot, offset, item._three);
			}

			public virtual void ReadFields(object obj, byte[] slot, int offset)
			{
				readCalled = true;
				Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.Item item = (Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.Item
					)obj;
				item._one = Db4objects.Db4o.Foundation.PrimitiveCodec.ReadInt(slot, offset);
				offset += Db4objects.Db4o.Foundation.PrimitiveCodec.INT_LENGTH;
				item._two = Db4objects.Db4o.Foundation.PrimitiveCodec.ReadLong(slot, offset);
				offset += Db4objects.Db4o.Foundation.PrimitiveCodec.LONG_LENGTH;
				item._three = Db4objects.Db4o.Foundation.PrimitiveCodec.ReadInt(slot, offset);
			}

			public virtual int MarshalledFieldLength()
			{
				return Db4objects.Db4o.Foundation.PrimitiveCodec.INT_LENGTH * 2 + Db4objects.Db4o.Foundation.PrimitiveCodec
					.LONG_LENGTH;
			}
		}

		public static readonly Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.ItemMarshaller
			 marshaller = new Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.ItemMarshaller
			();

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			base.Configure(config);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.Item)
				).MarshallWith(marshaller);
		}

		protected override void Store()
		{
			marshaller.Reset();
			Store(new Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.Item(int.MaxValue
				, long.MaxValue, 1));
			Db4oUnit.Assert.IsTrue(marshaller.writeCalled);
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.Item item = AssertRetrieve
				();
			Db4oUnit.Assert.IsTrue(marshaller.readCalled);
			marshaller.Reset();
			Db().Set(item);
			Db4oUnit.Assert.IsTrue(marshaller.writeCalled);
			Defragment();
			AssertRetrieve();
		}

		private Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.Item AssertRetrieve
			()
		{
			marshaller.Reset();
			Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.Item item = (Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.Item
				)RetrieveOnlyInstance(typeof(Db4objects.Db4o.Tests.Common.Assorted.ObjectMarshallerTestCase.Item)
				);
			Db4oUnit.Assert.AreEqual(int.MaxValue, item._one);
			Db4oUnit.Assert.AreEqual(long.MaxValue, item._two);
			Db4oUnit.Assert.AreEqual(1, item._three);
			return item;
		}
	}
}
