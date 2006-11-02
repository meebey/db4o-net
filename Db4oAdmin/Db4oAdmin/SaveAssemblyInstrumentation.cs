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
