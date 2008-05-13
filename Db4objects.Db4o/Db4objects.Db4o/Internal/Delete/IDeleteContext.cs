/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Delete
{
	/// <exclude></exclude>
	public interface IDeleteContext : IContext, IReadBuffer
	{
		bool CascadeDelete();

		int CascadeDeleteDepth();

		void Delete(ITypeHandler4 handler);

		void DeleteObject();

		bool IsLegacyHandlerVersion();

		int HandlerVersion();

		void DefragmentRecommended();

		Slot ReadSlot();
	}
}
