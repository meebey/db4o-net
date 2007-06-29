/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Nativequery
{
	public interface INativeClassFactory
	{
		Type ForName(string className);
	}
}
