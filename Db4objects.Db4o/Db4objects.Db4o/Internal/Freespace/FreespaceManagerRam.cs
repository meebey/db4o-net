using System;
using System.Text;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Freespace
{
	public class FreespaceManagerRam : AbstractFreespaceManager
	{
		private readonly TreeIntObject _finder = new TreeIntObject(0);

		private Tree _freeByAddress;

		private Tree _freeBySize;

		public FreespaceManagerRam(LocalObjectContainer file) : base(file)
		{
		}

		public virtual void TraverseFreeSlots(IVisitor4 visitor)
		{
			Tree.Traverse(_freeByAddress, new _AnonymousInnerClass24(this, visitor));
		}

		private sealed class _AnonymousInnerClass24 : IVisitor4
		{
			public _AnonymousInnerClass24(FreespaceManagerRam _enclosing, IVisitor4 visitor)
			{
				this._enclosing = _enclosing;
				this.visitor = visitor;
			}

			public void Visit(object obj)
			{
				FreeSlotNode node = (FreeSlotNode)obj;
				int address = node._key;
				int length = node._peer._key;
				visitor.Visit(new Slot(address, length));
			}

			private readonly FreespaceManagerRam _enclosing;

			private readonly IVisitor4 visitor;
		}

		private void AddFreeSlotNodes(int a_address, int a_length)
		{
			FreeSlotNode addressNode = new FreeSlotNode(a_address);
			addressNode.CreatePeer(a_length);
			_freeByAddress = Tree.Add(_freeByAddress, addressNode);
			_freeBySize = Tree.Add(_freeBySize, addressNode._peer);
		}

		public override void BeginCommit()
		{
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("RAM FreespaceManager\n");
			sb.Append("Address Index\n");
			_freeByAddress.Traverse(new _AnonymousInnerClass49(this, sb));
			sb.Append("Length Index\n");
			_freeBySize.Traverse(new _AnonymousInnerClass57(this, sb));
			return sb.ToString();
		}

		private sealed class _AnonymousInnerClass49 : IVisitor4
		{
			public _AnonymousInnerClass49(FreespaceManagerRam _enclosing, StringBuilder sb)
			{
				this._enclosing = _enclosing;
				this.sb = sb;
			}

			public void Visit(object obj)
			{
				sb.Append(obj);
				sb.Append("\n");
			}

			private readonly FreespaceManagerRam _enclosing;

			private readonly StringBuilder sb;
		}

		private sealed class _AnonymousInnerClass57 : IVisitor4
		{
			public _AnonymousInnerClass57(FreespaceManagerRam _enclosing, StringBuilder sb)
			{
				this._enclosing = _enclosing;
				this.sb = sb;
			}

			public void Visit(object obj)
			{
				sb.Append(obj);
				sb.Append("\n");
			}

			private readonly FreespaceManagerRam _enclosing;

			private readonly StringBuilder sb;
		}

		public override void EndCommit()
		{
		}

		public override void Free(Slot slot)
		{
			int address = slot._address;
			int length = slot._length;
			if (address <= 0)
			{
				throw new ArgumentException();
			}
			if (length <= DiscardLimit())
			{
				return;
			}
			length = _file.BlocksFor(length);
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
					_freeBySize = _freeBySize.RemoveNode(secondAddressNode._peer);
					_freeByAddress = _freeByAddress.RemoveNode(secondAddressNode);
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
					_freeByAddress = _freeByAddress.RemoveNode(addressnode);
					_freeBySize = _freeBySize.RemoveNode(sizeNode);
					sizeNode._key += length;
					addressnode._key = address;
					addressnode.RemoveChildren();
					sizeNode.RemoveChildren();
					_freeByAddress = Tree.Add(_freeByAddress, addressnode);
					_freeBySize = Tree.Add(_freeBySize, sizeNode);
				}
				else
				{
					AddFreeSlotNodes(address, length);
				}
			}
			_file.OverwriteDeletedSlot(slot);
		}

		public override void FreeSelf()
		{
		}

		public override Slot GetSlot(int length)
		{
			int requiredLength = _file.BlocksFor(length);
			_finder._key = requiredLength;
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
			if (blocksFound > requiredLength)
			{
				AddFreeSlotNodes(address + requiredLength, blocksFound - requiredLength);
			}
			return new Slot(address, length);
		}

		public override void Traverse(IVisitor4 visitor)
		{
			if (_freeByAddress == null)
			{
				return;
			}
			_freeByAddress.Traverse(new _AnonymousInnerClass167(this, visitor));
		}

		private sealed class _AnonymousInnerClass167 : IVisitor4
		{
			public _AnonymousInnerClass167(FreespaceManagerRam _enclosing, IVisitor4 visitor)
			{
				this._enclosing = _enclosing;
				this.visitor = visitor;
			}

			public void Visit(object a_object)
			{
				FreeSlotNode fsn = (FreeSlotNode)a_object;
				int address = fsn._key;
				int length = fsn._peer._key;
				visitor.Visit(new Slot(address, length));
			}

			private readonly FreespaceManagerRam _enclosing;

			private readonly IVisitor4 visitor;
		}

		public override int OnNew(LocalObjectContainer file)
		{
			return 0;
		}

		public override void Read(int freeSlotsID)
		{
			if (freeSlotsID <= 0)
			{
				return;
			}
			if (DiscardLimit() == int.MaxValue)
			{
				return;
			}
			StatefulBuffer reader = _file.ReadWriterByID(Trans(), freeSlotsID);
			if (reader == null)
			{
				return;
			}
			FreeSlotNode.sizeLimit = DiscardLimit();
			_freeBySize = new TreeReader(reader, new FreeSlotNode(0), true).Read();
			Tree.ByRef addressTree = new Tree.ByRef();
			if (_freeBySize != null)
			{
				_freeBySize.Traverse(new _AnonymousInnerClass200(this, addressTree));
			}
			_freeByAddress = addressTree.value;
			_file.Free(freeSlotsID, Const4.POINTER_LENGTH);
			_file.Free(reader.GetAddress(), reader.GetLength());
		}

		private sealed class _AnonymousInnerClass200 : IVisitor4
		{
			public _AnonymousInnerClass200(FreespaceManagerRam _enclosing, Tree.ByRef addressTree
				)
			{
				this._enclosing = _enclosing;
				this.addressTree = addressTree;
			}

			public void Visit(object a_object)
			{
				FreeSlotNode node = ((FreeSlotNode)a_object)._peer;
				addressTree.value = Tree.Add(addressTree.value, node);
			}

			private readonly FreespaceManagerRam _enclosing;

			private readonly Tree.ByRef addressTree;
		}

		public override void Start(int slotAddress)
		{
		}

		public override byte SystemType()
		{
			return FM_RAM;
		}

		private LocalTransaction Trans()
		{
			return (LocalTransaction)_file.SystemTransaction();
		}

		public override int Write()
		{
			int freeBySizeID = 0;
			int length = TreeInt.ByteCount((TreeInt)_freeBySize);
			Pointer4 ptr = _file.NewSlot(Trans(), length);
			freeBySizeID = ptr._id;
			StatefulBuffer sdwriter = new StatefulBuffer(Trans(), length);
			sdwriter.UseSlot(freeBySizeID, ptr._address, length);
			TreeInt.Write(sdwriter, (TreeInt)_freeBySize);
			sdwriter.WriteEncrypt();
			Trans().WritePointer(ptr._id, ptr._address, length);
			return freeBySizeID;
		}

		public override int SlotCount()
		{
			return Tree.Size(_freeByAddress);
		}
	}
}
