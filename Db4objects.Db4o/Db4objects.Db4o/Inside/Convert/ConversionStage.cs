namespace Db4objects.Db4o.Inside.Convert
{
	/// <exclude></exclude>
	public abstract class ConversionStage
	{
		public sealed class ClassCollectionAvailableStage : Db4objects.Db4o.Inside.Convert.ConversionStage
		{
			public ClassCollectionAvailableStage(Db4objects.Db4o.YapFile file) : base(file)
			{
			}

			public override void Accept(Db4objects.Db4o.Inside.Convert.Conversion conversion)
			{
				conversion.Convert(this);
			}
		}

		public sealed class SystemUpStage : Db4objects.Db4o.Inside.Convert.ConversionStage
		{
			public SystemUpStage(Db4objects.Db4o.YapFile file) : base(file)
			{
			}

			public override void Accept(Db4objects.Db4o.Inside.Convert.Conversion conversion)
			{
				conversion.Convert(this);
			}
		}

		private Db4objects.Db4o.YapFile _file;

		protected ConversionStage(Db4objects.Db4o.YapFile file)
		{
			_file = file;
		}

		public virtual Db4objects.Db4o.YapFile File()
		{
			return _file;
		}

		public virtual Db4objects.Db4o.Inside.SystemData SystemData()
		{
			return _file.SystemData();
		}

		public abstract void Accept(Db4objects.Db4o.Inside.Convert.Conversion conversion);
	}
}
