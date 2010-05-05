namespace Db4objects.Db4o.Foundation
{
	/// <summary>
	/// <P> Calculates the CRC32 - 32 bit Cyclical Redundancy Check
	/// <P> This check is used in numerous systems to verify the integrity
	/// of information.
	/// </summary>
	/// <remarks>
	/// <P> Calculates the CRC32 - 32 bit Cyclical Redundancy Check
	/// <P> This check is used in numerous systems to verify the integrity
	/// of information.  It's also used as a hashing function.  Unlike a regular
	/// checksum, it's sensitive to the order of the characters.
	/// It produces a 32 bit (Java <CODE>int</CODE>.
	/// <P>
	/// This Java programme was translated from a C version I had written.
	/// <P> This software is in the public domain.
	/// </remarks>
	/// <author>Michael Lecuyer</author>
	/// <version>1.1 August 11, 1998</version>
    public class CRC32
    {
        private static uint[] crcTable;

        static CRC32()
        {
            // This software is in the public domain.
            //
            // CRC Lookup table
            BuildCRCTable();
        }

        private static void BuildCRCTable()
        {
            uint Crc32Polynomial = 0xEDB88320;
            uint i;
            uint j;
            uint crc;
            crcTable = new uint[256];
            for (i = 0; i <= 255; i++)
            {
                crc = i;
                for (j = 8; j > 0; j--)
                {
                    if ((crc & 1) == 1)
                    {
                        crc = ((crc) >> (1 & 0x1f)) ^ Crc32Polynomial;
                    }
                    else
                    {
                        crc = crc >> (1 & 0x1f);
                    }
                }
                crcTable[i] = crc;
            }
        }

        public static long CheckSum(byte[] buffer, int start, int count)
        {
            uint temp1;
            uint temp2;
            int i = start;
            uint crc = 0xFFFFFFFF;
            while (count-- != 0)
            {
                temp1 = (crc) >> (8 & 0x1f);
                temp2 = crcTable[(crc ^ buffer[i++]) & 0xFF];
                crc = temp1 ^ temp2;
            }
            return (long)~crc & 0xFFFFFFFFL;
        }
    }
}
