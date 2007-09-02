/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Marshall
{
	/// <summary>a buffer interface with methods to read.</summary>
	/// <remarks>a buffer interface with methods to read.</remarks>
	public interface IReadBuffer
	{
		/// <summary>reads a byte from the buffer.</summary>
		/// <remarks>reads a byte from the buffer.</remarks>
		/// <returns>the byte</returns>
		byte ReadByte();

		/// <summary>reads an array of bytes from the buffer.</summary>
		/// <remarks>
		/// reads an array of bytes from the buffer.
		/// The length of the array that is passed as a parameter specifies the
		/// number of bytes that are to be read. The passed bytes buffer parameter
		/// is directly filled.
		/// </remarks>
		/// <param name="bytes">the byte array to read the bytes into.</param>
		void ReadBytes(byte[] bytes);

		/// <summary>reads an int from the buffer.</summary>
		/// <remarks>reads an int from the buffer.</remarks>
		/// <returns>the int</returns>
		int ReadInt();

		/// <summary>reads a long from the buffer.</summary>
		/// <remarks>reads a long from the buffer.</remarks>
		/// <returns>the long</returns>
		long ReadLong();
	}
}
