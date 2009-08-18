using System;
using Sharpen.Lang;

namespace Db4objects.Db4o.Foundation
{
	public class RunnableAction : IRunnable
	{
		private Action _action;

		public RunnableAction(Action action)
		{
			_action = action;
		}

		public void Run()
		{
			_action();
		}
	}
}
