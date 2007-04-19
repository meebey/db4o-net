using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MsgObject : MsgD
	{
		private const int LENGTH_FOR_ALL = Const4.ID_LENGTH + (Const4.INT_LENGTH * 3);

		private const int LENGTH_FOR_FIRST = LENGTH_FOR_ALL;

		private int _id;

		private int _address;

		internal virtual MsgD GetWriter(StatefulBuffer bytes, int[] prependInts)
		{
			int lengthNeeded = bytes.GetLength() + LENGTH_FOR_FIRST;
			if (prependInts != null)
			{
				lengthNeeded += (prependInts.Length * Const4.INT_LENGTH);
			}
			int embeddedCount = bytes.EmbeddedCount();
			if (embeddedCount > 0)
			{
				lengthNeeded += (LENGTH_FOR_ALL * embeddedCount) + bytes.EmbeddedLength();
			}
			MsgD message = GetWriterForLength(bytes.GetTransaction(), lengthNeeded);
			if (prependInts != null)
			{
				for (int i = 0; i < prependInts.Length; i++)
				{
					message._payLoad.WriteInt(prependInts[i]);
				}
			}
			message._payLoad.WriteInt(embeddedCount);
			bytes.AppendTo(message._payLoad, -1);
			return message;
		}

		public override MsgD GetWriter(StatefulBuffer bytes)
		{
			return GetWriter(bytes, null);
		}

		public virtual MsgD GetWriter(ClassMetadata a_yapClass, StatefulBuffer bytes)
		{
			if (a_yapClass == null)
			{
				return GetWriter(bytes, new int[] { 0 });
			}
			return GetWriter(bytes, new int[] { a_yapClass.GetID() });
		}

		public virtual MsgD GetWriter(ClassMetadata a_yapClass, int a_param, StatefulBuffer
			 bytes)
		{
			return GetWriter(bytes, new int[] { a_yapClass.GetID(), a_param });
		}

		public StatefulBuffer Unmarshall()
		{
			return Unmarshall(0);
		}

		public StatefulBuffer Unmarshall(int addLengthBeforeFirst)
		{
			_payLoad.SetTransaction(Transaction());
			int embeddedCount = _payLoad.ReadInt();
			int length = _payLoad.ReadInt();
			if (length == 0)
			{
				return null;
			}
			_id = _payLoad.ReadInt();
			_address = _payLoad.ReadInt();
			if (embeddedCount == 0)
			{
				_payLoad.RemoveFirstBytes(LENGTH_FOR_FIRST + addLengthBeforeFirst);
			}
			else
			{
				_payLoad._offset += length;
				StatefulBuffer[] embedded = new StatefulBuffer[embeddedCount + 1];
				embedded[0] = _payLoad;
				new StatefulBuffer(_payLoad, embedded, 1);
				_payLoad.Trim4(LENGTH_FOR_FIRST + addLengthBeforeFirst, length);
			}
			_payLoad.UseSlot(_id, _address, length);
			return _payLoad;
		}

		public virtual int GetId()
		{
			return _id;
		}
	}
}
