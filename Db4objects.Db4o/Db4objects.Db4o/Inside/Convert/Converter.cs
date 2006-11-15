namespace Db4objects.Db4o.Inside.Convert
{
	/// <exclude></exclude>
	public class Converter
	{
		public const int VERSION = Db4objects.Db4o.Inside.Convert.Conversions.FieldIndexesToBTrees_5_7
			.VERSION;

		private static Db4objects.Db4o.Inside.Convert.Converter _converter;

		private Db4objects.Db4o.Foundation.Hashtable4 _conversions;

		private Converter()
		{
			_conversions = new Db4objects.Db4o.Foundation.Hashtable4();
			Db4objects.Db4o.Inside.Convert.Conversions.CommonConversions.Register(this);
		}

		public static bool Convert(Db4objects.Db4o.Inside.Convert.ConversionStage stage)
		{
			if (!NeedsConversion(stage.SystemData()))
			{
				return false;
			}
			if (_converter == null)
			{
				_converter = new Db4objects.Db4o.Inside.Convert.Converter();
			}
			return _converter.RunConversions(stage);
		}

		private static bool NeedsConversion(Db4objects.Db4o.Inside.SystemData systemData)
		{
			return systemData.ConverterVersion() < VERSION;
		}

		public virtual void Register(int idx, Db4objects.Db4o.Inside.Convert.Conversion conversion
			)
		{
			if (_conversions.Get(idx) != null)
			{
				throw new System.InvalidOperationException();
			}
			_conversions.Put(idx, conversion);
		}

		public virtual bool RunConversions(Db4objects.Db4o.Inside.Convert.ConversionStage
			 stage)
		{
			Db4objects.Db4o.Inside.SystemData systemData = stage.SystemData();
			if (!NeedsConversion(systemData))
			{
				return false;
			}
			for (int i = systemData.ConverterVersion(); i <= VERSION; i++)
			{
				Db4objects.Db4o.Inside.Convert.Conversion conversion = (Db4objects.Db4o.Inside.Convert.Conversion
					)_conversions.Get(i);
				if (conversion != null)
				{
					stage.Accept(conversion);
				}
			}
			return true;
		}
	}
}
