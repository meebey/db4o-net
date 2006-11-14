namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class YapStringTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public virtual void TestIndexMarshalling()
		{
			Db4objects.Db4o.YapReader reader = new Db4objects.Db4o.YapReader(2 * Db4objects.Db4o.YapConst
				.INT_LENGTH);
			Db4objects.Db4o.YapStream stream = (Db4objects.Db4o.YapStream)Db();
			Db4objects.Db4o.YapString handler = new Db4objects.Db4o.YapString(stream, stream.
				StringIO());
			Db4objects.Db4o.Inside.Slots.Slot original = new Db4objects.Db4o.Inside.Slots.Slot
				(unchecked((int)(0xdb)), unchecked((int)(0x40)));
			handler.WriteIndexEntry(reader, original);
			reader._offset = 0;
			Db4objects.Db4o.Inside.Slots.Slot retrieved = (Db4objects.Db4o.Inside.Slots.Slot)
				handler.ReadIndexEntry(reader);
			Db4oUnit.Assert.AreEqual(original._address, retrieved._address);
			Db4oUnit.Assert.AreEqual(original._length, retrieved._length);
		}
	}
}
