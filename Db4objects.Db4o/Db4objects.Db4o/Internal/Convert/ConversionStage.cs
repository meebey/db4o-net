namespace Db4objects.Db4o.Internal.Convert
{
	/// <exclude></exclude>
	public abstract class ConversionStage
	{
		public sealed class ClassCollectionAvailableStage : Db4objects.Db4o.Internal.Convert.ConversionStage
		{
			public ClassCollectionAvailableStage(Db4objects.Db4o.Internal.LocalObjectContainer
				 file) : base(file)
			{
			}

			public override void Accept(Db4objects.Db4o.Internal.Convert.Conversion conversion
				)
			{
				conversion.Convert(this);
			}
		}

		public sealed class SystemUpStage : Db4objects.Db4o.Internal.Convert.ConversionStage
		{
			public SystemUpStage(Db4objects.Db4o.Internal.LocalObjectContainer file) : base(file
				)
			{
			}

			public override void Accept(Db4objects.Db4o.Internal.Convert.Conversion conversion
				)
			{
				conversion.Convert(this);
			}
		}

		private Db4objects.Db4o.Internal.LocalObjectContainer _file;

		protected ConversionStage(Db4objects.Db4o.Internal.LocalObjectContainer file)
		{
			_file = file;
		}

		public virtual Db4objects.Db4o.Internal.LocalObjectContainer File()
		{
			return _file;
		}

		public virtual Db4objects.Db4o.Internal.SystemData SystemData()
		{
			return _file.SystemData();
		}

		public abstract void Accept(Db4objects.Db4o.Internal.Convert.Conversion conversion
			);
	}
}
