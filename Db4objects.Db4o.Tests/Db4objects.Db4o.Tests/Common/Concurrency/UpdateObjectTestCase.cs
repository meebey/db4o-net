/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Concurrency;
using Db4objects.Db4o.Tests.Common.Persistent;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class UpdateObjectTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new UpdateObjectTestCase().RunConcurrency();
		}

		private static string testString = "simple test string";

		private static int COUNT = 100;

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			for (int i = 0; i < COUNT; i++)
			{
				Store(new SimpleObject(testString + i, i));
			}
		}

		/// <exception cref="Exception"></exception>
		public virtual void ConcUpdateSameObject(IExtObjectContainer oc, int seq)
		{
			IQuery query = oc.Query();
			query.Descend("_s").Constrain(testString + COUNT / 2);
			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Size());
			SimpleObject o = (SimpleObject)result.Next();
			o.SetI(COUNT + seq);
			oc.Set(o);
		}

		/// <exception cref="Exception"></exception>
		public virtual void CheckUpdateSameObject(IExtObjectContainer oc)
		{
			IQuery query = oc.Query();
			query.Descend("_s").Constrain(testString + COUNT / 2);
			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Size());
			SimpleObject o = (SimpleObject)result.Next();
			int i = o.GetI();
			Assert.IsTrue(COUNT <= i && i < COUNT + ThreadCount());
		}

		/// <exception cref="Exception"></exception>
		public virtual void ConcUpdateDifferentObject(IExtObjectContainer oc, int seq)
		{
			IQuery query = oc.Query();
			query.Descend("_s").Constrain(testString + seq).And(query.Descend("_i").Constrain
				(seq));
			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Size());
			SimpleObject o = (SimpleObject)result.Next();
			o.SetI(seq + COUNT);
			oc.Set(o);
		}

		/// <exception cref="Exception"></exception>
		public virtual void CheckUpdateDifferentObject(IExtObjectContainer oc)
		{
			IObjectSet result = oc.Query(typeof(SimpleObject));
			Assert.AreEqual(COUNT, result.Size());
			while (result.HasNext())
			{
				SimpleObject o = (SimpleObject)result.Next();
				int i = o.GetI();
				if (i >= COUNT)
				{
					i -= COUNT;
				}
				Assert.AreEqual(testString + i, o.GetS());
			}
		}
	}
}
