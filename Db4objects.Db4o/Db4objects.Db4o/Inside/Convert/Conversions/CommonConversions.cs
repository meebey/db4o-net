namespace Db4objects.Db4o.Inside.Convert.Conversions
{
	/// <exclude></exclude>
	public class CommonConversions
	{
		public static void Register(Db4objects.Db4o.Inside.Convert.Converter converter)
		{
			converter.Register(Db4objects.Db4o.Inside.Convert.Conversions.ClassIndexesToBTrees_5_5
				.VERSION, new Db4objects.Db4o.Inside.Convert.Conversions.ClassIndexesToBTrees_5_5
				());
			converter.Register(Db4objects.Db4o.Inside.Convert.Conversions.FieldIndexesToBTrees_5_7
				.VERSION, new Db4objects.Db4o.Inside.Convert.Conversions.FieldIndexesToBTrees_5_7
				());
		}
	}
}
