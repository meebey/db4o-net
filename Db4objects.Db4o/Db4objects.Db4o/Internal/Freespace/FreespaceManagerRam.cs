namespace Db4objects.Db4o.Internal.Freespace
{
	public class FreespaceManagerRam : Db4objects.Db4o.Internal.Freespace.FreespaceManager
	{
		private readonly Db4objects.Db4o.Internal.TreeIntObject _finder = new Db4objects.Db4o.Internal.TreeIntObject
			(0);

		private Db4objects.Db4o.Foundation.Tree _freeByAddress;

		private Db4objects.Db4o.Foundation.Tree _freeBySize;

		public FreespaceManagerRam(Db4objects.Db4o.Internal.LocalObjectContainer file) : 
			base(file)
		{
		}

		public virtual void TraverseFreeSlots(Db4objects.Db4o.Foundation.IVisitor4 visitor
			)
		{
			Db4objects.Db4o.Foundation.Tree.Traverse(_freeByAddress, new _AnonymousInnerClass24
				(this, visitor));
		}

		private sealed class _AnonymousInnerClass24 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass24(FreespaceManagerRam _enclosing, Db4objects.Db4o.Foundation.IVisitor4
				 visitor)
			{
				this._enclosing = _enclosing;
				this.visitor = visitor;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Internal.Freespace.FreeSlotNode node = (Db4objects.Db4o.Internal.Freespace.FreeSlotNode
					)obj;
				int address = node._key;
				int length = node._peer._key;
				visitor.Visit(new Db4objects.Db4o.Internal.Slots.Slot(address, length));
			}

			private readonly FreespaceManagerRam _enclosing;

			private readonly Db4objects.Db4o.Foundation.IVisitor4 visitor;
		}

		private void AddFreeSlotNodes(int a_address, int a_length)
		{
			Db4objects.Db4o.Internal.Freespace.FreeSlotNode addressNode = new Db4objects.Db4o.Internal.Freespace.FreeSlotNode
				(a_address);
			addressNode.CreatePeer(a_length);
			_freeByAddress = Db4objects.Db4o.Foundation.Tree.Add(_freeByAddress, addressNode);
			_freeBySize = Db4objects.Db4o.Foundation.Tree.Add(_freeBySize, addressNode._peer);
		}

		public override void BeginCommit()
		{
		}

		public override void Debug()
		{
		}

		public override void EndCommit()
		{
		}

		public override void Free(int a_address, int a_length)
		{
			if (a_address <= 0)
			{
				return;
			}
			if (a_length <= DiscardLimit())
			{
				return;
			}
			a_length = _file.BlocksFor(a_length);
			_finder._key = a_address;
			Db4objects.Db4o.Internal.Freespace.FreeSlotNode sizeNode;
			Db4objects.Db4o.Internal.Freespace.FreeSlotNode addressnode = (Db4objects.Db4o.Internal.Freespace.FreeSlotNode
				)Db4objects.Db4o.Foundation.Tree.FindSmaller(_freeByAddress, _finder);
			if ((addressnode != null) && ((addressnode._key + addressnode._peer._key) == a_address
				))
			{
				sizeNode = addressnode._peer;
				_freeBySize = _freeBySize.RemoveNode(sizeNode);
				sizeNode._key += a_length;
				Db4objects.Db4o.Internal.Freespace.FreeSlotNode secondAddressNode = (Db4objects.Db4o.Internal.Freespace.FreeSlotNode
					)Db4objects.Db4o.Foundation.Tree.FindGreaterOrEqual(_freeByAddress, _finder);
				if ((secondAddressNode != null) && (a_address + a_length == secondAddressNode._key
					))
				{
					sizeNode._key += secondAddressNode._peer._key;
					_freeBySize = _freeBySize.RemoveNode(secondAddressNode._peer);
					_freeByAddress = _freeByAddress.RemoveNode(secondAddressNode);
				}
				sizeNode.RemoveChildren();
				_freeBySize = Db4objects.Db4o.Foundation.Tree.Add(_freeBySize, sizeNode);
			}
			else
			{
				addressnode = (Db4objects.Db4o.Internal.Freespace.FreeSlotNode)Db4objects.Db4o.Foundation.Tree
					.FindGreaterOrEqual(_freeByAddress, _finder);
				if ((addressnode != null) && (a_address + a_length == addressnode._key))
				{
					sizeNode = addressnode._peer;
					_freeByAddress = _freeByAddress.RemoveNode(addressnode);
					_freeBySize = _freeBySize.RemoveNode(sizeNode);
					sizeNode._key += a_length;
					addressnode._key = a_address;
					addressnode.RemoveChildren();
					sizeNode.RemoveChildren();
					_freeByAddress = Db4objects.Db4o.Foundation.Tree.Add(_freeByAddress, addressnode);
					_freeBySize = Db4objects.Db4o.Foundation.Tree.Add(_freeBySize, sizeNode);
				}
				else
				{
					AddFreeSlotNodes(a_address, a_length);
				}
			}
		}

		public override void FreeSelf()
		{
		}

		public override int FreeSize()
		{
			Db4objects.Db4o.Foundation.MutableInt mint = new Db4objects.Db4o.Foundation.MutableInt
				();
			Db4objects.Db4o.Foundation.Tree.Traverse(_freeBySize, new _AnonymousInnerClass138
				(this, mint));
			return mint.Value();
		}

		private sealed class _AnonymousInnerClass138 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass138(FreespaceManagerRam _enclosing, Db4objects.Db4o.Foundation.MutableInt
				 mint)
			{
				this._enclosing = _enclosing;
				this.mint = mint;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Internal.Freespace.FreeSlotNode node = (Db4objects.Db4o.Internal.Freespace.FreeSlotNode
					)obj;
				mint.Add(node._key);
			}

			private readonly FreespaceManagerRam _enclosing;

			private readonly Db4objects.Db4o.Foundation.MutableInt mint;
		}

		public override int GetSlot(int length)
		{
			int address = GetSlot1(length);
			if (address != 0)
			{
			}
			return address;
		}

		public virtual int GetSlot1(int length)
		{
			length = _file.BlocksFor(length);
			_finder._key = length;
			_finder._object = null;
			_freeBySize = Db4objects.Db4o.Internal.Freespace.FreeSlotNode.RemoveGreaterOrEqual
				((Db4objects.Db4o.Internal.Freespace.FreeSlotNode)_freeBySize, _finder);
			if (_finder._object == null)
			{
				return 0;
			}
			Db4objects.Db4o.Internal.Freespace.FreeSlotNode node = (Db4objects.Db4o.Internal.Freespace.FreeSlotNode
				)_finder._object;
			int blocksFound = node._key;
			int address = node._peer._key;
			_freeByAddress = _freeByAddress.RemoveNode(node._peer);
			if (blocksFound > length)
			{
				AddFreeSlotNodes(address + length, blocksFound - length);
			}
			return address;
		}

		public override void Migrate(Db4objects.Db4o.Internal.Freespace.FreespaceManager 
			newFM)
		{
			if (_freeByAddress != null)
			{
				_freeByAddress.Traverse(new _AnonymousInnerClass182(this, newFM));
			}
		}

		private sealed class _AnonymousInnerClass182 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass182(FreespaceManagerRam _enclosing, Db4objects.Db4o.Internal.Freespace.FreespaceManager
				 newFM)
			{
				this._enclosing = _enclosing;
				this.newFM = newFM;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.Internal.Freespace.FreeSlotNode fsn = (Db4objects.Db4o.Internal.Freespace.FreeSlotNode
					)a_object;
				int address = fsn._key;
				int length = fsn._peer._key;
				newFM.Free(address, length);
			}

			private readonly FreespaceManagerRam _enclosing;

			private readonly Db4objects.Db4o.Internal.Freespace.FreespaceManager newFM;
		}

		public override void OnNew(Db4objects.Db4o.Internal.LocalObjectContainer file)
		{
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
			Db4objects.Db4o.Internal.StatefulBuffer reader = _file.ReadWriterByID(Trans(), freeSlotsID
				);
			if (reader == null)
			{
				return;
			}
			Db4objects.Db4o.Internal.Freespace.FreeSlotNode.sizeLimit = DiscardLimit();
			_freeBySize = new Db4objects.Db4o.Internal.TreeReader(reader, new Db4objects.Db4o.Internal.Freespace.FreeSlotNode
				(0), true).Read();
			Db4objects.Db4o.Foundation.Tree.ByRef addressTree = new Db4objects.Db4o.Foundation.Tree.ByRef
				();
			if (_freeBySize != null)
			{
				_freeBySize.Traverse(new _AnonymousInnerClass215(this, addressTree));
			}
			_freeByAddress = addressTree.value;
			_file.Free(freeSlotsID, Db4objects.Db4o.Internal.Const4.POINTER_LENGTH);
			_file.Free(reader.GetAddress(), reader.GetLength());
		}

		private sealed class _AnonymousInnerClass215 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass215(FreespaceManagerRam _enclosing, Db4objects.Db4o.Foundation.Tree.ByRef
				 addressTree)
			{
				this._enclosing = _enclosing;
				this.addressTree = addressTree;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.Internal.Freespace.FreeSlotNode node = ((Db4objects.Db4o.Internal.Freespace.FreeSlotNode
					)a_object)._peer;
				addressTree.value = Db4objects.Db4o.Foundation.Tree.Add(addressTree.value, node);
			}

			private readonly FreespaceManagerRam _enclosing;

			private readonly Db4objects.Db4o.Foundation.Tree.ByRef addressTree;
		}

		public override void Start(int slotAddress)
		{
		}

		public override byte SystemType()
		{
			return FM_RAM;
		}

		private Db4objects.Db4o.Internal.Transaction Trans()
		{
			return _file.GetSystemTransaction();
		}

		public override int Write(bool shuttingDown)
		{
			if (!shuttingDown)
			{
				return 0;
			}
			int freeBySizeID = 0;
			int length = Db4objects.Db4o.Internal.TreeInt.ByteCount((Db4objects.Db4o.Internal.TreeInt
				)_freeBySize);
			Db4objects.Db4o.Internal.Slots.Pointer4 ptr = _file.NewSlot(Trans(), length);
			freeBySizeID = ptr._id;
			Db4objects.Db4o.Internal.StatefulBuffer sdwriter = new Db4objects.Db4o.Internal.StatefulBuffer
				(Trans(), length);
			sdwriter.UseSlot(freeBySizeID, ptr._address, length);
			Db4objects.Db4o.Internal.TreeInt.Write(sdwriter, (Db4objects.Db4o.Internal.TreeInt
				)_freeBySize);
			sdwriter.WriteEncrypt();
			Trans().WritePointer(ptr._id, ptr._address, length);
			return freeBySizeID;
		}

		public override int EntryCount()
		{
			return Db4objects.Db4o.Foundation.Tree.Size(_freeByAddress);
		}
	}
}
