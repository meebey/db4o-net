/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using Db4objects.Db4o.Events;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Constraints
{
	class ConstraintPlatform
	{
		class CommitEventAdapter
		{
			private ObjectContainerBase _container;
			private ICommitHandler _handler;

			public CommitEventAdapter(ObjectContainerBase container, ICommitHandler handler)
			{
				_container = container;
				_handler = handler;
			}

			public void Handle(object sender, CommitEventArgs cea)
			{
				_handler.Handle(_container, cea);
			}
		}

		public static void AddCommittingHandler(ObjectContainerBase container, ICommitHandler handler)
		{
			EventRegistryFactory.ForObjectContainer(container).Committing +=
				new CommitEventHandler(new CommitEventAdapter(container, handler).Handle);
		}
	}
}
