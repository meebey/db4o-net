/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System.Collections;

namespace Db4objects.Db4o.Foundation
{
	public interface ISet4
	{
		bool Add(object obj);

		void Clear();

		bool Contains(object obj);

		bool IsEmpty();

		IEnumerator Iterator();

		bool Remove(object obj);

		int Size();
	}
}
