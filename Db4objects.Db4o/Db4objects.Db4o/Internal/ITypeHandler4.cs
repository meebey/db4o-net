/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Fieldhandlers;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude>
	/// TODO: Not all TypeHandlers can implement Comparable4.
	/// Consider to change the hierarchy, not to extend Comparable4
	/// and to have callers check, if Comparable4 is implemented by
	/// a TypeHandler.
	/// </exclude>
	public interface ITypeHandler4 : IFieldHandler, IComparable4
	{
		/// <exception cref="Db4oIOException"></exception>
		void Delete(IDeleteContext context);

		void Defragment(IDefragmentContext context);

		object Read(IReadContext context);

		void Write(IWriteContext context, object obj);
	}
}
