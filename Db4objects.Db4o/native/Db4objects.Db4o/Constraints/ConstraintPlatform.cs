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
			private IConstraint _constraint;

			public CommitEventAdapter(ObjectContainerBase container, IConstraint constraint)
			{
				_container = container;
				_constraint = constraint;
			}

			public void Check(object sender, CommitEventArgs cea)
			{
				_constraint.Check(_container, cea);
			}
		}

		public static void AddCommittingConstraint(ObjectContainerBase container, IConstraint constraint)
		{
			EventRegistryFactory.ForObjectContainer(container).Committing +=
				new CommitEventHandler(new CommitEventAdapter(container, constraint).Check);
		}
	}
}
