/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public abstract class HandlerUpdateTestCaseBase : FormatMigrationTestCaseBase
	{
		protected override string[] VersionNames()
		{
			return new string[] { "db4o_6.4.000" };
		}

		public class Holder
		{
			public object[] _values;

			public object _arrays;
		}

		protected override string FileNamePrefix()
		{
			return "migrate_" + TypeName() + "_";
		}

		protected override void Configure(IConfiguration config)
		{
		}

		protected override void Store(IExtObjectContainer objectContainer)
		{
			HandlerUpdateTestCaseBase.Holder holder = new HandlerUpdateTestCaseBase.Holder();
			holder._values = CreateValues();
			holder._arrays = CreateArrays();
			objectContainer.Set(holder);
		}

		protected override void AssertObjectsAreReadable(IExtObjectContainer objectContainer
			)
		{
			IQuery q = objectContainer.Query();
			q.Constrain(typeof(HandlerUpdateTestCaseBase.Holder));
			IObjectSet objectSet = q.Execute();
			HandlerUpdateTestCaseBase.Holder holder = (HandlerUpdateTestCaseBase.Holder)objectSet
				.Next();
			AssertValues(holder._values);
			AssertArrays(holder._arrays);
		}

		protected abstract string TypeName();

		protected abstract object[] CreateValues();

		protected abstract object CreateArrays();

		protected abstract void AssertValues(object[] values);

		protected abstract void AssertArrays(object obj);
	}
}
