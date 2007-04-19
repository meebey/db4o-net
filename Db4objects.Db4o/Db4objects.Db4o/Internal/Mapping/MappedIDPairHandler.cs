using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.IX;
using Db4objects.Db4o.Internal.Mapping;

namespace Db4objects.Db4o.Internal.Mapping
{
	/// <exclude></exclude>
	public class MappedIDPairHandler : IIndexable4
	{
		private readonly IntHandler _origHandler;

		private readonly IntHandler _mappedHandler;

		public MappedIDPairHandler(ObjectContainerBase stream)
		{
			_origHandler = new IntHandler(stream);
			_mappedHandler = new IntHandler(stream);
		}

		public virtual object ComparableObject(Transaction trans, object indexEntry)
		{
			throw new NotImplementedException();
		}

		public virtual void DefragIndexEntry(ReaderPair readers)
		{
			throw new NotImplementedException();
		}

		public virtual int LinkLength()
		{
			return _origHandler.LinkLength() + _mappedHandler.LinkLength();
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer reader)
		{
			int origID = ReadID(reader);
			int mappedID = ReadID(reader);
			return new MappedIDPair(origID, mappedID);
		}

		public virtual void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer reader, object
			 obj)
		{
			MappedIDPair mappedIDs = (MappedIDPair)obj;
			_origHandler.WriteIndexEntry(reader, mappedIDs.Orig());
			_mappedHandler.WriteIndexEntry(reader, mappedIDs.Mapped());
		}

		public virtual int CompareTo(object obj)
		{
			return _origHandler.CompareTo(((MappedIDPair)obj).Orig());
		}

		public virtual object Current()
		{
			return new MappedIDPair(_origHandler.CurrentInt(), _mappedHandler.CurrentInt());
		}

		public virtual bool IsEqual(object obj)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsGreater(object obj)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsSmaller(object obj)
		{
			throw new NotImplementedException();
		}

		public virtual IComparable4 PrepareComparison(object obj)
		{
			MappedIDPair mappedIDs = (MappedIDPair)obj;
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
