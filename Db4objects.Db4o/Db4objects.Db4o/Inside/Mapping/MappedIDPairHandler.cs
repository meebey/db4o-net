namespace Db4objects.Db4o.Inside.Mapping
{
	/// <exclude></exclude>
	public class MappedIDPairHandler : Db4objects.Db4o.Inside.IX.IIndexable4
	{
		private readonly Db4objects.Db4o.YInt _origHandler;

		private readonly Db4objects.Db4o.YInt _mappedHandler;

		public MappedIDPairHandler(Db4objects.Db4o.YapStream stream)
		{
			_origHandler = new Db4objects.Db4o.YInt(stream);
			_mappedHandler = new Db4objects.Db4o.YInt(stream);
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
			return _origHandler.LinkLength() + _mappedHandler.LinkLength();
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.YapReader reader)
		{
			int origID = ReadID(reader);
			int mappedID = ReadID(reader);
			return new Db4objects.Db4o.Inside.Mapping.MappedIDPair(origID, mappedID);
		}

		public virtual void WriteIndexEntry(Db4objects.Db4o.YapReader reader, object obj)
		{
			Db4objects.Db4o.Inside.Mapping.MappedIDPair mappedIDs = (Db4objects.Db4o.Inside.Mapping.MappedIDPair
				)obj;
			_origHandler.WriteIndexEntry(reader, mappedIDs.Orig());
			_mappedHandler.WriteIndexEntry(reader, mappedIDs.Mapped());
		}

		public virtual int CompareTo(object obj)
		{
			return _origHandler.CompareTo(((Db4objects.Db4o.Inside.Mapping.MappedIDPair)obj).
				Orig());
		}

		public virtual object Current()
		{
			return new Db4objects.Db4o.Inside.Mapping.MappedIDPair(_origHandler.CurrentInt(), 
				_mappedHandler.CurrentInt());
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
			return this;
		}

		private int ReadID(Db4objects.Db4o.YapReader a_reader)
		{
			return ((int)_origHandler.ReadIndexEntry(a_reader));
		}
	}
}
