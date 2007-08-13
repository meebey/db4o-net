/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Concurrency;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class PersistStaticFieldValuesTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new PersistStaticFieldValuesTestCase().RunConcurrency();
		}

		public static readonly PersistStaticFieldValuesTestCase.PsfvHelper ONE = new PersistStaticFieldValuesTestCase.PsfvHelper
			();

		public static readonly PersistStaticFieldValuesTestCase.PsfvHelper TWO = new PersistStaticFieldValuesTestCase.PsfvHelper
			();

		public static readonly PersistStaticFieldValuesTestCase.PsfvHelper THREE = new PersistStaticFieldValuesTestCase.PsfvHelper
			();

		public PersistStaticFieldValuesTestCase.PsfvHelper one;

		public PersistStaticFieldValuesTestCase.PsfvHelper two;

		public PersistStaticFieldValuesTestCase.PsfvHelper three;

		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(PersistStaticFieldValuesTestCase)).PersistStaticFieldValues
				();
		}

		protected override void Store()
		{
			PersistStaticFieldValuesTestCase psfv = new PersistStaticFieldValuesTestCase();
			psfv.one = ONE;
			psfv.two = TWO;
			psfv.three = THREE;
			Store(psfv);
		}

		public virtual void Conc(IExtObjectContainer oc)
		{
			PersistStaticFieldValuesTestCase psfv = (PersistStaticFieldValuesTestCase)RetrieveOnlyInstance
				(oc, typeof(PersistStaticFieldValuesTestCase));
			Assert.AreSame(ONE, psfv.one);
			Assert.AreSame(TWO, psfv.two);
			Assert.AreSame(THREE, psfv.three);
		}

		public class PsfvHelper
		{
		}
	}
}
