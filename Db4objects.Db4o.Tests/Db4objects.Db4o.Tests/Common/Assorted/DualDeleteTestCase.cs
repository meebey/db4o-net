/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Assorted;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class DualDeleteTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new DualDeleteTestCase().RunClientServer();
		}

		public Atom atom;

		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(this).CascadeOnDelete(true);
			config.ObjectClass(this).CascadeOnUpdate(true);
		}

		protected override void Store()
		{
			DualDeleteTestCase dd1 = new DualDeleteTestCase();
			dd1.atom = new Atom("justone");
			Store(dd1);
			DualDeleteTestCase dd2 = new DualDeleteTestCase();
			dd2.atom = dd1.atom;
			Store(dd2);
		}

		public virtual void Test()
		{
			IExtObjectContainer oc1 = OpenNewClient();
			IExtObjectContainer oc2 = OpenNewClient();
			try
			{
				IObjectSet os1 = oc1.Query(typeof(DualDeleteTestCase));
				IObjectSet os2 = oc2.Query(typeof(DualDeleteTestCase));
				DeleteObjectSet(oc1, os1);
				AssertOccurrences(oc1, typeof(Atom), 0);
				AssertOccurrences(oc2, typeof(Atom), 1);
				DeleteObjectSet(oc2, os2);
				AssertOccurrences(oc1, typeof(Atom), 0);
				AssertOccurrences(oc2, typeof(Atom), 0);
				oc1.Rollback();
				AssertOccurrences(oc1, typeof(Atom), 1);
				AssertOccurrences(oc2, typeof(Atom), 0);
				oc1.Commit();
				AssertOccurrences(oc1, typeof(Atom), 1);
				AssertOccurrences(oc2, typeof(Atom), 0);
				DeleteAll(oc2, typeof(DualDeleteTestCase));
				oc2.Commit();
				AssertOccurrences(oc1, typeof(Atom), 0);
				AssertOccurrences(oc2, typeof(Atom), 0);
			}
			finally
			{
				oc1.Close();
				oc2.Close();
			}
		}

		/// <exception cref="Exception"></exception>
		public virtual void Conc1(IExtObjectContainer oc)
		{
			IObjectSet os = oc.Query(typeof(DualDeleteTestCase));
			Thread.Sleep(500);
			DeleteObjectSet(oc, os);
			oc.Rollback();
		}

		/// <exception cref="Exception"></exception>
		public virtual void Check1(IExtObjectContainer oc)
		{
			AssertOccurrences(oc, typeof(Atom), 1);
		}

		/// <exception cref="Exception"></exception>
		public virtual void Conc2(IExtObjectContainer oc)
		{
			IObjectSet os = oc.Query(typeof(DualDeleteTestCase));
			Thread.Sleep(500);
			DeleteObjectSet(oc, os);
			AssertOccurrences(oc, typeof(Atom), 0);
		}

		/// <exception cref="Exception"></exception>
		public virtual void Check2(IExtObjectContainer oc)
		{
			AssertOccurrences(oc, typeof(Atom), 0);
		}
	}
}
