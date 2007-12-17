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
			BufferImpl content = InspectContent(buffer);
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
			buffer.TransferLastWriteTo(other, true);
			Assert.AreEqual(lastOffset, Offset(buffer));
			BufferImpl content = InspectContent(other);
			Assert.AreEqual(DATA_2, content.ReadByte());
		}

		private int Offset(MarshallingBuffer buffer)
		{
			return buffer.TestDelegate().Offset();
		}

		private BufferImpl InspectContent(MarshallingBuffer buffer)
		{
			BufferImpl bufferDelegate = buffer.TestDelegate();
			bufferDelegate.Seek(0);
			return bufferDelegate;
		}

		public virtual void TestChildren()
		{
			MarshallingBuffer buffer = new MarshallingBuffer();
			buffer.WriteInt(DATA_1);
			buffer.WriteByte(DATA_2);
			MarshallingBuffer child = buffer.AddChild();
			child.WriteInt(DATA_3);
			child.WriteInt(DATA_4);
			buffer.MergeChildren(null, 0, 0);
			BufferImpl content = InspectContent(buffer);
			Assert.AreEqual(DATA_1, content.ReadInt());
			Assert.AreEqual(DATA_2, content.ReadByte());
			int address = content.ReadInt();
			content.Seek(address);
			Assert.AreEqual(DATA_3, content.ReadInt());
			Assert.AreEqual(DATA_4, content.ReadInt());
		}

		public virtual void TestGrandChildren()
		{
			MarshallingBuffer buffer = new MarshallingBuffer();
			buffer.WriteInt(DATA_1);
			buffer.WriteByte(DATA_2);
			MarshallingBuffer child = buffer.AddChild();
			child.WriteInt(DATA_3);
			child.WriteInt(DATA_4);
			MarshallingBuffer grandChild = child.AddChild();
			grandChild.WriteInt(DATA_5);
			buffer.MergeChildren(null, 0, 0);
			BufferImpl content = InspectContent(buffer);
			Assert.AreEqual(DATA_1, content.ReadInt());
			Assert.AreEqual(DATA_2, content.ReadByte());
			int address = content.ReadInt();
			content.Seek(address);
			Assert.AreEqual(DATA_3, content.ReadInt());
			Assert.AreEqual(DATA_4, content.ReadInt());
			address = content.ReadInt();
			content.Seek(address);
			Assert.AreEqual(DATA_5, content.ReadInt());
		}

		public virtual void TestLinkOffset()
		{
			int linkOffset = 7;
			MarshallingBuffer buffer = new MarshallingBuffer();
			buffer.WriteInt(DATA_1);
			buffer.WriteByte(DATA_2);
			MarshallingBuffer child = buffer.AddChild();
			child.WriteInt(DATA_3);
			child.WriteInt(DATA_4);
			MarshallingBuffer grandChild = child.AddChild();
			grandChild.WriteInt(DATA_5);
			buffer.MergeChildren(null, 0, linkOffset);
			BufferImpl content = InspectContent(buffer);
			BufferImpl extendedBuffer = new BufferImpl(content.Length() + linkOffset);
			content.CopyTo(extendedBuffer, 0, linkOffset, content.Length());
			extendedBuffer.Seek(linkOffset);
			Assert.AreEqual(DATA_1, extendedBuffer.ReadInt());
			Assert.AreEqual(DATA_2, extendedBuffer.ReadByte());
			int address = extendedBuffer.ReadInt();
			extendedBuffer.Seek(address);
			Assert.AreEqual(DATA_3, extendedBuffer.ReadInt());
			Assert.AreEqual(DATA_4, extendedBuffer.ReadInt());
			address = extendedBuffer.ReadInt();
			extendedBuffer.Seek(address);
			Assert.AreEqual(DATA_5, extendedBuffer.ReadInt());
		}

		public virtual void TestLateChildrenWrite()
		{
			MarshallingBuffer buffer = new MarshallingBuffer();
			buffer.WriteInt(DATA_1);
			MarshallingBuffer child = buffer.AddChild(true, true);
			child.WriteInt(DATA_3);
			buffer.WriteByte(DATA_2);
			child.WriteInt(DATA_4);
			buffer.MergeChildren(null, 0, 0);
			BufferImpl content = InspectContent(buffer);
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
