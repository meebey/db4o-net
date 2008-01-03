/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Handlers;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public abstract class HandlerUpdateTestCaseBase : FormatMigrationTestCaseBase
	{
		public class Holder
		{
			public object[] _values;

			public object _arrays;
		}

		protected int _handlerVersion;

		protected override string FileNamePrefix()
		{
			return "migrate_" + TypeName() + "_";
		}

		protected override string[] VersionNames()
		{
			return new string[] { Sharpen.Runtime.Substring(Db4oFactory.Version(), 5) };
		}

		protected override void Store(IExtObjectContainer objectContainer)
		{
			HandlerUpdateTestCaseBase.Holder holder = new HandlerUpdateTestCaseBase.Holder();
			holder._values = CreateValues();
			holder._arrays = CreateArrays();
			StoreObject(objectContainer, holder);
		}

		protected override void AssertObjectsAreReadable(IExtObjectContainer objectContainer
			)
		{
			IQuery q = objectContainer.Query();
			q.Constrain(typeof(HandlerUpdateTestCaseBase.Holder));
			IObjectSet objectSet = q.Execute();
			HandlerUpdateTestCaseBase.Holder holder = (HandlerUpdateTestCaseBase.Holder)objectSet
				.Next();
			InvestigateHandlerVersion(objectContainer, holder);
			AssertValues(holder._values);
			AssertArrays(holder._arrays);
		}

		private void InvestigateHandlerVersion(IExtObjectContainer objectContainer, object
			 obj)
		{
			_handlerVersion = VersionServices.SlotHandlerVersion(objectContainer, obj);
		}

		protected abstract string TypeName();

		protected abstract object[] CreateValues();

		protected abstract object CreateArrays();

		protected abstract void AssertValues(object[] values);

		protected abstract void AssertArrays(object obj);

		protected virtual int[] CastToIntArray(object obj)
		{
			ObjectByRef byRef = new ObjectByRef(obj);
			return (int[])byRef.value;
		}
	}
}
