/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Freespace
{
	public class RamFreespaceManager : Db4objects.Db4o.Internal.Freespace.AbstractFreespaceManager
	{
		private readonly TreeIntObject _finder = new TreeIntObject(0);

		private Tree _freeByAddress;

		private Tree _freeBySize;

		public RamFreespaceManager(LocalObjectContainer file) : base(file)
		{
		}

		private void AddFreeSlotNodes(int address, int length)
		{
			FreeSlotNode addressNode = new FreeSlotNode(address);
			addressNode.CreatePeer(length);
			_freeByAddress = Tree.Add(_freeByAddress, addressNode);
			_freeBySize = Tree.Add(_freeBySize, addressNode._peer);
		}

		public override Slot AllocateTransactionLogSlot(int length)
		{
			FreeSlotNode sizeNode = (FreeSlotNode)Tree.Last(_freeBySize);
			if (sizeNode == null || sizeNode._key < length)
			{
				return null;
			}
			// We can just be appending to the end of the file, using one
			// really big contigous slot that keeps growing. Let's limit.
			int limit = length + 100;
			if (sizeNode._key > limit)
			{
				return GetSlot(limit);
			}
			RemoveFromBothTrees(sizeNode);
			return new Slot(sizeNode._peer._key, sizeNode._key);
		}

		public override void FreeTransactionLogSlot(Slot slot)
		{
			Free(slot);
		}

		public override void BeginCommit()
		{
		}

		// do nothing
		public override void Commit()
		{
		}

		// do nothing
		public override void EndCommit()
		{
		}

		// do nothing
		public override void Free(Slot slot)
		{
			int address = slot.Address();
			int length = slot.Length();
			if (address <= 0)
			{
				throw new ArgumentException();
			}
			if (DTrace.enabled)
			{
				DTrace.FreespacemanagerRamFree.LogLength(address, length);
			}
			_finder._key = address;
			FreeSlotNode sizeNode;
			FreeSlotNode addressnode = (FreeSlotNode)Tree.FindSmaller(_freeByAddress, _finder
				);
			if ((addressnode != null) && ((addressnode._key + addressnode._peer._key) == address
				))
			{
				sizeNode = addressnode._peer;
				_freeBySize = _freeBySize.RemoveNode(sizeNode);
				sizeNode._key += length;
				FreeSlotNode secondAddressNode = (FreeSlotNode)Tree.FindGreaterOrEqual(_freeByAddress
					, _finder);
				if ((secondAddressNode != null) && (address + length == secondAddressNode._key))
				{
					sizeNode._key += secondAddressNode._peer._key;
					RemoveFromBothTrees(secondAddressNode._peer);
				}
				sizeNode.RemoveChildren();
				_freeBySize = Tree.Add(_freeBySize, sizeNode);
			}
			else
			{
				addressnode = (FreeSlotNode)Tree.FindGreaterOrEqual(_freeByAddress, _finder);
				if ((addressnode != null) && (address + length == addressnode._key))
				{
					sizeNode = addressnode._peer;
					RemoveFromBothTrees(sizeNode);
					sizeNode._key += length;
					addressnode._key = address;
					addressnode.RemoveChildren();
					sizeNode.RemoveChildren();
					_freeByAddress = Tree.Add(_freeByAddress, addressnode);
					_freeBySize = Tree.Add(_freeBySize, sizeNode);
				}
				else
				{
					if (CanDiscard(length))
					{
						return;
					}
					AddFreeSlotNodes(address, length);
				}
			}
			_file.OverwriteDeletedBlockedSlot(slot);
		}

		public override void FreeSelf()
		{
		}

		// Do nothing.
		// The RAM manager frees itself on reading.
		private void FreeReader(StatefulBuffer reader)
		{
			_file.Free(reader.GetAddress(), reader.Length());
		}

		public override Slot GetSlot(int length)
		{
			_finder._key = length;
			_finder._object = null;
			_freeBySize = FreeSlotNode.RemoveGreaterOrEqual((FreeSlotNode)_freeBySize, _finder
				);
			if (_finder._object == null)
			{
				return null;
			}
			FreeSlotNode node = (FreeSlotNode)_finder._object;
			int blocksFound = node._key;
			int address = node._peer._key;
			_freeByAddress = _freeByAddress.RemoveNode(node._peer);
			int remainingBlocks = blocksFound - length;
			if (CanDiscard(remainingBlocks))
			{
				length = blocksFound;
			}
			else
			{
				AddFreeSlotNodes(address + length, remainingBlocks);
			}
			if (DTrace.enabled)
			{
				DTrace.FreespacemanagerGetSlot.LogLength(address, length);
			}
			return new Slot(address, length);
		}

		internal virtual int MarshalledLength()
		{
			return TreeInt.MarshalledLength((TreeInt)_freeBySize);
		}

		public override void Read(int freeSlotsID)
		{
			ReadById(freeSlotsID);
		}

		private void Read(StatefulBuffer reader)
		{
			FreeSlotNode.sizeLimit = BlockedDiscardLimit();
			_freeBySize = new TreeReader(reader, new FreeSlotNode(0), true).Read();
			Tree.ByRef addressTree = new Tree.ByRef();
			if (_freeBySize != null)
			{
				_freeBySize.Traverse(new _IVisitor4_168(addressTree));
			}
			_freeByAddress = addressTree.value;
		}

		private sealed class _IVisitor4_168 : IVisitor4
		{
			public _IVisitor4_168(Tree.ByRef addressTree)
			{
				this.addressTree = addressTree;
			}

			public void Visit(object a_object)
			{
				FreeSlotNode node = ((FreeSlotNode)a_object)._peer;
				addressTree.value = Tree.Add(addressTree.value, node);
			}

			private readonly Tree.ByRef addressTree;
		}

		internal virtual void Read(Slot slot)
		{
			if (slot.Address() == 0)
			{
				return;
			}
			StatefulBuffer reader = _file.ReadWriterByAddress(Transaction(), slot.Address(), 
				slot.Length());
			if (reader == null)
			{
				return;
			}
			Read(reader);
			FreeReader(reader);
		}

		private void ReadById(int freeSlotsID)
		{
			if (freeSlotsID <= 0)
			{
				return;
			}
			if (DiscardLimit() == int.MaxValue)
			{
				return;
			}
			StatefulBuffer reader = _file.ReadWriterByID(Transaction(), freeSlotsID);
			if (reader == null)
			{
				return;
			}
			Read(reader);
			_file.Free(freeSlotsID, Const4.PointerLength);
			FreeReader(reader);
		}

		private void RemoveFromBothTrees(FreeSlotNode sizeNode)
		{
			_freeBySize = _freeBySize.RemoveNode(sizeNode);
			_freeByAddress = _freeByAddress.RemoveNode(sizeNode._peer);
		}

		public override int SlotCount()
		{
			return Tree.Size(_freeByAddress);
		}

		public override void Start(int slotAddress)
		{
		}

		// this is done in read(), nothing to do here
		public override byte SystemType()
		{
			return FmRam;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("RAM FreespaceManager\n");
			sb.Append("Address Index\n");
			_freeByAddress.Traverse(new _IVisitor4_231(sb));
			sb.Append("Length Index\n");
			_freeBySize.Traverse(new _IVisitor4_239(sb));
			return sb.ToString();
		}

		private sealed class _IVisitor4_231 : IVisitor4
		{
			public _IVisitor4_231(StringBuilder sb)
			{
				this.sb = sb;
			}

			public void Visit(object obj)
			{
				sb.Append(obj);
				sb.Append("\n");
			}

			private readonly StringBuilder sb;
		}

		private sealed class _IVisitor4_239 : IVisitor4
		{
			public _IVisitor4_239(StringBuilder sb)
			{
				this.sb = sb;
			}

			public void Visit(object obj)
			{
				sb.Append(obj);
				sb.Append("\n");
			}

			private readonly StringBuilder sb;
		}

		public override void Traverse(IVisitor4 visitor)
		{
			if (_freeByAddress == null)
			{
				return;
			}
			_freeByAddress.Traverse(new _IVisitor4_252(visitor));
		}

		private sealed class _IVisitor4_252 : IVisitor4
		{
			public _IVisitor4_252(IVisitor4 visitor)
			{
				this.visitor = visitor;
			}

			public void Visit(object a_object)
			{
				FreeSlotNode fsn = (FreeSlotNode)a_object;
				int address = fsn._key;
				int length = fsn._peer._key;
				visitor.Visit(new Slot(address, length));
			}

			private readonly IVisitor4 visitor;
		}

		public override int Write()
		{
			Pointer4 pointer = _file.NewSlot(MarshalledLength());
			Write(pointer);
			return pointer._id;
		}

		internal virtual void Write(Pointer4 pointer)
		{
			StatefulBuffer buffer = new StatefulBuffer(Transaction(), pointer);
			TreeInt.Write(buffer, (TreeInt)_freeBySize);
			buffer.WriteEncrypt();
			Transaction().FlushFile();
			Transaction().WritePointer(pointer);
		}
	}
}
