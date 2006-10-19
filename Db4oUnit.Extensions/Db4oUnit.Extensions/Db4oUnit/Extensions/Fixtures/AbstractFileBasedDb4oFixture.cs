namespace Db4oUnit.Extensions.Fixtures
{
	public abstract class AbstractFileBasedDb4oFixture : Db4oUnit.Extensions.Fixtures.AbstractSoloDb4oFixture
	{
		private readonly Sharpen.IO.File _yap;

		public AbstractFileBasedDb4oFixture(Db4oUnit.Extensions.Fixtures.IConfigurationSource
			 configSource, string fileName) : base(configSource)
		{
			_yap = new Sharpen.IO.File(fileName);
		}

		public virtual string GetAbsolutePath()
		{
			return _yap.GetAbsolutePath();
		}

		protected override void DoClean()
		{
			_yap.Delete();
		}
	}
}
