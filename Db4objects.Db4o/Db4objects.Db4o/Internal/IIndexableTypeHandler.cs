/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IIndexableTypeHandler : IIndexable4, ITypeHandler4
	{
		object IndexEntryToObject(Transaction trans, object indexEntry);

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="Db4oIOException"></exception>
		object ReadIndexEntry(MarshallerFamily mf, StatefulBuffer writer);
	}
}
