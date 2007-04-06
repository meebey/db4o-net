using Db4objects.Db4o.Internal.Convert;

namespace Db4objects.Db4o.Internal.Convert
{
	/// <exclude></exclude>
	public abstract class Conversion
	{
		public virtual void Convert(ConversionStage.ClassCollectionAvailableStage stage)
		{
		}

		public virtual void Convert(ConversionStage.SystemUpStage stage)
		{
		}
	}
}
