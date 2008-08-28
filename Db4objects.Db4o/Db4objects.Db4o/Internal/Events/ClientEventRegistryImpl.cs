/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.Events
{
	public partial class ClientEventRegistryImpl : Db4objects.Db4o.Internal.Events.EventRegistryImpl
	{
		public ClientEventRegistryImpl(IInternalObjectContainer container) : base(container
			)
		{
		}
	}
}
