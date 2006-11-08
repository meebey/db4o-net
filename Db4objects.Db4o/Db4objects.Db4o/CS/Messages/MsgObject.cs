namespace Db4objects.Db4o.CS.Messages
{
	public class MsgObject : Db4objects.Db4o.CS.Messages.MsgD
	{
		private const int LENGTH_FOR_ALL = Db4objects.Db4o.YapConst.ID_LENGTH + (Db4objects.Db4o.YapConst
			.INT_LENGTH * 3);

		private const int LENGTH_FOR_FIRST = LENGTH_FOR_ALL;

		private int _id;

		private int _address;

		internal virtual Db4objects.Db4o.CS.Messages.MsgD GetWriter(Db4objects.Db4o.YapWriter
			 bytes, int[] prependInts)
		{
			int lengthNeeded = bytes.GetLength() + LENGTH_FOR_FIRST;
			if (prependInts != null)
			{
				lengthNeeded += (prependInts.Length * Db4objects.Db4o.YapConst.INT_LENGTH);
			}
			int embeddedCount = bytes.EmbeddedCount();
			if (embeddedCount > 0)
			{
				lengthNeeded += (LENGTH_FOR_ALL * embeddedCount) + bytes.EmbeddedLength();
			}
			Db4objects.Db4o.CS.Messages.MsgD message = GetWriterForLength(bytes.GetTransaction
				(), lengthNeeded);
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

		public override Db4objects.Db4o.CS.Messages.MsgD GetWriter(Db4objects.Db4o.YapWriter
			 bytes)
		{
			return GetWriter(bytes, null);
		}

		public virtual Db4objects.Db4o.CS.Messages.MsgD GetWriter(Db4objects.Db4o.YapClass
			 a_yapClass, Db4objects.Db4o.YapWriter bytes)
		{
			if (a_yapClass == null)
			{
				return GetWriter(bytes, new int[] { 0 });
			}
			return GetWriter(bytes, new int[] { a_yapClass.GetID() });
		}

		public virtual Db4objects.Db4o.CS.Messages.MsgD GetWriter(Db4objects.Db4o.YapClass
			 a_yapClass, int a_param, Db4objects.Db4o.YapWriter bytes)
		{
			return GetWriter(bytes, new int[] { a_yapClass.GetID(), a_param });
		}

		public Db4objects.Db4o.YapWriter Unmarshall()
		{
			return Unmarshall(0);
		}

		public Db4objects.Db4o.YapWriter Unmarshall(int addLengthBeforeFirst)
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
				Db4objects.Db4o.YapWriter[] embedded = new Db4objects.Db4o.YapWriter[embeddedCount
					 + 1];
				embedded[0] = _payLoad;
				new Db4objects.Db4o.YapWriter(_payLoad, embedded, 1);
				_payLoad.Trim4(LENGTH_FOR_FIRST + addLengthBeforeFirst, length);
			}
			_payLoad.UseSlot(_id, _address, length);
			return _payLoad;
		}

		public virtual void PayLoad(Db4objects.Db4o.YapWriter writer)
		{
			_payLoad = writer;
		}

		public virtual int GetId()
		{
			return _id;
		}
	}
}
