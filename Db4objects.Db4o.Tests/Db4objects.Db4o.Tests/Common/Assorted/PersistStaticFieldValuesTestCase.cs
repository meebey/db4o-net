namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class PersistStaticFieldValuesTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Data
		{
			public static readonly Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.PsfvHelper
				 ONE = new Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.PsfvHelper
				();

			public static readonly Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.PsfvHelper
				 TWO = new Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.PsfvHelper
				();

			public static readonly Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.PsfvHelper
				 THREE = new Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.PsfvHelper
				();

			public Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.PsfvHelper
				 one;

			public Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.PsfvHelper
				 two;

			public Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.PsfvHelper
				 three;
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.Data)
				).PersistStaticFieldValues();
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.Data psfv = 
				new Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.Data(
				);
			psfv.one = Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.Data
				.ONE;
			psfv.two = Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.Data
				.TWO;
			psfv.three = Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.Data
				.THREE;
			Store(psfv);
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.Data psfv = 
				(Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.Data)RetrieveOnlyInstance
				(typeof(Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.Data)
				);
			Db4oUnit.Assert.AreSame(Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.Data
				.ONE, psfv.one);
			Db4oUnit.Assert.AreSame(Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.Data
				.TWO, psfv.two);
			Db4oUnit.Assert.AreSame(Db4objects.Db4o.Tests.Common.Assorted.PersistStaticFieldValuesTestCase.Data
				.THREE, psfv.three);
		}

		public class PsfvHelper
		{
		}
	}
}
