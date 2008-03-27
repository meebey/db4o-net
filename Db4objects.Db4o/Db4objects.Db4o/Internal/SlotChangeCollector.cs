/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal
{
	internal interface ISlotChangeCollector
	{
		void Added(int id);

		void Updated(int id);

		void Deleted(int id);
	}
}
