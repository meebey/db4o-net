/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Tests.Common.Internal
{
	public class MarshallingBufferTestCase : ITestCase
	{
		private const int DATA_1 = 111;

		private const byte DATA_2 = (byte)2;

		private const int DATA_3 = 333;

		private const int DATA_4 = 444;

		private const int DATA_5 = 55;

		public virtual void TestWrite()
		{
			MarshallingBuffer buffer = new MarshallingBuffer();
			buffer.WriteInt(DATA_1);
			buffer.WriteByte(DATA_2);
			Db4objects.Db4o.Internal.Buffer content = InspectContent(buffer);
			Assert.AreEqual(DATA_1, content.ReadInt());
			Assert.AreEqual(DATA_2, content.ReadByte());
		}

		public virtual void TestTransferLastWrite()
		{
			MarshallingBuffer buffer = new MarshallingBuffer();
			buffer.WriteInt(DATA_1);
			int lastOffset = Offset(buffer);
			buffer.WriteByte(DATA_2);
			MarshallingBuffer other = new MarshallingBuffer();
			buffer.TransferLastWriteTo(other);
			Assert.AreEqual(lastOffset, Offset(buffer));
			Db4objects.Db4o.Internal.Buffer content = InspectContent(other);
			Assert.AreEqual(DATA_2, content.ReadByte());
		}

		private int Offset(MarshallingBuffer buffer)
		{
			return buffer.TestDelegate().Offset();
		}

		private Db4objects.Db4o.Internal.Buffer InspectContent(MarshallingBuffer buffer)
		{
			Db4objects.Db4o.Internal.Buffer bufferDelegate = buffer.TestDelegate();
			bufferDelegate.Offset(0);
			return bufferDelegate;
		}

		public virtual void TestChildren()
		{
			MarshallingBuffer buffer = new MarshallingBuffer();
			buffer.WriteInt(DATA_1);
			buffer.WriteByte(DATA_2);
			MarshallingBuffer child = buffer.AddChild(true);
			child.WriteInt(DATA_3);
			child.WriteInt(DATA_4);
			buffer.MergeChildren(0);
			Db4objects.Db4o.Internal.Buffer content = InspectContent(buffer);
			Assert.AreEqual(DATA_1, content.ReadInt());
			Assert.AreEqual(DATA_2, content.ReadByte());
			int address = content.ReadInt();
			content.Offset(address);
			Assert.AreEqual(DATA_3, content.ReadInt());
			Assert.AreEqual(DATA_4, content.ReadInt());
		}

		public virtual void TestGrandChildren()
		{
			MarshallingBuffer buffer = new MarshallingBuffer();
			buffer.WriteInt(DATA_1);
			buffer.WriteByte(DATA_2);
			MarshallingBuffer child = buffer.AddChild(true);
			child.WriteInt(DATA_3);
			child.WriteInt(DATA_4);
			MarshallingBuffer grandChild = child.AddChild(true);
			grandChild.WriteInt(DATA_5);
			buffer.MergeChildren(0);
			Db4objects.Db4o.Internal.Buffer content = InspectContent(buffer);
			Assert.AreEqual(DATA_1, content.ReadInt());
			Assert.AreEqual(DATA_2, content.ReadByte());
			int address = content.ReadInt();
			content.Offset(address);
			Assert.AreEqual(DATA_3, content.ReadInt());
			Assert.AreEqual(DATA_4, content.ReadInt());
			address = content.ReadInt();
			content.Offset(address);
			Assert.AreEqual(DATA_5, content.ReadInt());
		}

		public virtual void TestLinkOffset()
		{
			int linkOffset = 7;
			MarshallingBuffer buffer = new MarshallingBuffer();
			buffer.WriteInt(DATA_1);
			buffer.WriteByte(DATA_2);
			MarshallingBuffer child = buffer.AddChild(true);
			child.WriteInt(DATA_3);
			child.WriteInt(DATA_4);
			MarshallingBuffer grandChild = child.AddChild(true);
			grandChild.WriteInt(DATA_5);
			buffer.MergeChildren(linkOffset);
			Db4objects.Db4o.Internal.Buffer content = InspectContent(buffer);
			Db4objects.Db4o.Internal.Buffer extendedBuffer = new Db4objects.Db4o.Internal.Buffer
				(content.Length() + linkOffset);
			content.CopyTo(extendedBuffer, 0, linkOffset, content.Length());
			extendedBuffer.Offset(linkOffset);
			Assert.AreEqual(DATA_1, extendedBuffer.ReadInt());
			Assert.AreEqual(DATA_2, extendedBuffer.ReadByte());
			int address = extendedBuffer.ReadInt();
			extendedBuffer.Offset(address);
			Assert.AreEqual(DATA_3, extendedBuffer.ReadInt());
			Assert.AreEqual(DATA_4, extendedBuffer.ReadInt());
			address = extendedBuffer.ReadInt();
			extendedBuffer.Offset(address);
			Assert.AreEqual(DATA_5, extendedBuffer.ReadInt());
		}

		public virtual void TestLateChildrenWrite()
		{
			MarshallingBuffer buffer = new MarshallingBuffer();
			buffer.WriteInt(DATA_1);
			MarshallingBuffer child = buffer.AddChild(true);
			child.WriteInt(DATA_3);
			buffer.WriteByte(DATA_2);
			child.WriteInt(DATA_4);
			buffer.MergeChildren(0);
			Db4objects.Db4o.Internal.Buffer content = InspectContent(buffer);
			Assert.AreEqual(DATA_1, content.ReadInt());
			int address = content.ReadInt();
			content.ReadInt();
			Assert.AreEqual(DATA_2, content.ReadByte());
			content.Seek(address);
			Assert.AreEqual(DATA_3, content.ReadInt());
			Assert.AreEqual(DATA_4, content.ReadInt());
		}
	}
}
