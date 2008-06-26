/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Jre5.Collections.Typehandler;

namespace Db4objects.Db4o.Tests.Jre5.Collections.Typehandler
{
	public abstract class TypeHandlerUnitTest : TypeHandlerTestUnitBase
	{
		protected abstract void AssertCompareItems(object element, bool successful);

		protected virtual void AssertQuery(bool successful, object element, bool withContains
			)
		{
			IQuery q = NewQuery(ItemFactory().ItemClass());
			IConstraint constraint = q.Descend(ItemFactory().FieldName()).Constrain(element);
			if (withContains)
			{
				constraint.Contains();
			}
			AssertQueryResult(q, successful);
		}

		public virtual void TestRetrieveInstance()
		{
			object item = RetrieveItemInstance();
			AssertContent(item);
		}

		protected virtual object RetrieveItemInstance()
		{
			Type itemClass = ItemFactory().ItemClass();
			object item = RetrieveOnlyInstance(itemClass);
			return item;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestSuccessfulQuery()
		{
			AssertQuery(true, Elements()[0], false);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestFailingQuery()
		{
			AssertQuery(false, NotContained(), false);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestSuccessfulContainsQuery()
		{
			AssertQuery(true, Elements()[0], true);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestFailingContainsQuery()
		{
			AssertQuery(false, NotContained(), true);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestCompareItems()
		{
			AssertCompareItems(Elements()[0], true);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestFailingCompareItems()
		{
			AssertCompareItems(NotContained(), false);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestDeletion()
		{
			AssertFirstClassElementCount(Elements().Length);
			object item = RetrieveOnlyInstance(ItemFactory().ItemClass());
			Db().Delete(item);
			Db().Purge();
			Db4oAssert.PersistedCount(0, ItemFactory().ItemClass());
			AssertFirstClassElementCount(0);
		}

		protected virtual void AssertFirstClassElementCount(int expected)
		{
			if (!IsFirstClass(ElementClass()))
			{
				return;
			}
			Db4oAssert.PersistedCount(expected, ElementClass());
		}

		private bool IsFirstClass(Type elementClass)
		{
			return typeof(ListTypeHandlerTestVariables.FirstClassElement) == elementClass;
		}
	}
}
