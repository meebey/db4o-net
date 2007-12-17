/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IIndexable4 : IComparable4
	{
		int LinkLength();

		object ReadIndexEntry(BufferImpl reader);

		void WriteIndexEntry(BufferImpl writer, object obj);

		void DefragIndexEntry(DefragmentContextImpl context);
	}
}
