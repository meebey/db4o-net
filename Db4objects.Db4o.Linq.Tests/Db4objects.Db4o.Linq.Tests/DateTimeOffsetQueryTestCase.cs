/* Copyright (C) 2009  Versant Inc.  http://www.db4o.com */
using System;
using System.Linq;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Query;
using Db4oUnit;

namespace Db4objects.Db4o.Linq.Tests
{
	class DateTimeOffsetQueryTestCase : AbstractDb4oLinqTestCase
	{
#if !CF
		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(Item)).ObjectField("indexed").Indexed(true);
		}

		protected override void Store()
		{
			foreach (Item item in Items)
			{
				Store(item);
			}
		}

		public void TestIndexed()
		{
			DateTimeOffset now = DateTimeOffset.Now;
			AssertQuery(
				from Item item in Db() where item.Indexed > now select item,
				string.Format("(Item(indexed > {0}))", now),
				from candidate in Items where candidate.Indexed > now select candidate);	
		}

		public void TestNonIndexed()
		{
			DateTimeOffset now = DateTimeOffset.Now;
			AssertQuery(
				from Item item in Db() where item.NonIndexed > now select item,
				string.Format("(Item(nonIndexed > {0}))", now),
				from candidate in Items where candidate.NonIndexed > now select candidate);
		}	
		
		//TODO: Not working yet. Improve ValueTypeHandlerTestCaseBase to include this scenario.
		public void _TestUntyped()
		{
			IQuery query = NewQuery(typeof (Item));
			query.Descend("untyped").Constrain(DateTimeOffset.Now).Greater();

			Assert.AreEqual(3, query.Execute().Count);
			//DateTimeOffset now = DateTimeOffset.Now;
			//AssertQuery(
			//    from Item item in Db() where ((DateTimeOffset) item.Untyped) > now select item,
			//    string.Format("(Item(untyped > {0}))", now),
			//    from candidate in Items where ((DateTimeOffset) candidate.Untyped) > now select candidate);
		}

		private class Item
		{
			public Item(DateTimeOffset dateTimeOffset)
			{
				untyped = dateTimeOffset;
				nonIndexed = dateTimeOffset;
				indexed = dateTimeOffset;
			}

			public DateTimeOffset Indexed
			{
				get { return indexed; }
			}

			public DateTimeOffset NonIndexed
			{
				get { return nonIndexed; }
			}

			public object Untyped
			{
				get { return untyped; }
			}

			public override bool Equals(object obj)
			{
				Item other = obj as Item;
				if (null == other)
				{
					return false;
				}
				
				return untyped.Equals(other.untyped) && indexed == other.Indexed && nonIndexed == other.nonIndexed;
			}

			public override int GetHashCode()
			{
				return untyped.GetHashCode() + indexed.GetHashCode() + nonIndexed.GetHashCode();
			}

			private readonly DateTimeOffset indexed;
			private readonly DateTimeOffset nonIndexed;
			private readonly object untyped;
		}

		private static Item[] Items = new[]
										{
											new Item(DateTimeOffset.Now.AddHours(-3)), 
											new Item(DateTimeOffset.Now.AddHours(-2)), 
											new Item(DateTimeOffset.Now.AddHours(-1)), 
											new Item(DateTimeOffset.Now.AddHours(1)), 
											new Item(DateTimeOffset.Now.AddHours(2)), 
											new Item(DateTimeOffset.Now.AddHours(3)), 
										};
#endif
	}
}
