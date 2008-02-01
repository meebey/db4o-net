/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class PersistentIntegerArray : PersistentBase
	{
		private int[] _ints;

		public PersistentIntegerArray(int id)
		{
			SetID(id);
		}

		public PersistentIntegerArray(int[] arr)
		{
			_ints = new int[arr.Length];
			System.Array.Copy(arr, 0, _ints, 0, arr.Length);
		}

		public override byte GetIdentifier()
		{
			return Const4.IntegerArray;
		}

		public override int OwnLength()
		{
			return (Const4.IntLength * (Size() + 1)) + Const4.AddedLength;
		}

		public override void ReadThis(Transaction trans, ByteArrayBuffer reader)
		{
			int length = reader.ReadInt();
			_ints = new int[length];
			for (int i = 0; i < length; i++)
			{
				_ints[i] = reader.ReadInt();
			}
		}

		public override void WriteThis(Transaction trans, ByteArrayBuffer writer)
		{
			writer.WriteInt(Size());
			for (int i = 0; i < _ints.Length; i++)
			{
				writer.WriteInt(_ints[i]);
			}
		}

		private int Size()
		{
			return _ints.Length;
		}

		public virtual int[] Array()
		{
			return _ints;
		}
	}
}
