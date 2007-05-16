/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;

namespace Db4objects.Db4o.Foundation
{
	public interface IQueue4
	{
		void Add(object obj);

		object Next();

		bool HasNext();

		IEnumerator Iterator();
	}
}
