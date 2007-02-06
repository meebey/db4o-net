namespace Db4objects.Db4o.Internal.IX
{
	/// <summary>A range of index entries in the database file.</summary>
	/// <remarks>A range of index entries in the database file.</remarks>
	internal class IxFileRange : Db4objects.Db4o.Internal.IX.IxTree
	{
		internal readonly int _address;

		internal int _addressOffset;

		internal int _entries;

		private int[] _lowerAndUpperMatches;

		public IxFileRange(Db4objects.Db4o.Internal.IX.IndexTransaction a_ft, int a_address
			, int addressOffset, int a_entries) : base(a_ft)
		{
			_address = a_address;
			_addressOffset = addressOffset;
			_entries = a_entries;
			_size = a_entries;
		}

		public override Db4objects.Db4o.Foundation.Tree Add(Db4objects.Db4o.Foundation.Tree
			 a_new)
		{
			return Reader().Add(this, a_new);
		}

		public override int Compare(Db4objects.Db4o.Foundation.Tree a_to)
		{
			_lowerAndUpperMatches = new int[2];
			return Reader().Compare(this, _lowerAndUpperMatches);
		}

		internal override int[] LowerAndUpperMatch()
		{
			return _lowerAndUpperMatches;
		}

		private Db4objects.Db4o.Internal.IX.IxFileRangeReader Reader()
		{
			return _fieldTransaction.i_index.FileRangeReader();
		}

		public virtual void IncrementAddress(int length)
		{
			_addressOffset += length;
		}

		public override int OwnSize()
		{
			return _entries;
		}

		public override string ToString()
		{
			return base.ToString();
			Db4objects.Db4o.Internal.Buffer fileReader = new Db4objects.Db4o.Internal.Buffer(
				SlotLength());
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("IxFileRange");
			VisitAll(new _AnonymousInnerClass59(this, sb));
			return sb.ToString();
		}

		private sealed class _AnonymousInnerClass59 : Db4objects.Db4o.Foundation.IIntObjectVisitor
		{
			public _AnonymousInnerClass59(IxFileRange _enclosing, System.Text.StringBuilder sb
				)
			{
				this._enclosing = _enclosing;
				this.sb = sb;
			}

			public void Visit(int anInt, object anObject)
			{
				sb.Append("\n  ");
				sb.Append("Parent: " + anInt);
				sb.Append("\n ");
				sb.Append(anObject);
			}

			private readonly IxFileRange _enclosing;

			private readonly System.Text.StringBuilder sb;
		}

		public override void Visit(object obj)
		{
			Visit((Db4objects.Db4o.Foundation.IVisitor4)obj, null);
		}

		public override void Visit(Db4objects.Db4o.Foundation.IVisitor4 visitor, int[] lowerUpper
			)
		{
			Db4objects.Db4o.Internal.IX.IxFileRangeReader frr = Reader();
			if (lowerUpper == null)
			{
				lowerUpper = new int[] { 0, _entries - 1 };
			}
			int count = lowerUpper[1] - lowerUpper[0] + 1;
			if (count > 0)
			{
				Db4objects.Db4o.Internal.Buffer fileReader = new Db4objects.Db4o.Internal.Buffer(
					count * frr._slotLength);
				fileReader.Read(Stream(), _address, _addressOffset + (lowerUpper[0] * frr._slotLength
					));
				for (int i = lowerUpper[0]; i <= lowerUpper[1]; i++)
				{
					fileReader.IncrementOffset(frr._linkLegth);
					visitor.Visit(fileReader.ReadInt());
				}
			}
		}

		public override int Write(Db4objects.Db4o.Internal.IX.IIndexable4 a_handler, Db4objects.Db4o.Internal.StatefulBuffer
			 a_writer)
		{
			Db4objects.Db4o.Internal.LocalObjectContainer yf = (Db4objects.Db4o.Internal.LocalObjectContainer
				)a_writer.GetStream();
			int length = _entries * SlotLength();
			yf.Copy(_address, _addressOffset, a_writer.GetAddress(), a_writer.AddressOffset()
				, length);
			a_writer.MoveForward(length);
			return _entries;
		}

		public override void VisitAll(Db4objects.Db4o.Foundation.IIntObjectVisitor visitor
			)
		{
			Db4objects.Db4o.Internal.LocalObjectContainer yf = Stream();
			Db4objects.Db4o.Internal.Transaction transaction = Trans();
			Db4objects.Db4o.Internal.Buffer fileReader = new Db4objects.Db4o.Internal.Buffer(
				SlotLength());
			for (int i = 0; i < _entries; i++)
			{
				int address = _address + (i * SlotLength());
				fileReader.Read(yf, address, _addressOffset);
				fileReader._offset = 0;
				object obj = Handler().ComparableObject(transaction, Handler().ReadIndexEntry(fileReader
					));
				visitor.Visit(fileReader.ReadInt(), obj);
			}
		}

		public override void VisitFirst(Db4objects.Db4o.Internal.Freespace.FreespaceVisitor
			 visitor)
		{
			if (_preceding != null)
			{
				((Db4objects.Db4o.Internal.IX.IxTree)_preceding).VisitFirst(visitor);
				if (visitor.Visited())
				{
					return;
				}
			}
			FreespaceVisit(visitor, 0);
		}

		public override void VisitLast(Db4objects.Db4o.Internal.Freespace.FreespaceVisitor
			 visitor)
		{
			if (_subsequent != null)
			{
				((Db4objects.Db4o.Internal.IX.IxTree)_subsequent).VisitLast(visitor);
				if (visitor.Visited())
				{
					return;
				}
			}
			FreespaceVisit(visitor, _entries - 1);
		}

		public override void FreespaceVisit(Db4objects.Db4o.Internal.Freespace.FreespaceVisitor
			 visitor, int index)
		{
			Db4objects.Db4o.Internal.IX.IxFileRangeReader frr = Reader();
			Db4objects.Db4o.Internal.Buffer fileReader = new Db4objects.Db4o.Internal.Buffer(
				frr._slotLength);
			fileReader.Read(Stream(), _address, _addressOffset + (index * frr._slotLength));
			int val = fileReader.ReadInt();
			int parentID = fileReader.ReadInt();
			visitor.Visit(parentID, val);
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Internal.IX.IxFileRange range = new Db4objects.Db4o.Internal.IX.IxFileRange
				(_fieldTransaction, _address, _addressOffset, _entries);
			base.ShallowCloneInternal(range);
			if (_lowerAndUpperMatches != null)
			{
				range._lowerAndUpperMatches = new int[] { _lowerAndUpperMatches[0], _lowerAndUpperMatches
					[1] };
			}
			return range;
		}
	}
}
