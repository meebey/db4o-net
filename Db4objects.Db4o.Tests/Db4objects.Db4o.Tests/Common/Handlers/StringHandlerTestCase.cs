namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class StringHandlerTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public virtual void TestIndexMarshalling()
		{
			Db4objects.Db4o.Internal.Buffer reader = new Db4objects.Db4o.Internal.Buffer(2 * 
				Db4objects.Db4o.Internal.Const4.INT_LENGTH);
			Db4objects.Db4o.Internal.ObjectContainerBase stream = (Db4objects.Db4o.Internal.ObjectContainerBase
				)Db();
			Db4objects.Db4o.Internal.Handlers.StringHandler handler = new Db4objects.Db4o.Internal.Handlers.StringHandler
				(stream, stream.StringIO());
			Db4objects.Db4o.Internal.Slots.Slot original = new Db4objects.Db4o.Internal.Slots.Slot
				(unchecked((int)(0xdb)), unchecked((int)(0x40)));
			handler.WriteIndexEntry(reader, original);
			reader._offset = 0;
			Db4objects.Db4o.Internal.Slots.Slot retrieved = (Db4objects.Db4o.Internal.Slots.Slot
				)handler.ReadIndexEntry(reader);
			Db4oUnit.Assert.AreEqual(original._address, retrieved._address);
			Db4oUnit.Assert.AreEqual(original._length, retrieved._length);
		}
	}
}
