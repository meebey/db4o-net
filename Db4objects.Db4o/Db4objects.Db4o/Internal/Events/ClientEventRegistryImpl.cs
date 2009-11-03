/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Events;

namespace Db4objects.Db4o.Internal.Events
{
	public partial class ClientEventRegistryImpl : EventRegistryImpl
	{
		public ClientEventRegistryImpl(IInternalObjectContainer container) : base(container
			)
		{
		}
	}
}
