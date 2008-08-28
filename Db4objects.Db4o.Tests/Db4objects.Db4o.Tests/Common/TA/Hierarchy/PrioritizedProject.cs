/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Tests.Common.TA.Hierarchy;

namespace Db4objects.Db4o.Tests.Common.TA.Hierarchy
{
	/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
	internal class PrioritizedProject : Project
	{
		private int _priority;

		public PrioritizedProject(string name, int priority) : base(name)
		{
			_priority = priority;
		}

		public virtual int GetPriority()
		{
			// TA BEGIN
			Activate(ActivationPurpose.Read);
			// TA END
			return _priority;
		}
	}
}
