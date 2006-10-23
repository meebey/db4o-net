namespace Db4objects.Db4o
{
	/// <summary>public for .NET conversion reasons.</summary>
	/// <remarks>public for .NET conversion reasons.</remarks>
	/// <exclude></exclude>
	public class YapReader : Db4objects.Db4o.ISlotReader
	{
		public byte[] _buffer;

		public int _offset;

		internal YapReader()
		{
		}

		public YapReader(int a_length)
		{
			_buffer = new byte[a_length];
		}

		public virtual void Seek(int offset)
		{
			_offset = offset;
		}

		public virtual void Append(byte a_byte)
		{
			_buffer[_offset++] = a_byte;
		}

		public virtual void Append(byte[] a_bytes)
		{
			System.Array.Copy(a_bytes, 0, _buffer, _offset, a_bytes.Length);
			_offset += a_bytes.Length;
		}

		internal bool ContainsTheSame(Db4objects.Db4o.YapReader other)
		{
			if (other != null)
			{
				byte[] otherBytes = other._buffer;
				if (_buffer == null)
				{
					return otherBytes == null;
				}
				if (otherBytes != null && _buffer.Length == otherBytes.Length)
				{
					int len = _buffer.Length;
					for (int i = 0; i < len; i++)
					{
						if (_buffer[i] != otherBytes[i])
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		public virtual void CopyTo(Db4objects.Db4o.YapReader to, int fromOffset, int toOffset
			, int length)
		{
			System.Array.Copy(_buffer, fromOffset, to._buffer, toOffset, length);
		}

		public virtual int GetLength()
		{
			return _buffer.Length;
		}

		public virtual void IncrementOffset(int a_by)
		{
			_offset += a_by;
		}

		/// <summary>non-encrypted read, used for indexes</summary>
		/// <param name="a_stream"></param>
		/// <param name="a_address"></param>
		public virtual void Read(Db4objects.Db4o.YapStream a_stream, int a_address, int addressOffset
			)
		{
			a_stream.ReadBytes(_buffer, a_address, addressOffset, GetLength());
		}

		public void ReadBegin(byte a_identifier)
		{
		}

		public virtual Db4objects.Db4o.Foundation.BitMap4 ReadBitMap(int bitCount)
		{
			Db4objects.Db4o.Foundation.BitMap4 map = new Db4objects.Db4o.Foundation.BitMap4(_buffer
				, _offset, bitCount);
			_offset += map.MarshalledLength();
			return map;
		}

		public virtual byte ReadByte()
		{
			return _buffer[_offset++];
		}

		public virtual byte[] ReadBytes(int a_length)
		{
			byte[] bytes = new byte[a_length];
			ReadBytes(bytes);
			return bytes;
		}

		internal virtual void ReadBytes(byte[] bytes)
		{
			int length = bytes.Length;
			System.Array.Copy(_buffer, _offset, bytes, 0, length);
			_offset += length;
		}

		public Db4objects.Db4o.YapReader ReadEmbeddedObject(Db4objects.Db4o.Transaction a_trans
			)
		{
			return a_trans.Stream().ReadReaderByAddress(ReadInt(), ReadInt());
		}

		public virtual void ReadEncrypt(Db4objects.Db4o.YapStream a_stream, int a_address
			)
		{
			a_stream.ReadBytes(_buffer, a_address, GetLength());
			a_stream.i_handlers.Decrypt(this);
		}

		public virtual void ReadEnd()
		{
			if (Db4objects.Db4o.Deploy.debug && Db4objects.Db4o.Deploy.brackets)
			{
				if (ReadByte() != Db4objects.Db4o.YapConst.YAPEND)
				{
					throw new System.Exception("YapBytes.readEnd() YAPEND expected");
				}
			}
		}

		public int ReadInt()
		{
			int o = (_offset += 4) - 1;
			return (_buffer[o] & 255) | (_buffer[--o] & 255) << 8 | (_buffer[--o] & 255) << 16
				 | _buffer[--o] << 24;
		}

		public virtual long ReadLong()
		{
			return Db4objects.Db4o.YLong.ReadLong(this);
		}

		public virtual Db4objects.Db4o.YapReader ReadPayloadReader(int offset, int length
			)
		{
			Db4objects.Db4o.YapReader payLoad = new Db4objects.Db4o.YapReader(length);
			System.Array.Copy(_buffer, offset, payLoad._buffer, 0, length);
			return payLoad;
		}

		internal virtual void ReplaceWith(byte[] a_bytes)
		{
			System.Array.Copy(a_bytes, 0, _buffer, 0, GetLength());
		}

		public override string ToString()
		{
			try
			{
				string str = "";
				for (int i = 0; i < _buffer.Length; i++)
				{
					if (i > 0)
					{
						str += " , ";
					}
					str += _buffer[i];
				}
				return str;
			}
			catch (System.Exception e)
			{
				if (Db4objects.Db4o.Deploy.debug || Db4objects.Db4o.Debug.atHome)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
			return "";
		}

		public virtual void WriteBegin(byte a_identifier)
		{
		}

		public virtual void WriteBitMap(Db4objects.Db4o.Foundation.BitMap4 nullBitMap)
		{
			nullBitMap.WriteTo(_buffer, _offset);
			_offset += nullBitMap.MarshalledLength();
		}

		public virtual void WriteEncrypt(Db4objects.Db4o.YapFile file, int address, int addressOffset
			)
		{
			file.i_handlers.Encrypt(this);
			Write(file, address, addressOffset);
			file.i_handlers.Decrypt(this);
		}

		public virtual void Write(Db4objects.Db4o.YapFile file, int address, int addressOffset
			)
		{
			file.WriteBytes(this, address, addressOffset);
		}

		public virtual void WriteEnd()
		{
			if (Db4objects.Db4o.Deploy.debug && Db4objects.Db4o.Deploy.brackets)
			{
				Append(Db4objects.Db4o.YapConst.YAPEND);
			}
		}

		public void WriteInt(int a_int)
		{
			int o = _offset + 4;
			_offset = o;
			byte[] b = _buffer;
			b[--o] = (byte)a_int;
			b[--o] = (byte)(a_int >>= 8);
			b[--o] = (byte)(a_int >>= 8);
			b[--o] = (byte)(a_int >>= 8);
		}

		public virtual void WriteIDOf(Db4objects.Db4o.Transaction trans, object obj)
		{
			if (obj == null)
			{
				WriteInt(0);
				return;
			}
			if (obj is Db4objects.Db4o.YapMeta)
			{
				WriteIDOf(trans, (Db4objects.Db4o.YapMeta)obj);
				return;
			}
			WriteInt(((int)obj));
		}

		public virtual void WriteIDOf(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapMeta
			 yapMeta)
		{
			if (yapMeta == null)
			{
				WriteInt(0);
				return;
			}
			yapMeta.WriteOwnID(trans, this);
		}

		public virtual void WriteShortString(Db4objects.Db4o.Transaction trans, string a_string
			)
		{
			trans.Stream().i_handlers.i_stringHandler.WriteShort(a_string, this);
		}

		public virtual void WriteLong(long l)
		{
			Db4objects.Db4o.YLong.WriteLong(l, this);
		}

		public virtual void IncrementIntSize()
		{
			IncrementOffset(Db4objects.Db4o.YapConst.INT_LENGTH);
		}

		public virtual int Offset()
		{
			return _offset;
		}

		public virtual void Offset(int offset)
		{
			_offset = offset;
		}

		public virtual void CopyBytes(byte[] target, int sourceOffset, int targetOffset, 
			int length)
		{
			System.Array.Copy(_buffer, sourceOffset, target, targetOffset, length);
		}
	}
}
