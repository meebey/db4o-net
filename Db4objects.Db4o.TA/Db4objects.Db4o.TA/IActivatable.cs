/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Activation;

namespace Db4objects.Db4o.TA
{
	public interface IActivatable
	{
		void Bind(IActivator activator);
	}
}
