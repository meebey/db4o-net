/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface ITypeHandler4 : IComparable4
	{
		void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer buffer);

		void Defrag(MarshallerFamily mf, BufferPair readers, bool redirect);

		object Read(IReadContext context);

		void Write(IWriteContext context, object obj);
	}
}
