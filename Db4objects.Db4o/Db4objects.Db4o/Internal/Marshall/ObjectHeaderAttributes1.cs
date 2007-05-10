using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class ObjectHeaderAttributes1 : ObjectHeaderAttributes
	{
		private const byte VERSION = (byte)1;

		private readonly int _fieldCount;

		private readonly BitMap4 _nullBitMap;

		private int _baseLength;

		private int _payLoadLength;

		public ObjectHeaderAttributes1(ObjectReference yo)
		{
			_fieldCount = yo.GetYapClass().FieldCount();
			_nullBitMap = new BitMap4(_fieldCount);
			CalculateLengths(yo);
		}

		public ObjectHeaderAttributes1(Db4objects.Db4o.Internal.Buffer reader)
		{
			_fieldCount = reader.ReadInt();
			_nullBitMap = reader.ReadBitMap(_fieldCount);
		}

		public override void AddBaseLength(int length)
		{
			_baseLength += length;
		}

		public override void AddPayLoadLength(int length)
		{
			_payLoadLength += length;
		}

		private void CalculateLengths(ObjectReference yo)
		{
			_baseLength = HeaderLength() + NullBitMapLength();
			_payLoadLength = 0;
			ClassMetadata yc = yo.GetYapClass();
			Transaction trans = yo.GetTrans();
			object obj = yo.GetObject();
			CalculateLengths(trans, yc, obj, 0);
			_baseLength = yo.GetStream().BlockAlignedBytes(_baseLength);
		}

		private void CalculateLengths(Transaction trans, ClassMetadata yc, object obj, int
			 fieldIndex)
		{
			_baseLength += Const4.INT_LENGTH;
			if (yc.i_fields != null)
			{
				for (int i = 0; i < yc.i_fields.Length; i++)
				{
					FieldMetadata yf = yc.i_fields[i];
					object child = yf.GetOrCreate(trans, obj);
					if (child == null && yf.CanUseNullBitmap())
					{
						_nullBitMap.SetTrue(fieldIndex);
					}
					else
					{
						yf.CalculateLengths(trans, this, child);
					}
					fieldIndex++;
				}
			}
			if (yc.i_ancestor == null)
			{
				return;
			}
			CalculateLengths(trans, yc.i_ancestor, obj, fieldIndex);
		}

		private int HeaderLength()
		{
			return Const4.OBJECT_LENGTH + Const4.ID_LENGTH + 1;
		}

		public virtual bool IsNull(int fieldIndex)
		{
			return _nullBitMap.IsTrue(fieldIndex);
		}

		private int NullBitMapLength()
		{
			return Const4.INT_LENGTH + _nullBitMap.MarshalledLength();
		}

		public virtual int ObjectLength()
		{
			return _baseLength + _payLoadLength;
		}

		public override void PrepareIndexedPayLoadEntry(Transaction trans)
		{
			_payLoadLength = trans.Stream().BlockAlignedBytes(_payLoadLength);
		}

		public virtual void Write(StatefulBuffer writer)
		{
			writer.Append(VERSION);
			writer.WriteInt(_fieldCount);
			writer.WriteBitMap(_nullBitMap);
			writer._payloadOffset = _baseLength;
		}
	}
}
