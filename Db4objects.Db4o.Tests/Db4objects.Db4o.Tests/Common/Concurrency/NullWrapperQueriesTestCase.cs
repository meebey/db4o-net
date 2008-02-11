/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Concurrency;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class NullWrapperQueriesTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new NullWrapperQueriesTestCase().RunConcurrency();
		}

		public bool m1;

		public bool m2;

		public char m3;

		public DateTime m4;

		public double m5;

		public float m6;

		public int m7;

		public long m8;

		public short m9;

		public string m10;

		protected override void Configure(IConfiguration config)
		{
			for (int i = 1; i < 11; i++)
			{
				string desc = "m" + i;
				config.ObjectClass(this).ObjectField(desc).Indexed(true);
			}
		}

		protected override void Store()
		{
			NullWrapperQueriesTestCase nwq = new NullWrapperQueriesTestCase();
			nwq.Fill1();
			Store(nwq);
			nwq = new NullWrapperQueriesTestCase();
			nwq.Fill0();
			Store(nwq);
			nwq = new NullWrapperQueriesTestCase();
			nwq.Fill0();
			Store(nwq);
			nwq = new NullWrapperQueriesTestCase();
			nwq.Fill1();
			Store(nwq);
			nwq = new NullWrapperQueriesTestCase();
			Store(nwq);
			nwq = new NullWrapperQueriesTestCase();
			Store(nwq);
		}

		public virtual void Conc(IExtObjectContainer oc)
		{
			for (int i = 1; i < 11; i++)
			{
				IQuery q = oc.Query();
				q.Constrain(typeof(NullWrapperQueriesTestCase));
				string desc = "m" + i;
				q.Descend(desc).Constrain(null);
				Assert.AreEqual(2, q.Execute().Size());
			}
		}

		private void Fill0()
		{
			m1 = false;
			m2 = false;
			m3 = (char)0;
			m4 = new DateTime(0);
			m5 = System.Convert.ToDouble(0);
			m6 = System.Convert.ToSingle(0);
			m7 = 0;
			m8 = System.Convert.ToInt64(0);
			m9 = (short)0;
			m10 = string.Empty;
		}

		private void Fill1()
		{
			m1 = true;
			m2 = true;
			m3 = (char)1;
			m4 = new DateTime(1);
			m5 = System.Convert.ToDouble(1);
			m6 = System.Convert.ToSingle(1);
			m7 = 1;
			m8 = System.Convert.ToInt64(1);
			m9 = (short)1;
			m10 = "1";
		}
	}
}
