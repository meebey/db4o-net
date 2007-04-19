using Db4objects.Db4o.Internal.Convert;
using Db4objects.Db4o.Internal.Convert.Conversions;

namespace Db4objects.Db4o.Internal.Convert.Conversions
{
	/// <exclude></exclude>
	public class CommonConversions
	{
		public static void Register(Converter converter)
		{
			converter.Register(ClassIndexesToBTrees_5_5.VERSION, new ClassIndexesToBTrees_5_5
				());
			converter.Register(FieldIndexesToBTrees_5_7.VERSION, new FieldIndexesToBTrees_5_7
				());
		}
	}
}
