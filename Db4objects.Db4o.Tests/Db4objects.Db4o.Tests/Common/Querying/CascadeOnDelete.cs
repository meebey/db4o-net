/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Querying;

namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class CascadeOnDelete : AbstractDb4oTestCase
	{
		public class Item
		{
			public string item;
		}

		public CascadeOnDelete.Item[] items;

		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			NoAccidentalDeletes();
		}

		/// <exception cref="Exception"></exception>
		private void NoAccidentalDeletes()
		{
			NoAccidentalDeletes1(true, true);
			NoAccidentalDeletes1(true, false);
			NoAccidentalDeletes1(false, true);
			NoAccidentalDeletes1(false, false);
		}

		/// <exception cref="Exception"></exception>
		private void NoAccidentalDeletes1(bool cascadeOnUpdate, bool cascadeOnDelete)
		{
			DeleteAll(GetType());
			DeleteAll(typeof(CascadeOnDelete.Item));
			IObjectClass oc = Db4oFactory.Configure().ObjectClass(typeof(CascadeOnDelete));
			oc.CascadeOnDelete(cascadeOnDelete);
			oc.CascadeOnUpdate(cascadeOnUpdate);
			Reopen();
			CascadeOnDelete.Item i = new CascadeOnDelete.Item();
			CascadeOnDelete cod = new CascadeOnDelete();
			cod.items = new CascadeOnDelete.Item[] { i };
			Db().Store(cod);
			Db().Commit();
			cod.items[0].item = "abrakadabra";
			Db().Store(cod);
			if (!cascadeOnDelete && !cascadeOnUpdate)
			{
				Db().Store(cod.items[0]);
			}
			Assert.AreEqual(1, CountOccurences(typeof(CascadeOnDelete.Item)));
			Db().Commit();
			Assert.AreEqual(1, CountOccurences(typeof(CascadeOnDelete.Item)));
		}
	}
}
