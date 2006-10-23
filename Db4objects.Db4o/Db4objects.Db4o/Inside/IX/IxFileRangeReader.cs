namespace Db4objects.Db4o.Inside.IX
{
	/// <exclude></exclude>
	internal class IxFileRangeReader
	{
		private int _baseAddress;

		private int _baseAddressOffset;

		private int _addressOffset;

		private readonly Db4objects.Db4o.Inside.IX.IIndexable4 _handler;

		private int _lower;

		private int _upper;

		private int _cursor;

		private readonly Db4objects.Db4o.YapReader _reader;

		internal readonly int _slotLength;

		internal readonly int _linkLegth;

		internal IxFileRangeReader(Db4objects.Db4o.Inside.IX.IIndexable4 handler)
		{
			_handler = handler;
			_linkLegth = handler.LinkLength();
			_slotLength = _linkLegth + Db4objects.Db4o.YapConst.INT_LENGTH;
			_reader = new Db4objects.Db4o.YapReader(_slotLength);
		}

		internal virtual Db4objects.Db4o.Foundation.Tree Add(Db4objects.Db4o.Inside.IX.IxFileRange
			 fileRange, Db4objects.Db4o.Foundation.Tree newTree)
		{
			SetFileRange(fileRange);
			Db4objects.Db4o.YapFile yf = fileRange.Stream();
			Db4objects.Db4o.Transaction trans = fileRange.Trans();
			while (true)
			{
				_reader.Read(yf, _baseAddress, _baseAddressOffset + _addressOffset);
				_reader._offset = 0;
				int cmp = Compare(trans);
				if (cmp == 0)
				{
					int parentID = _reader.ReadInt();
					cmp = parentID - ((Db4objects.Db4o.Inside.IX.IxPatch)newTree)._parentID;
				}
				if (cmp > 0)
				{
					_upper = _cursor - 1;
					if (_upper < _lower)
					{
						_upper = _lower;
					}
				}
				else
				{
					if (cmp < 0)
					{
						_lower = _cursor + 1;
						if (_lower > _upper)
						{
							_lower = _upper;
						}
					}
					else
					{
						if (newTree is Db4objects.Db4o.Inside.IX.IxRemove)
						{
							if (_cursor == 0)
							{
								newTree._preceding = fileRange._preceding;
								if (fileRange._entries == 1)
								{
									newTree._subsequent = fileRange._subsequent;
									return newTree.BalanceCheckNulls();
								}
								fileRange._entries--;
								fileRange.IncrementAddress(_slotLength);
								fileRange._preceding = null;
								newTree._subsequent = fileRange;
							}
							else
							{
								if (_cursor + 1 == fileRange._entries)
								{
									newTree._preceding = fileRange;
									newTree._subsequent = fileRange._subsequent;
									fileRange._subsequent = null;
									fileRange._entries--;
								}
								else
								{
									return Insert(fileRange, newTree, _cursor, 0);
								}
							}
							fileRange.CalculateSize();
							return newTree.BalanceCheckNulls();
						}
						if (_cursor == 0)
						{
							newTree._subsequent = fileRange;
							return newTree.RotateLeft();
						}
						else
						{
							if (_cursor == fileRange._entries)
							{
								newTree._preceding = fileRange;
								return newTree.RotateRight();
							}
						}
						return Insert(fileRange, newTree, _cursor, cmp);
					}
				}
				if (!AdjustCursor())
				{
					if (_cursor == 0 && cmp > 0)
					{
						return fileRange.Add(newTree, 1);
					}
					if (_cursor == fileRange._entries - 1 && cmp < 0)
					{
						return fileRange.Add(newTree, -1);
					}
					return Insert(fileRange, newTree, _cursor, cmp);
				}
			}
		}

		private bool AdjustCursor()
		{
			if (_upper < _lower)
			{
				return false;
			}
			int oldCursor = _cursor;
			_cursor = _lower + ((_upper - _lower) / 2);
			if (_cursor == oldCursor && _cursor == _lower && _lower < _upper)
			{
				_cursor++;
			}
			_addressOffset = _cursor * _slotLength;
			return _cursor != oldCursor;
		}

		internal virtual int Compare(Db4objects.Db4o.Inside.IX.IxFileRange fileRange, int[]
			 matches)
		{
			SetFileRange(fileRange);
			Db4objects.Db4o.YapFile yf = fileRange.Stream();
			Db4objects.Db4o.Transaction trans = fileRange.Trans();
			int res = 0;
			while (true)
			{
				_reader.Read(yf, _baseAddress, _baseAddressOffset + _addressOffset);
				_reader._offset = 0;
				int cmp = Compare(trans);
				if (cmp > 0)
				{
					_upper = _cursor - 1;
				}
				else
				{
					if (cmp < 0)
					{
						_lower = _cursor + 1;
					}
					else
					{
						break;
					}
				}
				if (!AdjustCursor())
				{
					if (_cursor <= 0)
					{
						if (!(cmp < 0 && fileRange._entries > 1))
						{
							res = cmp;
						}
					}
					else
					{
						if (_cursor >= (fileRange._entries - 1))
						{
							if (cmp < 0)
							{
								res = cmp;
							}
						}
					}
					break;
				}
			}
			matches[0] = _lower;
			matches[1] = _upper;
			if (_lower > _upper)
			{
				return res;
			}
			int tempCursor = _cursor;
			_upper = _cursor;
			AdjustCursor();
			while (true)
			{
				_reader.Read(yf, _baseAddress, _baseAddressOffset + _addressOffset);
				_reader._offset = 0;
				int cmp = Compare(trans);
				if (cmp == 0)
				{
					_upper = _cursor;
				}
				else
				{
					_lower = _cursor + 1;
					if (_lower > _upper)
					{
						matches[0] = _upper;
						break;
					}
				}
				if (!AdjustCursor())
				{
					matches[0] = _upper;
					break;
				}
			}
			_upper = matches[1];
			_lower = tempCursor;
			if (_lower > _upper)
			{
				_lower = _upper;
			}
			AdjustCursor();
			while (true)
			{
				_reader.Read(yf, _baseAddress, _baseAddressOffset + _addressOffset);
				_reader._offset = 0;
				int cmp = Compare(trans);
				if (cmp == 0)
				{
					_lower = _cursor;
				}
				else
				{
					_upper = _cursor - 1;
					if (_upper < _lower)
					{
						matches[1] = _lower;
						break;
					}
				}
				if (!AdjustCursor())
				{
					matches[1] = _lower;
					break;
				}
			}
			return res;
		}

		private int Compare(Db4objects.Db4o.Transaction trans)
		{
			return _handler.CompareTo(_handler.ComparableObject(trans, _handler.ReadIndexEntry
				(_reader)));
		}

		private Db4objects.Db4o.Foundation.Tree Insert(Db4objects.Db4o.Inside.IX.IxFileRange
			 fileRange, Db4objects.Db4o.Foundation.Tree a_new, int a_cursor, int a_cmp)
		{
			int incStartNewAt = a_cmp <= 0 ? 1 : 0;
			int newAddressOffset = (a_cursor + incStartNewAt) * _slotLength;
			int newEntries = fileRange._entries - a_cursor - incStartNewAt;
			fileRange._entries = a_cmp < 0 ? a_cursor + 1 : a_cursor;
			Db4objects.Db4o.Inside.IX.IxFileRange ifr = new Db4objects.Db4o.Inside.IX.IxFileRange
				(fileRange._fieldTransaction, _baseAddress, _baseAddressOffset + newAddressOffset
				, newEntries);
			ifr._subsequent = fileRange._subsequent;
			fileRange._subsequent = null;
			a_new._preceding = fileRange.BalanceCheckNulls();
			a_new._subsequent = ifr.BalanceCheckNulls();
			return a_new.Balance();
		}

		private void SetFileRange(Db4objects.Db4o.Inside.IX.IxFileRange a_fr)
		{
			_lower = 0;
			_upper = a_fr._entries - 1;
			_baseAddress = a_fr._address;
			_baseAddressOffset = a_fr._addressOffset;
			AdjustCursor();
		}
	}
}
