/* Copyright (C) 2009  Versant Inc.  http://www.db4o.com */
using System;

namespace Db4objects.Db4o.Internal
{
	public partial interface IInternalObjectContainer
	{
		void WithEnvironment(Action action);
	}
}
