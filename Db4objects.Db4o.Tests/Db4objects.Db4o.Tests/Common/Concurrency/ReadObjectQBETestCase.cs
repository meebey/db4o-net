/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Concurrency;
using Db4objects.Db4o.Tests.Common.Persistent;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class ReadObjectQBETestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new ReadObjectQBETestCase().RunConcurrency();
		}

		private static string testString = "simple test string";

		protected override void Store()
		{
			for (int i = 0; i < ThreadCount(); i++)
			{
				Store(new SimpleObject(testString + i, i));
			}
		}

		/// <exception cref="Exception"></exception>
		public virtual void ConcReadSameObject(IExtObjectContainer oc)
		{
			int mid = ThreadCount() / 2;
			SimpleObject example = new SimpleObject(testString + mid, mid);
			IObjectSet result = oc.QueryByExample(example);
			Assert.AreEqual(1, result.Size());
			Assert.AreEqual(example, result.Next());
		}

		/// <exception cref="Exception"></exception>
		public virtual void ConcReadDifferentObject(IExtObjectContainer oc, int seq)
		{
			SimpleObject example = new SimpleObject(testString + seq, seq);
			IObjectSet result = oc.QueryByExample(example);
			Assert.AreEqual(1, result.Size());
			Assert.AreEqual(example, result.Next());
		}
	}
}
