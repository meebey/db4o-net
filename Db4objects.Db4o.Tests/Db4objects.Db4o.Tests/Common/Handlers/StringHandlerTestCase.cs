/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class StringHandlerTestCase : AbstractDb4oTestCase
	{
		public virtual void TestIndexMarshalling()
		{
			Db4objects.Db4o.Internal.Buffer reader = new Db4objects.Db4o.Internal.Buffer(2 * 
				Const4.INT_LENGTH);
			ObjectContainerBase stream = (ObjectContainerBase)Db();
			StringHandler handler = new StringHandler(stream, stream.StringIO());
			Slot original = new Slot(unchecked((int)(0xdb)), unchecked((int)(0x40)));
			handler.WriteIndexEntry(reader, original);
			reader._offset = 0;
			Slot retrieved = (Slot)handler.ReadIndexEntry(reader);
			Assert.AreEqual(original.Address(), retrieved.Address());
			Assert.AreEqual(original.Length(), retrieved.Length());
		}
	}
}
