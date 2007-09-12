using System.IO;
using Db4oAdmin.Core;
using Mono.Cecil;

namespace Db4oAdmin.TA
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
