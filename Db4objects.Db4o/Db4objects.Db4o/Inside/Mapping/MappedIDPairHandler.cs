namespace Db4objects.Db4o.Inside.Mapping
{
	public class MappedIDPairHandler : Db4objects.Db4o.Inside.IX.IIndexable4
	{
		private readonly Db4objects.Db4o.YInt _origHandler;

		private readonly Db4objects.Db4o.YInt _mappedHandler;

		private readonly Db4objects.Db4o.YBoolean _seenHandler;

		public MappedIDPairHandler(Db4objects.Db4o.YapStream stream)
		{
			_origHandler = new Db4objects.Db4o.YInt(stream);
			_mappedHandler = new Db4objects.Db4o.YInt(stream);
			_seenHandler = new Db4objects.Db4o.YBoolean(stream);
		}

		public virtual object ComparableObject(Db4objects.Db4o.Transaction trans, object 
			indexEntry)
		{
			throw new System.NotImplementedException();
		}

		public virtual void DefragIndexEntry(Db4objects.Db4o.ReaderPair readers)
		{
			throw new System.NotImplementedException();
		}

		public virtual int LinkLength()
		{
			return _origHandler.LinkLength() + _mappedHandler.LinkLength() + _seenHandler.LinkLength
				();
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.YapReader reader)
		{
			int origID = ReadID(reader);
			int mappedID = ReadID(reader);
			bool seen = ReadSeen(reader);
			return new Db4objects.Db4o.Inside.Mapping.MappedIDPair(origID, mappedID, seen);
		}

		public virtual void WriteIndexEntry(Db4objects.Db4o.YapReader reader, object obj)
		{
			Db4objects.Db4o.Inside.Mapping.MappedIDPair mappedIDs = (Db4objects.Db4o.Inside.Mapping.MappedIDPair
				)obj;
			_origHandler.WriteIndexEntry(reader, mappedIDs.Orig());
			_mappedHandler.WriteIndexEntry(reader, mappedIDs.Mapped());
			_seenHandler.WriteIndexEntry(reader, (mappedIDs.Seen() ? true : false));
		}

		public virtual int CompareTo(object obj)
		{
			if (null == obj)
			{
				throw new System.ArgumentNullException();
			}
			Db4objects.Db4o.Inside.Mapping.MappedIDPair mappedIDs = (Db4objects.Db4o.Inside.Mapping.MappedIDPair
				)obj;
			int result = _origHandler.CompareTo(mappedIDs.Orig());
			return result;
		}

		public virtual object Current()
		{
			return new Db4objects.Db4o.Inside.Mapping.MappedIDPair(_origHandler.CurrentInt(), 
				_mappedHandler.CurrentInt(), ((bool)_seenHandler.Current()));
		}

		public virtual bool IsEqual(object obj)
		{
			throw new System.NotImplementedException();
		}

		public virtual bool IsGreater(object obj)
		{
			throw new System.NotImplementedException();
		}

		public virtual bool IsSmaller(object obj)
		{
			throw new System.NotImplementedException();
		}

		public virtual Db4objects.Db4o.IYapComparable PrepareComparison(object obj)
		{
			Db4objects.Db4o.Inside.Mapping.MappedIDPair mappedIDs = (Db4objects.Db4o.Inside.Mapping.MappedIDPair
				)obj;
			_origHandler.PrepareComparison(mappedIDs.Orig());
			_mappedHandler.PrepareComparison(mappedIDs.Mapped());
			_seenHandler.PrepareComparison((mappedIDs.Seen() ? true : false));
			return this;
		}

		private int ReadID(Db4objects.Db4o.YapReader a_reader)
		{
			return ((int)_origHandler.ReadIndexEntry(a_reader));
		}

		private bool ReadSeen(Db4objects.Db4o.YapReader a_reader)
		{
			return ((bool)_seenHandler.ReadIndexEntry(a_reader));
		}
	}
}
