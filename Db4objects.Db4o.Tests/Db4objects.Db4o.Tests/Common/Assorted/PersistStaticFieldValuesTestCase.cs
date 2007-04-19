using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class PersistStaticFieldValuesTestCase : AbstractDb4oTestCase
	{
		public class Data
		{
			public static readonly PersistStaticFieldValuesTestCase.PsfvHelper ONE = new PersistStaticFieldValuesTestCase.PsfvHelper
				();

			public static readonly PersistStaticFieldValuesTestCase.PsfvHelper TWO = new PersistStaticFieldValuesTestCase.PsfvHelper
				();

			public static readonly PersistStaticFieldValuesTestCase.PsfvHelper THREE = new PersistStaticFieldValuesTestCase.PsfvHelper
				();

			public PersistStaticFieldValuesTestCase.PsfvHelper one;

			public PersistStaticFieldValuesTestCase.PsfvHelper two;

			public PersistStaticFieldValuesTestCase.PsfvHelper three;
		}

		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(PersistStaticFieldValuesTestCase.Data)).PersistStaticFieldValues
				();
		}

		protected override void Store()
		{
			PersistStaticFieldValuesTestCase.Data psfv = new PersistStaticFieldValuesTestCase.Data
				();
			psfv.one = PersistStaticFieldValuesTestCase.Data.ONE;
			psfv.two = PersistStaticFieldValuesTestCase.Data.TWO;
			psfv.three = PersistStaticFieldValuesTestCase.Data.THREE;
			Store(psfv);
		}

		public virtual void Test()
		{
			PersistStaticFieldValuesTestCase.Data psfv = (PersistStaticFieldValuesTestCase.Data
				)RetrieveOnlyInstance(typeof(PersistStaticFieldValuesTestCase.Data));
			Assert.AreSame(PersistStaticFieldValuesTestCase.Data.ONE, psfv.one);
			Assert.AreSame(PersistStaticFieldValuesTestCase.Data.TWO, psfv.two);
			Assert.AreSame(PersistStaticFieldValuesTestCase.Data.THREE, psfv.three);
		}

		public class PsfvHelper
		{
		}
	}
}
