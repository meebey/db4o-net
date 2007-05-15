/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
namespace Db4oAdmin
{
	class SaveAssemblyInstrumentation : IAssemblyInstrumentation
	{
		public void Run(InstrumentationContext context)
		{
			context.SaveAssembly();
		}
	}
}
