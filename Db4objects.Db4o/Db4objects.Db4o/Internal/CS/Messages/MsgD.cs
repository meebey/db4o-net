/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <summary>Messages with Data for Client/Server Communication</summary>
	public class MsgD : Msg
	{
		internal StatefulBuffer _payLoad;

		internal MsgD() : base()
		{
		}

		internal MsgD(string aName) : base(aName)
		{
		}

		public override ByteArrayBuffer GetByteLoad()
		{
			return _payLoad;
		}

		public sealed override StatefulBuffer PayLoad()
		{
			return _payLoad;
		}

		public virtual void PayLoad(StatefulBuffer writer)
		{
			_payLoad = writer;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForByte(Transaction trans
			, byte b)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = GetWriterForLength(trans, 1);
			msg._payLoad.WriteByte(b);
			return msg;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForLength(Transaction trans
			, int length)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = (Db4objects.Db4o.Internal.CS.Messages.MsgD
				)PublicClone();
			message.SetTransaction(trans);
			message._payLoad = new StatefulBuffer(trans, length + Const4.MessageLength);
			message.WriteInt(_msgID);
			message.WriteInt(length);
			if (trans.ParentTransaction() == null)
			{
				message._payLoad.WriteByte(Const4.SystemTrans);
			}
			else
			{
				message._payLoad.WriteByte(Const4.UserTrans);
			}
			return message;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriter(Transaction trans)
		{
			return GetWriterForLength(trans, 0);
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForInts(Transaction trans
			, int[] ints)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(trans, Const4
				.IntLength * ints.Length);
			for (int i = 0; i < ints.Length; i++)
			{
				message.WriteInt(ints[i]);
			}
			return message;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForIntArray(Transaction
			 a_trans, int[] ints, int length)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(a_trans, Const4
				.IntLength * (length + 1));
			message.WriteInt(length);
			for (int i = 0; i < length; i++)
			{
				message.WriteInt(ints[i]);
			}
			return message;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForInt(Transaction a_trans
			, int id)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(a_trans, Const4
				.IntLength);
			message.WriteInt(id);
			return message;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForIntString(Transaction
			 a_trans, int anInt, string str)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(a_trans, Const4
				.stringIO.Length(str) + Const4.IntLength * 2);
			message.WriteInt(anInt);
			message.WriteString(str);
			return message;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForLong(Transaction a_trans
			, long a_long)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(a_trans, Const4
				.LongLength);
			message.WriteLong(a_long);
			return message;
		}

		public virtual Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForSingleObject
			(Transaction trans, object obj)
		{
			SerializedGraph serialized = Serializer.Marshall(trans.Container(), obj);
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = GetWriterForLength(trans, serialized
				.MarshalledLength());
			serialized.Write(msg._payLoad);
			return msg;
		}

		public Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriterForString(Transaction a_trans
			, string str)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(a_trans, Const4
				.stringIO.Length(str) + Const4.IntLength);
			message.WriteString(str);
			return message;
		}

		public virtual Db4objects.Db4o.Internal.CS.Messages.MsgD GetWriter(StatefulBuffer
			 bytes)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = GetWriterForLength(bytes.GetTransaction
				(), bytes.Length());
			message._payLoad.Append(bytes._buffer);
			return message;
		}

		public virtual byte[] ReadBytes()
		{
			return _payLoad.ReadBytes(ReadInt());
		}

		public int ReadInt()
		{
			return _payLoad.ReadInt();
		}

		public long ReadLong()
		{
			return _payLoad.ReadLong();
		}

		public bool ReadBoolean()
		{
			return _payLoad.ReadByte() != 0;
		}

		public virtual object ReadObjectFromPayLoad()
		{
			return Serializer.Unmarshall(Stream(), _payLoad);
		}

		internal sealed override Msg ReadPayLoad(IMessageDispatcher messageDispatcher, Transaction
			 a_trans, ISocket4 sock, ByteArrayBuffer reader)
		{
			int length = reader.ReadInt();
			a_trans = CheckParentTransaction(a_trans, reader);
			Db4objects.Db4o.Internal.CS.Messages.MsgD command = (Db4objects.Db4o.Internal.CS.Messages.MsgD
				)PublicClone();
			command.SetTransaction(a_trans);
			command.SetMessageDispatcher(messageDispatcher);
			command._payLoad = ReadMessageBuffer(a_trans, sock, length);
			return command;
		}

		public string ReadString()
		{
			int length = ReadInt();
			return Const4.stringIO.Read(_payLoad, length);
		}

		public virtual object ReadSingleObject()
		{
			return Serializer.Unmarshall(Stream(), SerializedGraph.Read(_payLoad));
		}

		public void WriteBytes(byte[] aBytes)
		{
			WriteInt(aBytes.Length);
			_payLoad.Append(aBytes);
		}

		public void WriteInt(int aInt)
		{
			_payLoad.WriteInt(aInt);
		}

		public void WriteLong(long l)
		{
			_payLoad.WriteLong(l);
		}

		public void WriteString(string aStr)
		{
			_payLoad.WriteInt(aStr.Length);
			Const4.stringIO.Write(_payLoad, aStr);
		}
	}
}
