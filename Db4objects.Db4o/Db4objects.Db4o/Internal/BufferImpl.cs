/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class BufferImpl : IReadBuffer, IBuffer, IWriteBuffer
	{
		public byte[] _buffer;

		public int _offset;

		internal BufferImpl()
		{
		}

		public BufferImpl(int a_length)
		{
			// for coding convenience, we allow objects to grab into the buffer
			_buffer = new byte[a_length];
		}

		public virtual void Seek(int offset)
		{
			_offset = offset;
		}

		public virtual void WriteBytes(byte[] bytes)
		{
			System.Array.Copy(bytes, 0, _buffer, _offset, bytes.Length);
			_offset += bytes.Length;
		}

		public virtual void Append(byte[] bytes)
		{
			// TODO: Change all callers to call writeBytes directly.
			WriteBytes(bytes);
		}

		public bool ContainsTheSame(Db4objects.Db4o.Internal.BufferImpl other)
		{
			if (other != null)
			{
				return Arrays4.AreEqual(_buffer, other._buffer);
			}
			return false;
		}

		public virtual void CopyTo(Db4objects.Db4o.Internal.BufferImpl to, int fromOffset
			, int toOffset, int length)
		{
			System.Array.Copy(_buffer, fromOffset, to._buffer, toOffset, length);
		}

		public virtual int Length()
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
		public virtual void Read(ObjectContainerBase stream, int address, int addressOffset
			)
		{
			stream.ReadBytes(_buffer, address, addressOffset, Length());
		}

		public void ReadBegin(byte identifier)
		{
		}

		public virtual BitMap4 ReadBitMap(int bitCount)
		{
			BitMap4 map = new BitMap4(_buffer, _offset, bitCount);
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

		public virtual void ReadBytes(byte[] bytes)
		{
			int length = bytes.Length;
			System.Array.Copy(_buffer, _offset, bytes, 0, length);
			_offset += length;
		}

		/// <exception cref="Db4oIOException"></exception>
		public Db4objects.Db4o.Internal.BufferImpl ReadEmbeddedObject(Transaction trans)
		{
			int address = ReadInt();
			int length = ReadInt();
			if (address == 0)
			{
				return null;
			}
			return trans.Container().BufferByAddress(address, length);
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual void ReadEncrypt(ObjectContainerBase stream, int address)
		{
			stream.ReadBytes(_buffer, address, Length());
			stream._handlers.Decrypt(this);
		}

		public virtual void ReadEnd()
		{
		}

		public int ReadInt()
		{
			int o = (_offset += 4) - 1;
			return (_buffer[o] & 255) | (_buffer[--o] & 255) << 8 | (_buffer[--o] & 255) << 16
				 | _buffer[--o] << 24;
		}

		public virtual long ReadLong()
		{
			return LongHandler.ReadLong(this);
		}

		public virtual Db4objects.Db4o.Internal.BufferImpl ReadPayloadReader(int offset, 
			int length)
		{
			Db4objects.Db4o.Internal.BufferImpl payLoad = new Db4objects.Db4o.Internal.BufferImpl
				(length);
			System.Array.Copy(_buffer, offset, payLoad._buffer, 0, length);
			return payLoad;
		}

		internal virtual void ReplaceWith(byte[] a_bytes)
		{
			System.Array.Copy(a_bytes, 0, _buffer, 0, Length());
		}

		public override string ToString()
		{
			string str = string.Empty;
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

		public virtual void WriteBegin(byte a_identifier)
		{
		}

		public void WriteBitMap(BitMap4 nullBitMap)
		{
			nullBitMap.WriteTo(_buffer, _offset);
			_offset += nullBitMap.MarshalledLength();
		}

		public void WriteByte(byte a_byte)
		{
			_buffer[_offset++] = a_byte;
		}

		public virtual void WriteEnd()
		{
			if (Deploy.debug && Deploy.brackets)
			{
				WriteByte(Const4.Yapend);
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
			b[--o] = (byte)(a_int >> 8);
		}

		public virtual void WriteIDOf(Transaction trans, object obj)
		{
			if (obj == null)
			{
				WriteInt(0);
				return;
			}
			if (obj is PersistentBase)
			{
				WriteIDOf(trans, (PersistentBase)obj);
				return;
			}
			WriteInt(((int)obj));
		}

		public virtual void WriteIDOf(Transaction trans, PersistentBase persistent)
		{
			if (persistent == null)
			{
				WriteInt(0);
				return;
			}
			if (CanWritePersistentBase())
			{
				persistent.WriteOwnID(trans, this);
			}
			else
			{
				WriteInt(persistent.GetID());
			}
		}

		protected virtual bool CanWritePersistentBase()
		{
			return true;
		}

		public virtual void WriteShortString(Transaction trans, string a_string)
		{
			trans.Container()._handlers._stringHandler.WriteShort(trans, a_string, this);
		}

		public virtual void WriteLong(long l)
		{
			LongHandler.WriteLong(this, l);
		}

		public virtual void IncrementIntSize()
		{
			IncrementOffset(Const4.IntLength);
		}

		public virtual int Offset()
		{
			return _offset;
		}
	}
}
