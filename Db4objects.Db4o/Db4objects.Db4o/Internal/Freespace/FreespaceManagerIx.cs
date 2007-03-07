namespace Db4objects.Db4o.Internal.Freespace
{
	public class FreespaceManagerIx : Db4objects.Db4o.Internal.Freespace.FreespaceManager
	{
		private int _slotAddress;

		private Db4objects.Db4o.Internal.Freespace.FreespaceIxAddress _addressIx;

		private Db4objects.Db4o.Internal.Freespace.FreespaceIxLength _lengthIx;

		private bool _started;

		private Db4objects.Db4o.Foundation.Collection4 _xBytes;

		private readonly bool _overwriteDeletedSlots;

		internal FreespaceManagerIx(Db4objects.Db4o.Internal.LocalObjectContainer file) : 
			base(file)
		{
			_overwriteDeletedSlots = Db4objects.Db4o.Debug.xbytes || file.Config().FreespaceFiller
				() != null;
		}

		private void Add(int address, int length)
		{
			_addressIx.Add(address, length);
			_lengthIx.Add(address, length);
		}

		public override void BeginCommit()
		{
			if (!Started())
			{
				return;
			}
			SlotEntryToZeroes(_file, _slotAddress);
		}

		public override void Debug()
		{
		}

		public override void EndCommit()
		{
			if (!Started())
			{
				return;
			}
			if (_overwriteDeletedSlots)
			{
				_xBytes = new Db4objects.Db4o.Foundation.Collection4();
			}
			_addressIx._index.CommitFreeSpace(_lengthIx._index);
			Db4objects.Db4o.Internal.StatefulBuffer writer = new Db4objects.Db4o.Internal.StatefulBuffer
				(_file.GetSystemTransaction(), _slotAddress, SlotLength());
			_addressIx._index._metaIndex.Write(writer);
			_lengthIx._index._metaIndex.Write(writer);
			if (_overwriteDeletedSlots)
			{
				writer.SetID(Db4objects.Db4o.Internal.Const4.IGNORE_ID);
			}
			if (_file.ConfigImpl().FlushFileBuffers())
			{
				_file.SyncFiles();
			}
			writer.WriteEncrypt();
			if (_overwriteDeletedSlots)
			{
				System.Collections.IEnumerator i = _xBytes.GetEnumerator();
				_xBytes = null;
				while (i.MoveNext())
				{
					int[] addressLength = (int[])i.Current;
					OverwriteDeletedSlots(addressLength[0], addressLength[1]);
				}
			}
		}

		public override int EntryCount()
		{
			return _addressIx.EntryCount();
		}

		public override void Free(int address, int length)
		{
			if (!Started())
			{
				return;
			}
			if (address <= 0)
			{
				return;
			}
			if (length <= DiscardLimit())
			{
				return;
			}
			length = _file.BlocksFor(length);
			int freedAddress = address;
			int freedLength = length;
			_addressIx.Find(address);
			if (_addressIx.Preceding())
			{
				if (_addressIx.Address() + _addressIx.Length() == address)
				{
					Remove(_addressIx.Address(), _addressIx.Length());
					address = _addressIx.Address();
					length += _addressIx.Length();
					_addressIx.Find(freedAddress);
				}
			}
			if (_addressIx.Subsequent())
			{
				if (freedAddress + freedLength == _addressIx.Address())
				{
					Remove(_addressIx.Address(), _addressIx.Length());
					length += _addressIx.Length();
				}
			}
			Add(address, length);
			if (_overwriteDeletedSlots)
			{
				OverwriteDeletedSlots(freedAddress, freedLength);
			}
		}

		public override void FreeSelf()
		{
			if (!Started())
			{
				return;
			}
			_addressIx._index._metaIndex.Free(_file);
			_lengthIx._index._metaIndex.Free(_file);
		}

		public override int FreeSize()
		{
			return _addressIx.FreeSize();
		}

		public override int GetSlot(int length)
		{
			if (!Started())
			{
				return 0;
			}
			int address = GetSlot1(length);
			if (address != 0)
			{
			}
			return address;
		}

		private int GetSlot1(int length)
		{
			if (!Started())
			{
				return 0;
			}
			length = _file.BlocksFor(length);
			_lengthIx.Find(length);
			if (_lengthIx.Match())
			{
				Remove(_lengthIx.Address(), _lengthIx.Length());
				return _lengthIx.Address();
			}
			if (_lengthIx.Subsequent())
			{
				int lengthRemainder = _lengthIx.Length() - length;
				int addressRemainder = _lengthIx.Address() + length;
				Remove(_lengthIx.Address(), _lengthIx.Length());
				Add(addressRemainder, lengthRemainder);
				return _lengthIx.Address();
			}
			return 0;
		}

		public override void Migrate(Db4objects.Db4o.Internal.Freespace.FreespaceManager 
			newFM)
		{
			if (!Started())
			{
				return;
			}
			Db4objects.Db4o.Foundation.IIntObjectVisitor addToNewFM = new _AnonymousInnerClass193
				(this, newFM);
			Db4objects.Db4o.Foundation.Tree.Traverse(_addressIx._indexTrans.GetRoot(), new _AnonymousInnerClass198
				(this, addToNewFM));
		}

		private sealed class _AnonymousInnerClass193 : Db4objects.Db4o.Foundation.IIntObjectVisitor
		{
			public _AnonymousInnerClass193(FreespaceManagerIx _enclosing, Db4objects.Db4o.Internal.Freespace.FreespaceManager
				 newFM)
			{
				this._enclosing = _enclosing;
				this.newFM = newFM;
			}

			public void Visit(int length, object address)
			{
				newFM.Free(((int)address), length);
			}

			private readonly FreespaceManagerIx _enclosing;

			private readonly Db4objects.Db4o.Internal.Freespace.FreespaceManager newFM;
		}

		private sealed class _AnonymousInnerClass198 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass198(FreespaceManagerIx _enclosing, Db4objects.Db4o.Foundation.IIntObjectVisitor
				 addToNewFM)
			{
				this._enclosing = _enclosing;
				this.addToNewFM = addToNewFM;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.Internal.IX.IxTree ixTree = (Db4objects.Db4o.Internal.IX.IxTree)a_object;
				ixTree.VisitAll(addToNewFM);
			}

			private readonly FreespaceManagerIx _enclosing;

			private readonly Db4objects.Db4o.Foundation.IIntObjectVisitor addToNewFM;
		}

		public override void OnNew(Db4objects.Db4o.Internal.LocalObjectContainer file)
		{
			file.EnsureFreespaceSlot();
		}

		public override void Read(int freespaceID)
		{
		}

		private void Remove(int address, int length)
		{
			_addressIx.Remove(address, length);
			_lengthIx.Remove(address, length);
		}

		public override void Start(int slotAddress)
		{
			if (Started())
			{
				return;
			}
			_slotAddress = slotAddress;
			Db4objects.Db4o.MetaIndex miAddress = new Db4objects.Db4o.MetaIndex();
			Db4objects.Db4o.MetaIndex miLength = new Db4objects.Db4o.MetaIndex();
			Db4objects.Db4o.Internal.Buffer reader = new Db4objects.Db4o.Internal.Buffer(SlotLength
				());
			reader.Read(_file, slotAddress, 0);
			miAddress.Read(reader);
			miLength.Read(reader);
			_addressIx = new Db4objects.Db4o.Internal.Freespace.FreespaceIxAddress(_file, miAddress
				);
			_lengthIx = new Db4objects.Db4o.Internal.Freespace.FreespaceIxLength(_file, miLength
				);
			_started = true;
		}

		private bool Started()
		{
			return _started;
		}

		public override byte SystemType()
		{
			return FM_IX;
		}

		public override int Write(bool shuttingDown)
		{
			return 0;
		}

		private void OverwriteDeletedSlots(int address, int length)
		{
			if (_overwriteDeletedSlots)
			{
				if (_xBytes == null)
				{
					length = length * BlockSize();
					_file.OverwriteDeletedBytes(address, length);
				}
				else
				{
					_xBytes.Add(new int[] { address, length });
				}
			}
		}
	}
}
