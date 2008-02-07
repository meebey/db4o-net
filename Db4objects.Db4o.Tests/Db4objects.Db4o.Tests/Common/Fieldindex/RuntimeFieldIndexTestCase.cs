/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Fieldindex;

namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class RuntimeFieldIndexTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		private static readonly string Fieldname = "_id";

		public class Data
		{
			public int _id;

			public Data(int id)
			{
				_id = id;
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			for (int i = 1; i <= 3; i++)
			{
				Store(new RuntimeFieldIndexTestCase.Data(i));
			}
		}

		public virtual void TestCreateIndexAtRuntime()
		{
			IStoredField field = Db().StoredClass(typeof(RuntimeFieldIndexTestCase.Data)).StoredField
				(Fieldname, null);
			Assert.IsFalse(field.HasIndex());
			field.CreateIndex();
			Assert.IsTrue(field.HasIndex());
			IQuery query = NewQuery(typeof(RuntimeFieldIndexTestCase.Data));
			query.Descend(Fieldname).Constrain(2);
			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Size());
			field.CreateIndex();
		}
		// ensure that second call is ignored
	}
}
