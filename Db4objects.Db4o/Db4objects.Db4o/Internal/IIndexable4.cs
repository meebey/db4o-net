/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IIndexable4 : IComparable4
	{
		int LinkLength();

		object ReadIndexEntry(IContext context, ByteArrayBuffer reader);

		void WriteIndexEntry(IContext context, ByteArrayBuffer writer, object obj);

		void DefragIndexEntry(DefragmentContextImpl context);
	}
}
