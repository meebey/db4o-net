namespace Db4objects.Db4o.Inside.Freespace
{
	public class FreespaceManagerRam : Db4objects.Db4o.Inside.Freespace.FreespaceManager
	{
		private readonly Db4objects.Db4o.TreeIntObject _finder = new Db4objects.Db4o.TreeIntObject
			(0);

		private Db4objects.Db4o.Foundation.Tree _freeByAddress;

		private Db4objects.Db4o.Foundation.Tree _freeBySize;

		public FreespaceManagerRam(Db4objects.Db4o.YapFile file) : base(file)
		{
		}

		public virtual void TraverseFreeSlots(Db4objects.Db4o.Foundation.IVisitor4 visitor
			)
		{
			Db4objects.Db4o.Foundation.Tree.Traverse(_freeByAddress, new _AnonymousInnerClass23
				(this, visitor));
		}

		private sealed class _AnonymousInnerClass23 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass23(FreespaceManagerRam _enclosing, Db4objects.Db4o.Foundation.IVisitor4
				 visitor)
			{
				this._enclosing = _enclosing;
				this.visitor = visitor;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Inside.Freespace.FreeSlotNode node = (Db4objects.Db4o.Inside.Freespace.FreeSlotNode
					)obj;
				int address = node._key;
				int length = node._peer._key;
				visitor.Visit(new Db4objects.Db4o.Inside.Slots.Slot(address, length));
			}

			private readonly FreespaceManagerRam _enclosing;

			private readonly Db4objects.Db4o.Foundation.IVisitor4 visitor;
		}

		private void AddFreeSlotNodes(int a_address, int a_length)
		{
			Db4objects.Db4o.Inside.Freespace.FreeSlotNode addressNode = new Db4objects.Db4o.Inside.Freespace.FreeSlotNode
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
			Db4objects.Db4o.Inside.Freespace.FreeSlotNode sizeNode;
			Db4objects.Db4o.Inside.Freespace.FreeSlotNode addressnode = (Db4objects.Db4o.Inside.Freespace.FreeSlotNode
				)Db4objects.Db4o.Foundation.Tree.FindSmaller(_freeByAddress, _finder);
			if ((addressnode != null) && ((addressnode._key + addressnode._peer._key) == a_address
				))
			{
				sizeNode = addressnode._peer;
				_freeBySize = _freeBySize.RemoveNode(sizeNode);
				sizeNode._key += a_length;
				Db4objects.Db4o.Inside.Freespace.FreeSlotNode secondAddressNode = (Db4objects.Db4o.Inside.Freespace.FreeSlotNode
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
				addressnode = (Db4objects.Db4o.Inside.Freespace.FreeSlotNode)Db4objects.Db4o.Foundation.Tree
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
			Db4objects.Db4o.Foundation.Tree.Traverse(_freeBySize, new _AnonymousInnerClass137
				(this, mint));
			return mint.Value();
		}

		private sealed class _AnonymousInnerClass137 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass137(FreespaceManagerRam _enclosing, Db4objects.Db4o.Foundation.MutableInt
				 mint)
			{
				this._enclosing = _enclosing;
				this.mint = mint;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Inside.Freespace.FreeSlotNode node = (Db4objects.Db4o.Inside.Freespace.FreeSlotNode
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
			_freeBySize = Db4objects.Db4o.Inside.Freespace.FreeSlotNode.RemoveGreaterOrEqual(
				(Db4objects.Db4o.Inside.Freespace.FreeSlotNode)_freeBySize, _finder);
			if (_finder._object == null)
			{
				return 0;
			}
			Db4objects.Db4o.Inside.Freespace.FreeSlotNode node = (Db4objects.Db4o.Inside.Freespace.FreeSlotNode
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

		public override void Migrate(Db4objects.Db4o.Inside.Freespace.FreespaceManager newFM
			)
		{
			if (_freeByAddress != null)
			{
				_freeByAddress.Traverse(new _AnonymousInnerClass181(this, newFM));
			}
		}

		private sealed class _AnonymousInnerClass181 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass181(FreespaceManagerRam _enclosing, Db4objects.Db4o.Inside.Freespace.FreespaceManager
				 newFM)
			{
				this._enclosing = _enclosing;
				this.newFM = newFM;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.Inside.Freespace.FreeSlotNode fsn = (Db4objects.Db4o.Inside.Freespace.FreeSlotNode
					)a_object;
				int address = fsn._key;
				int length = fsn._peer._key;
				newFM.Free(address, length);
			}

			private readonly FreespaceManagerRam _enclosing;

			private readonly Db4objects.Db4o.Inside.Freespace.FreespaceManager newFM;
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
			Db4objects.Db4o.YapWriter reader = _file.ReadWriterByID(Trans(), freeSlotsID);
			if (reader == null)
			{
				return;
			}
			Db4objects.Db4o.Inside.Freespace.FreeSlotNode.sizeLimit = DiscardLimit();
			_freeBySize = new Db4objects.Db4o.TreeReader(reader, new Db4objects.Db4o.Inside.Freespace.FreeSlotNode
				(0), true).Read();
			Db4objects.Db4o.Foundation.Tree[] addressTree = new Db4objects.Db4o.Foundation.Tree
				[1];
			if (_freeBySize != null)
			{
				_freeBySize.Traverse(new _AnonymousInnerClass210(this, addressTree));
			}
			_freeByAddress = addressTree[0];
			_file.Free(freeSlotsID, Db4objects.Db4o.YapConst.POINTER_LENGTH);
			_file.Free(reader.GetAddress(), reader.GetLength());
		}

		private sealed class _AnonymousInnerClass210 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass210(FreespaceManagerRam _enclosing, Db4objects.Db4o.Foundation.Tree[]
				 addressTree)
			{
				this._enclosing = _enclosing;
				this.addressTree = addressTree;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.Inside.Freespace.FreeSlotNode node = ((Db4objects.Db4o.Inside.Freespace.FreeSlotNode
					)a_object)._peer;
				addressTree[0] = Db4objects.Db4o.Foundation.Tree.Add(addressTree[0], node);
			}

			private readonly FreespaceManagerRam _enclosing;

			private readonly Db4objects.Db4o.Foundation.Tree[] addressTree;
		}

		public override void Start(int slotAddress)
		{
		}

		public override byte SystemType()
		{
			return FM_RAM;
		}

		private Db4objects.Db4o.Transaction Trans()
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
			int length = Db4objects.Db4o.TreeInt.ByteCount((Db4objects.Db4o.TreeInt)_freeBySize
				);
			Db4objects.Db4o.Inside.Slots.Pointer4 ptr = _file.NewSlot(Trans(), length);
			freeBySizeID = ptr._id;
			Db4objects.Db4o.YapWriter sdwriter = new Db4objects.Db4o.YapWriter(Trans(), length
				);
			sdwriter.UseSlot(freeBySizeID, ptr._address, length);
			Db4objects.Db4o.TreeInt.Write(sdwriter, (Db4objects.Db4o.TreeInt)_freeBySize);
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
