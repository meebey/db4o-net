using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ObjectMarshallerTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new ObjectMarshallerTestCase().RunSoloAndClientServer();
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

		public class ItemMarshaller : IObjectMarshaller
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
				ObjectMarshallerTestCase.Item item = (ObjectMarshallerTestCase.Item)obj;
				PrimitiveCodec.WriteInt(slot, offset, item._one);
				offset += PrimitiveCodec.INT_LENGTH;
				PrimitiveCodec.WriteLong(slot, offset, item._two);
				offset += PrimitiveCodec.LONG_LENGTH;
				PrimitiveCodec.WriteInt(slot, offset, item._three);
			}

			public virtual void ReadFields(object obj, byte[] slot, int offset)
			{
				readCalled = true;
				ObjectMarshallerTestCase.Item item = (ObjectMarshallerTestCase.Item)obj;
				item._one = PrimitiveCodec.ReadInt(slot, offset);
				offset += PrimitiveCodec.INT_LENGTH;
				item._two = PrimitiveCodec.ReadLong(slot, offset);
				offset += PrimitiveCodec.LONG_LENGTH;
				item._three = PrimitiveCodec.ReadInt(slot, offset);
			}

			public virtual int MarshalledFieldLength()
			{
				return PrimitiveCodec.INT_LENGTH * 2 + PrimitiveCodec.LONG_LENGTH;
			}
		}

		public static readonly ObjectMarshallerTestCase.ItemMarshaller marshaller = new ObjectMarshallerTestCase.ItemMarshaller
			();

		protected override void Configure(IConfiguration config)
		{
			base.Configure(config);
			config.ObjectClass(typeof(ObjectMarshallerTestCase.Item)).MarshallWith(marshaller
				);
		}

		protected override void Store()
		{
			marshaller.Reset();
			Store(new ObjectMarshallerTestCase.Item(int.MaxValue, long.MaxValue, 1));
			Assert.IsTrue(marshaller.writeCalled);
		}

		public virtual void TestReadWrite()
		{
			ObjectMarshallerTestCase.Item item = AssertRetrieve();
			Assert.IsTrue(marshaller.readCalled);
			marshaller.Reset();
			Db().Set(item);
			Assert.IsTrue(marshaller.writeCalled);
			Defragment();
			AssertRetrieve();
		}

		public virtual void TestQueryByExample()
		{
			IObjectSet os = Db().Get(new ObjectMarshallerTestCase.Item());
			Assert.AreEqual(1, os.Size());
			ObjectMarshallerTestCase.Item item = (ObjectMarshallerTestCase.Item)os.Next();
			AssertItem(item);
		}

		private ObjectMarshallerTestCase.Item AssertRetrieve()
		{
			marshaller.Reset();
			ObjectMarshallerTestCase.Item item = (ObjectMarshallerTestCase.Item)RetrieveOnlyInstance
				(typeof(ObjectMarshallerTestCase.Item));
			AssertItem(item);
			return item;
		}

		private void AssertItem(ObjectMarshallerTestCase.Item item)
		{
			Assert.AreEqual(int.MaxValue, item._one);
			Assert.AreEqual(long.MaxValue, item._two);
			Assert.AreEqual(1, item._three);
		}
	}
}
