/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public interface IMarshallingInfo : IFieldListInfo
	{
		Db4objects.Db4o.Internal.ClassMetadata ClassMetadata();

		IReadBuffer Buffer();

		int CurrentSlot();

		void BeginSlot();
	}
}
