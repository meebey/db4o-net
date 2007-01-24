namespace Db4objects.Db4o.IO.Crypt
{
	/// <exclude></exclude>
	public class XTEA
	{
		/// <exclude></exclude>
		public sealed class IterationSpec
		{
			internal int _iterations;

			internal int _deltaSumInitial;

			internal IterationSpec(int iterations, int deltaSumInitial)
			{
				_iterations = iterations;
				_deltaSumInitial = deltaSumInitial;
			}
		}

		public static readonly Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec ITERATIONS8 = 
			new Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec(8, unchecked((int)(0xF1BBCDC8)));

		public static readonly Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec ITERATIONS16 = 
			new Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec(16, unchecked((int)(0xE3779B90))
			);

		public static readonly Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec ITERATIONS32 = 
			new Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec(32, unchecked((int)(0xC6EF3720))
			);

		public static readonly Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec ITERATIONS64 = 
			new Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec(64, unchecked((int)(0x8DDE6E40))
			);

		private readonly Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec _iterationSpec;

		private const int DELTA = unchecked((int)(0x9E3779B9));

		private int[] _key;

		public XTEA(string key, Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec iterationSpec
			) : this(new Db4objects.Db4o.IO.Crypt.KeyGenerator().Core(key), iterationSpec)
		{
		}

		public XTEA(string key) : this(new Db4objects.Db4o.IO.Crypt.KeyGenerator().Core(key
			), ITERATIONS32)
		{
		}

		private XTEA(int[] key, Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec iterationSpec
			)
		{
			if (key.Length != 4)
			{
				throw new System.ArgumentException();
			}
			_key = key;
			_iterationSpec = iterationSpec;
		}

		/// <summary>
		/// converts incoming array of eight bytes from offset to array of two
		/// integer values.<br />
		/// (An Integer is represented in memory as four bytes.)
		/// </summary>
		/// <param name="bytes">Incoming byte array of length eight to be converted<br /></param>
		/// <param name="offset">Offset from which to start converting bytes<br /></param>
		/// <param name="res">Int array of length two which contains converted array bytes.</param>
		private void Byte2int(byte[] bytes, int offset, int[] res)
		{
			res[0] = (((bytes[offset] & unchecked((int)(0xff))) << 24) | ((bytes[offset + 1] 
				& unchecked((int)(0xff))) << 16) | ((bytes[offset + 2] & unchecked((int)(0xff)))
				 << 8) | (bytes[offset + 3] & unchecked((int)(0xff))));
			res[1] = (((bytes[offset + 4] & unchecked((int)(0xff))) << 24) | ((bytes[offset +
				 5] & unchecked((int)(0xff))) << 16) | ((bytes[offset + 6] & unchecked((int)(0xff
				))) << 8) | (bytes[offset + 7] & unchecked((int)(0xff))));
		}

		/// <summary>
		/// converts incoming array of two integers from offset to array of eight
		/// bytes.<br />
		/// (An Integer is represented in memory as four bytes.)
		/// </summary>
		/// <param name="i">Incoming integer array of two to be converted<br /></param>
		/// <param name="offset">Offset from which to start converting integer values<br /></param>
		/// <param name="res">
		/// byte array of length eight which contains converted integer
		/// array i.
		/// </param>
		private void Int2byte(int[] i, int offset, byte[] res)
		{
			res[offset] = (byte)(((i[0] & unchecked((int)(0xff000000)))) >> (24 & 0x1f));
			res[offset + 1] = (byte)(((i[0] & unchecked((int)(0x00ff0000)))) >> (16 & 0x1f));
			res[offset + 2] = (byte)(((i[0] & unchecked((int)(0x0000ff00)))) >> (8 & 0x1f));
			res[offset + 3] = (byte)(i[0] & unchecked((int)(0x000000ff)));
			res[offset + 4] = (byte)(((i[1] & unchecked((int)(0xff000000)))) >> (24 & 0x1f));
			res[offset + 5] = (byte)(((i[1] & unchecked((int)(0x00ff0000)))) >> (16 & 0x1f));
			res[offset + 6] = (byte)(((i[1] & unchecked((int)(0x0000ff00)))) >> (8 & 0x1f));
			res[offset + 7] = (byte)(i[1] & unchecked((int)(0x000000ff)));
		}

		/// <summary>enciphers two int values</summary>
		/// <param name="block">
		/// 
		/// int array to be encipher according to the XTEA encryption
		/// algorithm<br />
		/// </param>
		private void Encipher(int[] block)
		{
			int n = _iterationSpec._iterations;
			int delta_sum = 0;
			while (n-- > 0)
			{
				block[0] += ((block[1] << 4 ^ block[1] >> 5) + block[1]) ^ (delta_sum + _key[delta_sum
					 & 3]);
				delta_sum += DELTA;
				block[1] += ((block[0] << 4 ^ block[0] >> 5) + block[0]) ^ (delta_sum + _key[delta_sum
					 >> 11 & 3]);
			}
		}

		/// <summary>deciphers two int values</summary>
		/// <param name="e_block">
		/// int array to be decipher according to the XTEA encryption
		/// algorithm<br />
		/// </param>
		private void Decipher(int[] e_block)
		{
			int delta_sum = _iterationSpec._deltaSumInitial;
			int n = _iterationSpec._iterations;
			while (n-- > 0)
			{
				e_block[1] -= ((e_block[0] << 4 ^ e_block[0] >> 5) + e_block[0]) ^ (delta_sum + _key
					[delta_sum >> 11 & 3]);
				delta_sum -= DELTA;
				e_block[0] -= ((e_block[1] << 4 ^ e_block[1] >> 5) + e_block[1]) ^ (delta_sum + _key
					[delta_sum & 3]);
			}
		}

		/// <summary>encrypts incoming byte array according XTEA</summary>
		/// <param name="buffer">incoming byte array to be encrypted</param>
		public virtual void Encrypt(byte[] buffer)
		{
			int[] asInt = new int[2];
			for (int i = 0; i < buffer.Length; i += 8)
			{
				Byte2int(buffer, i, asInt);
				Encipher(asInt);
				Int2byte(asInt, i, buffer);
			}
		}

		/// <summary>decrypts incoming byte array according XTEA</summary>
		/// <param name="buffer">incoming byte array to be decrypted</param>
		public virtual void Decrypt(byte[] buffer)
		{
			int[] asInt = new int[2];
			for (int i = 0; i < buffer.Length; i += 8)
			{
				Byte2int(buffer, i, asInt);
				Decipher(asInt);
				Int2byte(asInt, i, buffer);
			}
		}
	}
}
