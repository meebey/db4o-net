namespace Db4objects.Db4o.Foundation
{
	public partial class Environments
	{
		public static string DefaultImplementationFor(System.Type type)
		{
			string ns = type.Namespace;
			int lastDot = ns.LastIndexOf('.');
			string typeName = ns.Substring(0, lastDot) + ".Internal." + ns.Substring(lastDot + 1) + "." + type.Name.Substring(1) + "Impl";
			return typeName + ", " + type.Assembly.GetName().Name;
		}
	}
}
