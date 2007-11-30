using System.IO;
using Mono.Cecil;

namespace Db4oTool.Core
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
