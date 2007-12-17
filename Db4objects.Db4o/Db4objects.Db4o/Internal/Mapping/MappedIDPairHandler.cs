/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
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

		public virtual void DefragIndexEntry(DefragmentContextImpl context)
		{
			throw new NotImplementedException();
		}

		public virtual int LinkLength()
		{
			return _origHandler.LinkLength() + _mappedHandler.LinkLength();
		}

		public virtual object ReadIndexEntry(BufferImpl reader)
		{
			int origID = ReadID(reader);
			int mappedID = ReadID(reader);
			return new MappedIDPair(origID, mappedID);
		}

		public virtual void WriteIndexEntry(BufferImpl reader, object obj)
		{
			MappedIDPair mappedIDs = (MappedIDPair)obj;
			_origHandler.WriteIndexEntry(reader, mappedIDs.Orig());
			_mappedHandler.WriteIndexEntry(reader, mappedIDs.Mapped());
		}

		public virtual int CompareTo(object obj)
		{
			return _origHandler.CompareTo(((MappedIDPair)obj).Orig());
		}

		public virtual IComparable4 PrepareComparison(object obj)
		{
			MappedIDPair mappedIDs = (MappedIDPair)obj;
			_origHandler.PrepareComparison(mappedIDs.Orig());
			_mappedHandler.PrepareComparison(mappedIDs.Mapped());
			return this;
		}

		private int ReadID(BufferImpl a_reader)
		{
			return ((int)_origHandler.ReadIndexEntry(a_reader));
		}
	}
}
