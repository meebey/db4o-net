/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Filestats
{
	/// <exclude></exclude>
	public interface ISlotMap
	{
		void Add(Slot slot);

		IList Merged();

		IList Gaps(long length);
	}
}
