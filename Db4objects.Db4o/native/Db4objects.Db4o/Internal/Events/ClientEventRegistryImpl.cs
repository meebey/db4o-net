/* Copyright (C) 2008   db4objects Inc.   http://www.db4o.com */

using System;
using Db4objects.Db4o.Events;

namespace Db4objects.Db4o.Internal.Events
{
	partial class ClientEventRegistryImpl
	{
		public override event ObjectEventHandler Deleted
		{
			add
			{
				throw new ArgumentException("delete() event is raised only at server side.");
			}

			remove
			{
				throw new ArgumentException("delete() event is raised only at server side.");
			}
		}

		public override event CancellableObjectEventHandler Deleting
		{
			add
			{
				throw new ArgumentException("deleting() event is raised only at server side.");
			}

			remove
			{
				throw new ArgumentException("deleting() event is raised only at server side.");
			}
		}
	}
}
