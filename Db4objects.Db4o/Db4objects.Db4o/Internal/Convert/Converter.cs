using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Convert;
using Db4objects.Db4o.Internal.Convert.Conversions;

namespace Db4objects.Db4o.Internal.Convert
{
	/// <exclude></exclude>
	public class Converter
	{
		public const int VERSION = FieldIndexesToBTrees_5_7.VERSION;

		private static Db4objects.Db4o.Internal.Convert.Converter _converter;

		private Hashtable4 _conversions;

		private Converter()
		{
			_conversions = new Hashtable4();
			CommonConversions.Register(this);
		}

		public static bool Convert(ConversionStage stage)
		{
			if (!NeedsConversion(stage.SystemData()))
			{
				return false;
			}
			if (_converter == null)
			{
				_converter = new Db4objects.Db4o.Internal.Convert.Converter();
			}
			return _converter.RunConversions(stage);
		}

		private static bool NeedsConversion(SystemData systemData)
		{
			return systemData.ConverterVersion() < VERSION;
		}

		public virtual void Register(int idx, Conversion conversion)
		{
			if (_conversions.Get(idx) != null)
			{
				throw new InvalidOperationException();
			}
			_conversions.Put(idx, conversion);
		}

		public virtual bool RunConversions(ConversionStage stage)
		{
			SystemData systemData = stage.SystemData();
			if (!NeedsConversion(systemData))
			{
				return false;
			}
			for (int i = systemData.ConverterVersion(); i <= VERSION; i++)
			{
				Conversion conversion = (Conversion)_conversions.Get(i);
				if (conversion != null)
				{
					stage.Accept(conversion);
				}
			}
			return true;
		}
	}
}
