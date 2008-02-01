/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public interface IInternalReadContext : IReadContext
	{
		IReadWriteBuffer Buffer(IReadWriteBuffer buffer);

		IReadWriteBuffer Buffer();

		ObjectContainerBase Container();

		int Offset();

		object Read(ITypeHandler4 handler);

		IReadWriteBuffer ReadIndirectedBuffer();

		void Seek(int offset);
	}
}
