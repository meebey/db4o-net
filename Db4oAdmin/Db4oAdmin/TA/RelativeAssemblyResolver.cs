using System.IO;
using Db4oTool.Core;
using Mono.Cecil;

namespace Db4oTool.TA
{
	public class RelativeAssemblyResolver : DefaultAssemblyResolver
	{
		public RelativeAssemblyResolver(InstrumentationContext context)
		{
			RegisterAssembly(context.Assembly);
			AddSearchDirectory(Path.GetDirectoryName(context.AssemblyLocation));
		}
	}
}
