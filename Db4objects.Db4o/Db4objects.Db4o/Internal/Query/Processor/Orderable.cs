/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Internal.Query.Processor
{
	internal interface IOrderable
	{
		int CompareTo(object obj);

		void HintOrder(int a_order, bool a_major);

		bool HasDuplicates();
	}
}
