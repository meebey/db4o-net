/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface ISlotBuffer
	{
		int Offset();

		void Offset(int offset);

		void IncrementOffset(int numBytes);

		void IncrementIntSize();

		void ReadBegin(byte identifier);

		void ReadEnd();

		byte ReadByte();

		void WriteByte(byte value);

		int ReadInt();

		void WriteInt(int value);

		long ReadLong();

		void WriteLong(long value);

		BitMap4 ReadBitMap(int bitCount);

		void CopyBytes(byte[] target, int sourceOffset, int targetOffset, int length);
	}
}
