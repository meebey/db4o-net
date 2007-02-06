namespace Db4objects.Db4o.Internal.Mapping
{
	/// <exclude></exclude>
	public class MappedIDPairHandler : Db4objects.Db4o.Internal.IX.IIndexable4
	{
		private readonly Db4objects.Db4o.Internal.Handlers.IntHandler _origHandler;

		private readonly Db4objects.Db4o.Internal.Handlers.IntHandler _mappedHandler;

		public MappedIDPairHandler(Db4objects.Db4o.Internal.ObjectContainerBase stream)
		{
			_origHandler = new Db4objects.Db4o.Internal.Handlers.IntHandler(stream);
			_mappedHandler = new Db4objects.Db4o.Internal.Handlers.IntHandler(stream);
		}

		public virtual object ComparableObject(Db4objects.Db4o.Internal.Transaction trans
			, object indexEntry)
		{
			throw new System.NotImplementedException();
		}

		public virtual void DefragIndexEntry(Db4objects.Db4o.Internal.ReaderPair readers)
		{
			throw new System.NotImplementedException();
		}

		public virtual int LinkLength()
		{
			return _origHandler.LinkLength() + _mappedHandler.LinkLength();
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer reader)
		{
			int origID = ReadID(reader);
			int mappedID = ReadID(reader);
			return new Db4objects.Db4o.Internal.Mapping.MappedIDPair(origID, mappedID);
		}

		public virtual void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer reader, object
			 obj)
		{
			Db4objects.Db4o.Internal.Mapping.MappedIDPair mappedIDs = (Db4objects.Db4o.Internal.Mapping.MappedIDPair
				)obj;
			_origHandler.WriteIndexEntry(reader, mappedIDs.Orig());
			_mappedHandler.WriteIndexEntry(reader, mappedIDs.Mapped());
		}

		public virtual int CompareTo(object obj)
		{
			return _origHandler.CompareTo(((Db4objects.Db4o.Internal.Mapping.MappedIDPair)obj
				).Orig());
		}

		public virtual object Current()
		{
			return new Db4objects.Db4o.Internal.Mapping.MappedIDPair(_origHandler.CurrentInt(
				), _mappedHandler.CurrentInt());
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

		public virtual Db4objects.Db4o.Internal.IComparable4 PrepareComparison(object obj
			)
		{
			Db4objects.Db4o.Internal.Mapping.MappedIDPair mappedIDs = (Db4objects.Db4o.Internal.Mapping.MappedIDPair
				)obj;
			_origHandler.PrepareComparison(mappedIDs.Orig());
			_mappedHandler.PrepareComparison(mappedIDs.Mapped());
			return this;
		}

		private int ReadID(Db4objects.Db4o.Internal.Buffer a_reader)
		{
			return ((int)_origHandler.ReadIndexEntry(a_reader));
		}
	}
}
