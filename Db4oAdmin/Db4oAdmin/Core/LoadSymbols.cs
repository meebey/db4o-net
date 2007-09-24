/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
namespace Db4oAdmin.Core
{
	class LoadSymbols : IAssemblyInstrumentation
	{
		public void Run(InstrumentationContext context)
		{
			context.Assembly.MainModule.LoadSymbols();
		}
	}
}
