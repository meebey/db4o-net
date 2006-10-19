namespace Db4objects.Db4o
{
	/// <summary>Messages with Data for Client/Server Communication</summary>
	internal class MsgD : Db4objects.Db4o.Msg
	{
		internal Db4objects.Db4o.YapWriter _payLoad;

		internal MsgD() : base()
		{
		}

		internal MsgD(string aName) : base(aName)
		{
		}

		internal override void FakePayLoad(Db4objects.Db4o.Transaction a_trans)
		{
		}

		internal override Db4objects.Db4o.YapReader GetByteLoad()
		{
			return _payLoad;
		}

		internal sealed override Db4objects.Db4o.YapWriter GetPayLoad()
		{
			return _payLoad;
		}

		internal Db4objects.Db4o.MsgD GetWriterForLength(Db4objects.Db4o.Transaction a_trans
			, int length)
		{
			Db4objects.Db4o.MsgD message = (Db4objects.Db4o.MsgD)Clone(a_trans);
			message._payLoad = new Db4objects.Db4o.YapWriter(a_trans, length + Db4objects.Db4o.YapConst
				.MESSAGE_LENGTH);
			message.WriteInt(_msgID);
			message.WriteInt(length);
			if (a_trans.i_parentTransaction == null)
			{
				message._payLoad.Append(Db4objects.Db4o.YapConst.SYSTEM_TRANS);
			}
			else
			{
				message._payLoad.Append(Db4objects.Db4o.YapConst.USER_TRANS);
			}
			return message;
		}

		internal Db4objects.Db4o.MsgD GetWriter(Db4objects.Db4o.Transaction a_trans)
		{
			return GetWriterForLength(a_trans, 0);
		}

		internal Db4objects.Db4o.MsgD GetWriterForInts(Db4objects.Db4o.Transaction a_trans
			, int[] ints)
		{
			Db4objects.Db4o.MsgD message = GetWriterForLength(a_trans, Db4objects.Db4o.YapConst
				.INT_LENGTH * ints.Length);
			for (int i = 0; i < ints.Length; i++)
			{
				message.WriteInt(ints[i]);
			}
			return message;
		}

		internal Db4objects.Db4o.MsgD GetWriterForIntArray(Db4objects.Db4o.Transaction a_trans
			, int[] ints, int length)
		{
			Db4objects.Db4o.MsgD message = GetWriterForLength(a_trans, Db4objects.Db4o.YapConst
				.INT_LENGTH * (length + 1));
			message.WriteInt(length);
			for (int i = 0; i < length; i++)
			{
				message.WriteInt(ints[i]);
			}
			return message;
		}

		internal Db4objects.Db4o.MsgD GetWriterForInt(Db4objects.Db4o.Transaction a_trans
			, int id)
		{
			Db4objects.Db4o.MsgD message = GetWriterForLength(a_trans, Db4objects.Db4o.YapConst
				.INT_LENGTH);
			message.WriteInt(id);
			return message;
		}

		internal Db4objects.Db4o.MsgD GetWriterForIntString(Db4objects.Db4o.Transaction a_trans
			, int anInt, string str)
		{
			Db4objects.Db4o.MsgD message = GetWriterForLength(a_trans, Db4objects.Db4o.YapConst
				.stringIO.Length(str) + Db4objects.Db4o.YapConst.INT_LENGTH * 2);
			message.WriteInt(anInt);
			message.WriteString(str);
			return message;
		}

		internal Db4objects.Db4o.MsgD GetWriterForLong(Db4objects.Db4o.Transaction a_trans
			, long a_long)
		{
			Db4objects.Db4o.MsgD message = GetWriterForLength(a_trans, Db4objects.Db4o.YapConst
				.LONG_LENGTH);
			message.WriteLong(a_long);
			return message;
		}

		internal Db4objects.Db4o.MsgD GetWriterForString(Db4objects.Db4o.Transaction a_trans
			, string str)
		{
			Db4objects.Db4o.MsgD message = GetWriterForLength(a_trans, Db4objects.Db4o.YapConst
				.stringIO.Length(str) + Db4objects.Db4o.YapConst.INT_LENGTH);
			message.WriteString(str);
			return message;
		}

		internal virtual Db4objects.Db4o.MsgD GetWriter(Db4objects.Db4o.YapWriter bytes)
		{
			Db4objects.Db4o.MsgD message = GetWriterForLength(bytes.GetTransaction(), bytes.GetLength
				());
			message._payLoad.Append(bytes._buffer);
			return message;
		}

		internal virtual byte[] ReadBytes()
		{
			return _payLoad.ReadBytes(ReadInt());
		}

		internal int ReadInt()
		{
			return _payLoad.ReadInt();
		}

		internal long ReadLong()
		{
			return _payLoad.ReadLong();
		}

		internal sealed override Db4objects.Db4o.Msg ReadPayLoad(Db4objects.Db4o.Transaction
			 a_trans, Db4objects.Db4o.Foundation.Network.IYapSocket sock, Db4objects.Db4o.YapReader
			 reader)
		{
			int length = reader.ReadInt();
			if ((reader.ReadByte() == Db4objects.Db4o.YapConst.SYSTEM_TRANS) && (a_trans.i_parentTransaction
				 != null))
			{
				a_trans = a_trans.i_parentTransaction;
			}
			Db4objects.Db4o.MsgD command = (Db4objects.Db4o.MsgD)Clone(a_trans);
			command._payLoad = new Db4objects.Db4o.YapWriter(a_trans, length);
			command._payLoad.Read(sock);
			return command;
		}

		internal string ReadString()
		{
			int length = ReadInt();
			return Db4objects.Db4o.YapConst.stringIO.Read(_payLoad, length);
		}

		internal void WriteBytes(byte[] aBytes)
		{
			WriteInt(aBytes.Length);
			_payLoad.Append(aBytes);
		}

		internal void WriteInt(int aInt)
		{
			_payLoad.WriteInt(aInt);
		}

		internal void WriteLong(long l)
		{
			_payLoad.WriteLong(l);
		}

		internal void WriteString(string aStr)
		{
			_payLoad.WriteInt(aStr.Length);
			Db4objects.Db4o.YapConst.stringIO.Write(_payLoad, aStr);
		}
	}
}
