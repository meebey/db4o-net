/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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
			public static readonly PersistStaticFieldValuesTestCase.PsfvHelper One = new PersistStaticFieldValuesTestCase.PsfvHelper
				();

			public static readonly PersistStaticFieldValuesTestCase.PsfvHelper Two = new PersistStaticFieldValuesTestCase.PsfvHelper
				();

			public static readonly PersistStaticFieldValuesTestCase.PsfvHelper Three = new PersistStaticFieldValuesTestCase.PsfvHelper
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
			psfv.one = PersistStaticFieldValuesTestCase.Data.One;
			psfv.two = PersistStaticFieldValuesTestCase.Data.Two;
			psfv.three = PersistStaticFieldValuesTestCase.Data.Three;
			Store(psfv);
		}

		public virtual void Test()
		{
			PersistStaticFieldValuesTestCase.Data psfv = (PersistStaticFieldValuesTestCase.Data
				)RetrieveOnlyInstance(typeof(PersistStaticFieldValuesTestCase.Data));
			Assert.AreSame(PersistStaticFieldValuesTestCase.Data.One, psfv.one);
			Assert.AreSame(PersistStaticFieldValuesTestCase.Data.Two, psfv.two);
			Assert.AreSame(PersistStaticFieldValuesTestCase.Data.Three, psfv.three);
		}

		public class PsfvHelper
		{
		}
	}
}
