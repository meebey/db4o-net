/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.IX;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Freespace
{
	public class FreespaceManagerIx : AbstractFreespaceManager
	{
		private int _slotAddress;

		private FreespaceIxAddress _addressIx;

		private FreespaceIxLength _lengthIx;

		private bool _started;

		private Collection4 _xBytes;

		private readonly bool _overwriteDeletedSlots;

		public FreespaceManagerIx(LocalObjectContainer file) : base(file)
		{
			_overwriteDeletedSlots = Debug.xbytes || file.Config().FreespaceFiller() != null;
		}

		private void Add(int address, int length)
		{
			_addressIx.Add(address, length);
			_lengthIx.Add(address, length);
		}

		public override Slot AllocateTransactionLogSlot(int length)
		{
			return GetSlot(length);
		}

		public override void FreeTransactionLogSlot(Slot slot)
		{
			Free(slot);
		}

		public override void BeginCommit()
		{
			if (!Started())
			{
				return;
			}
			SlotEntryToZeroes(_file, _slotAddress);
		}

		public override void EndCommit()
		{
			if (!Started())
			{
				return;
			}
			if (_overwriteDeletedSlots)
			{
				_xBytes = new Collection4();
			}
			_addressIx._index.CommitFreeSpace(_lengthIx._index);
			StatefulBuffer writer = new StatefulBuffer(_file.SystemTransaction(), _slotAddress
				, SlotLength());
			_addressIx._index._metaIndex.Write(writer);
			_lengthIx._index._metaIndex.Write(writer);
			if (_overwriteDeletedSlots)
			{
				writer.SetID(Const4.IGNORE_ID);
			}
			if (_file.ConfigImpl().FlushFileBuffers())
			{
				_file.SyncFiles();
			}
			writer.WriteEncrypt();
			if (_overwriteDeletedSlots)
			{
				IEnumerator i = _xBytes.GetEnumerator();
				_xBytes = null;
				while (i.MoveNext())
				{
					Slot slot = (Slot)i.Current;
					OverwriteDeletedSlot(slot);
				}
			}
		}

		public override int SlotCount()
		{
			return _addressIx.EntryCount();
		}

		public override void Free(Slot slot)
		{
			int address = slot.Address();
			int length = slot.Length();
			if (!Started())
			{
				return;
			}
			if (address <= 0)
			{
				return;
			}
			if (CanDiscard(length))
			{
				return;
			}
			if (DTrace.enabled)
			{
				DTrace.FREE.LogLength(address, length);
			}
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
				OverwriteDeletedSlot(new Slot(freedAddress, freedLength));
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

		public override Slot GetSlot(int length)
		{
			if (!Started())
			{
				return null;
			}
			int address = 0;
			_lengthIx.Find(length);
			if (_lengthIx.Match())
			{
				Remove(_lengthIx.Address(), _lengthIx.Length());
				address = _lengthIx.Address();
			}
			else
			{
				if (_lengthIx.Subsequent())
				{
					int lengthRemainder = _lengthIx.Length() - length;
					int addressRemainder = _lengthIx.Address() + length;
					Remove(_lengthIx.Address(), _lengthIx.Length());
					Add(addressRemainder, lengthRemainder);
					address = _lengthIx.Address();
				}
			}
			if (address == 0)
			{
				return null;
			}
			if (DTrace.enabled)
			{
				DTrace.GET_FREESPACE.LogLength(address, length);
			}
			return new Slot(address, length);
		}

		public override void MigrateTo(IFreespaceManager fm)
		{
			if (!Started())
			{
				return;
			}
			base.MigrateTo(fm);
		}

		public override void Traverse(IVisitor4 visitor)
		{
			if (!Started())
			{
				return;
			}
			IIntObjectVisitor dispatcher = new _IIntObjectVisitor_184(this, visitor);
			Tree.Traverse(_addressIx._indexTrans.GetRoot(), new _IVisitor4_189(this, dispatcher
				));
		}

		private sealed class _IIntObjectVisitor_184 : IIntObjectVisitor
		{
			public _IIntObjectVisitor_184(FreespaceManagerIx _enclosing, IVisitor4 visitor)
			{
				this._enclosing = _enclosing;
				this.visitor = visitor;
			}

			public void Visit(int length, object address)
			{
				visitor.Visit(new Slot(((int)address), length));
			}

			private readonly FreespaceManagerIx _enclosing;

			private readonly IVisitor4 visitor;
		}

		private sealed class _IVisitor4_189 : IVisitor4
		{
			public _IVisitor4_189(FreespaceManagerIx _enclosing, IIntObjectVisitor dispatcher
				)
			{
				this._enclosing = _enclosing;
				this.dispatcher = dispatcher;
			}

			public void Visit(object a_object)
			{
				IxTree ixTree = (IxTree)a_object;
				ixTree.VisitAll(dispatcher);
			}

			private readonly FreespaceManagerIx _enclosing;

			private readonly IIntObjectVisitor dispatcher;
		}

		public override int OnNew(LocalObjectContainer file)
		{
			return file.EnsureFreespaceSlot();
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
			MetaIndex miAddress = new MetaIndex();
			MetaIndex miLength = new MetaIndex();
			BufferImpl reader = new BufferImpl(SlotLength());
			reader.Read(_file, slotAddress, 0);
			miAddress.Read(reader);
			miLength.Read(reader);
			_addressIx = new FreespaceIxAddress(_file, miAddress);
			_lengthIx = new FreespaceIxLength(_file, miLength);
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

		public override int Write()
		{
			return 0;
		}

		private void OverwriteDeletedSlot(Slot slot)
		{
			if (_overwriteDeletedSlots)
			{
				if (_xBytes == null)
				{
					_file.OverwriteDeletedBlockedSlot(slot);
				}
				else
				{
					_xBytes.Add(slot);
				}
			}
		}

		public override string ToString()
		{
			string str = "FreespaceManagerIx\n" + _lengthIx.ToString();
			return str;
		}

		public override void Commit()
		{
		}
	}
}
