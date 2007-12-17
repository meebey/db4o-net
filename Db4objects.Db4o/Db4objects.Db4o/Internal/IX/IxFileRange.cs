/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Text;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.IX;

namespace Db4objects.Db4o.Internal.IX
{
	/// <summary>A range of index entries in the database file.</summary>
	/// <remarks>A range of index entries in the database file.</remarks>
	internal class IxFileRange : IxTree
	{
		internal readonly int _address;

		internal int _addressOffset;

		internal int _entries;

		private int[] _lowerAndUpperMatches;

		public IxFileRange(IndexTransaction a_ft, int a_address, int addressOffset, int a_entries
			) : base(a_ft)
		{
			_address = a_address;
			_addressOffset = addressOffset;
			_entries = a_entries;
			_size = a_entries;
		}

		public override Tree Add(Tree a_new)
		{
			return Reader().Add(this, a_new);
		}

		public override int Compare(Tree a_to)
		{
			_lowerAndUpperMatches = new int[2];
			return Reader().Compare(this, _lowerAndUpperMatches);
		}

		internal override int[] LowerAndUpperMatch()
		{
			return _lowerAndUpperMatches;
		}

		private IxFileRangeReader Reader()
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
			BufferImpl fileReader = new BufferImpl(SlotLength());
			StringBuilder sb = new StringBuilder();
			sb.Append("IxFileRange");
			VisitAll(new _IIntObjectVisitor_62(this, sb));
			return sb.ToString();
		}

		private sealed class _IIntObjectVisitor_62 : IIntObjectVisitor
		{
			public _IIntObjectVisitor_62(IxFileRange _enclosing, StringBuilder sb)
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

			private readonly StringBuilder sb;
		}

		public override void Visit(object obj)
		{
			Visit((IVisitor4)obj, null);
		}

		/// <exception cref="IxException"></exception>
		public override void Visit(IVisitor4 visitor, int[] lowerUpper)
		{
			IxFileRangeReader frr = Reader();
			if (lowerUpper == null)
			{
				lowerUpper = new int[] { 0, _entries - 1 };
			}
			int count = lowerUpper[1] - lowerUpper[0] + 1;
			if (count > 0)
			{
				BufferImpl fileReader = new BufferImpl(count * frr._slotLength);
				int offset = _addressOffset + (lowerUpper[0] * frr._slotLength);
				fileReader.Read(Stream(), _address, offset);
				for (int i = lowerUpper[0]; i <= lowerUpper[1]; i++)
				{
					fileReader.IncrementOffset(frr._linkLegth);
					visitor.Visit(fileReader.ReadInt());
				}
			}
		}

		public override int Write(IIndexable4 a_handler, StatefulBuffer a_writer)
		{
			LocalObjectContainer yf = (LocalObjectContainer)a_writer.GetStream();
			int length = _entries * SlotLength();
			yf.Copy(_address, _addressOffset, a_writer.GetAddress(), a_writer.AddressOffset()
				, length);
			a_writer.MoveForward(length);
			return _entries;
		}

		/// <exception cref="IxException"></exception>
		public override void VisitAll(IIntObjectVisitor visitor)
		{
			LocalObjectContainer yf = Stream();
			Transaction transaction = Trans();
			BufferImpl fileReader = new BufferImpl(SlotLength());
			for (int i = 0; i < _entries; i++)
			{
				int address = _address + (i * SlotLength());
				fileReader.Read(yf, address, _addressOffset);
				fileReader._offset = 0;
				object obj = IxDeprecationHelper.ComparableObject(Handler(), transaction, Handler
					().ReadIndexEntry(fileReader));
				visitor.Visit(fileReader.ReadInt(), obj);
			}
		}

		public override void VisitFirst(FreespaceVisitor visitor)
		{
			if (_preceding != null)
			{
				((IxTree)_preceding).VisitFirst(visitor);
				if (visitor.Visited())
				{
					return;
				}
			}
			FreespaceVisit(visitor, 0);
		}

		public override void VisitLast(FreespaceVisitor visitor)
		{
			if (_subsequent != null)
			{
				((IxTree)_subsequent).VisitLast(visitor);
				if (visitor.Visited())
				{
					return;
				}
			}
			FreespaceVisit(visitor, _entries - 1);
		}

		public override void FreespaceVisit(FreespaceVisitor visitor, int index)
		{
			IxFileRangeReader frr = Reader();
			BufferImpl fileReader = new BufferImpl(frr._slotLength);
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
