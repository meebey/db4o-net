namespace Db4objects.Db4o
{
	public interface ISlotReader
	{
		int Offset();

		void Offset(int offset);

		void IncrementOffset(int numBytes);

		void IncrementIntSize();

		void ReadBegin(byte identifier);

		void ReadEnd();

		byte ReadByte();

		void Append(byte value);

		int ReadInt();

		void WriteInt(int value);

		long ReadLong();

		void WriteLong(long value);

		Db4objects.Db4o.Foundation.BitMap4 ReadBitMap(int bitCount);

		void CopyBytes(byte[] target, int sourceOffset, int targetOffset, int length);
	}
}
