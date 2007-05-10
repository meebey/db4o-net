using Db4objects.Db4o.TA.Tests;

namespace Db4objects.Db4o.TA.Tests
{
	internal class PrioritizedProject : Project
	{
		private int _priority;

		public PrioritizedProject(string name, int priority) : base(name)
		{
			_priority = priority;
		}

		public virtual int GetPriority()
		{
			Activate();
			return _priority;
		}
	}
}
