/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

#if !SILVERLIGHT
using System.Collections;
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class PrefetchConfigurationTestCase : ClientServerTestCaseBase, IOptOutAllButNetworkingCS
	{
		/// <exception cref="System.Exception"></exception>
		protected override void Db4oSetupAfterStore()
		{
			EnsureQueryGraphClassMetadataHasBeenExchanged();
		}

		public virtual void TestDefaultPrefetchDepth()
		{
			Assert.AreEqual(0, Client().Config().PrefetchDepth());
		}

		public virtual void TestPrefetchingBehaviorForClassOnlyQuery()
		{
			IQuery query = Client().Query();
			query.Constrain(typeof(PrefetchConfigurationTestCase.Item));
			AssertPrefetchingBehaviorFor(query, Msg.GetInternalIds);
		}

		public virtual void TestPrefetchingBehaviorForConstrainedQuery()
		{
			IQuery query = Client().Query();
			query.Constrain(typeof(PrefetchConfigurationTestCase.Item));
			query.Descend("child").Constrain(null);
			AssertPrefetchingBehaviorFor(query, Msg.QueryExecute);
		}

		public virtual void TestRefreshIsUnaffectedByPrefetchingBehavior()
		{
			IExtObjectContainer oc1 = Db();
			IExtObjectContainer oc2 = OpenNewClient();
			oc1.Configure().ClientServer().PrefetchDepth(1);
			oc2.Configure().ClientServer().PrefetchDepth(1);
			try
			{
				PrefetchConfigurationTestCase.Item itemFromClient1 = new PrefetchConfigurationTestCase.RootItem
					(new PrefetchConfigurationTestCase.Item());
				oc1.Store(itemFromClient1);
				oc1.Commit();
				itemFromClient1.child = null;
				oc1.Store(itemFromClient1);
				PrefetchConfigurationTestCase.Item itemFromClient2 = ((PrefetchConfigurationTestCase.RootItem
					)RetrieveOnlyInstance(oc2, typeof(PrefetchConfigurationTestCase.RootItem)));
				Assert.IsNotNull(itemFromClient2.child);
				oc1.Rollback();
				itemFromClient2 = ((PrefetchConfigurationTestCase.RootItem)RetrieveOnlyInstance(oc2
					, typeof(PrefetchConfigurationTestCase.RootItem)));
				oc2.Refresh(itemFromClient2, int.MaxValue);
				Assert.IsNotNull(itemFromClient2.child);
				oc1.Commit();
				itemFromClient2 = ((PrefetchConfigurationTestCase.RootItem)RetrieveOnlyInstance(oc2
					, typeof(PrefetchConfigurationTestCase.RootItem)));
				Assert.IsNotNull(itemFromClient2.child);
				oc1.Store(itemFromClient1);
				oc1.Commit();
				oc2.Refresh(itemFromClient2, int.MaxValue);
				itemFromClient2 = ((PrefetchConfigurationTestCase.RootItem)RetrieveOnlyInstance(oc2
					, typeof(PrefetchConfigurationTestCase.RootItem)));
				Assert.IsNull(itemFromClient2.child);
			}
			finally
			{
				oc2.Close();
			}
		}

		public virtual void TestPrefetchingDepth2Behavior()
		{
			StoreDepth2Graph();
			Client().Config().PrefetchObjectCount(2);
			Client().Config().PrefetchDepth(2);
			IQuery query = QueryForItemsWithChild();
			// TODO: items to level 3
			AssertQueryIterationProtocol(query, Msg.QueryExecute, new PrefetchConfigurationTestCase.Stimulus
				[] { new PrefetchConfigurationTestCase.Depth2Stimulus(this, new MsgD[] {  }), new 
				PrefetchConfigurationTestCase.Depth2Stimulus(this, new MsgD[] {  }), new PrefetchConfigurationTestCase.Depth2Stimulus
				(this, new MsgD[] { Msg.ReadMultipleObjects }) });
		}

		public virtual void TestDepth2WithPrefetching1()
		{
			StoreDepth2Graph();
			Client().Config().PrefetchObjectCount(2);
			Client().Config().PrefetchDepth(1);
			IQuery query = QueryForItemsWithChild();
			AssertQueryIterationProtocol(query, Msg.QueryExecute, new PrefetchConfigurationTestCase.Stimulus
				[] { new PrefetchConfigurationTestCase.Depth2Stimulus(this, new MsgD[] { Msg.ReadReaderById
				 }), new PrefetchConfigurationTestCase.Depth2Stimulus(this, new MsgD[] { Msg.ReadReaderById
				 }), new PrefetchConfigurationTestCase.Depth2Stimulus(this, new MsgD[] { Msg.ReadMultipleObjects
				, Msg.ReadReaderById }) });
		}

		private IQuery QueryForItemsWithChild()
		{
			IQuery query = Client().Query();
			query.Constrain(typeof(PrefetchConfigurationTestCase.Item));
			query.Descend("child").Constrain(null).Not();
			return query;
		}

		private void StoreDepth2Graph()
		{
			StoreAllAndPurge(new PrefetchConfigurationTestCase.Item[] { new PrefetchConfigurationTestCase.Item
				(new PrefetchConfigurationTestCase.Item()), new PrefetchConfigurationTestCase.Item
				(new PrefetchConfigurationTestCase.Item()), new PrefetchConfigurationTestCase.Item
				(new PrefetchConfigurationTestCase.Item()), new PrefetchConfigurationTestCase.Item
				() });
		}

		private void AssertPrefetchingBehaviorFor(IQuery query, MsgD expectedFirstMessage
			)
		{
			StoreFlatItemGraph();
			Client().Config().PrefetchObjectCount(2);
			Client().Config().PrefetchDepth(1);
			AssertQueryIterationProtocol(query, expectedFirstMessage, new PrefetchConfigurationTestCase.Stimulus
				[] { new PrefetchConfigurationTestCase.Stimulus(new MsgD[] {  }), new PrefetchConfigurationTestCase.Stimulus
				(new MsgD[] {  }), new PrefetchConfigurationTestCase.Stimulus(new MsgD[] { Msg.ReadMultipleObjects
				 }), new PrefetchConfigurationTestCase.Stimulus(new MsgD[] {  }), new PrefetchConfigurationTestCase.Stimulus
				(new MsgD[] { Msg.ReadMultipleObjects }) });
		}

		private void AssertQueryIterationProtocol(IQuery query, MsgD expectedResultMessage
			, PrefetchConfigurationTestCase.Stimulus[] stimuli)
		{
			IList messages = MessageCollector.ForServerDispatcher(ServerDispatcher());
			IObjectSet result = query.Execute();
			AssertMessages(messages, new MsgD[] { expectedResultMessage });
			messages.Clear();
			for (int stimulusIndex = 0; stimulusIndex < stimuli.Length; ++stimulusIndex)
			{
				PrefetchConfigurationTestCase.Stimulus stimulus = stimuli[stimulusIndex];
				stimulus.ActUpon(result);
				AssertMessages(messages, stimulus.expectedMessagesAfter);
				messages.Clear();
			}
			Assert.IsFalse(result.HasNext());
			AssertMessages(messages, new MsgD[] {  });
		}

		private class Depth2Stimulus : PrefetchConfigurationTestCase.Stimulus
		{
			public Depth2Stimulus(PrefetchConfigurationTestCase _enclosing, MsgD[] expectedMessagesAfter
				) : base(expectedMessagesAfter)
			{
				this._enclosing = _enclosing;
			}

			public override void ActUpon(IObjectSet result)
			{
				this.ActUpon(((PrefetchConfigurationTestCase.Item)result.Next()));
			}

			protected virtual void ActUpon(PrefetchConfigurationTestCase.Item item)
			{
				Assert.IsNotNull(item.child);
				this._enclosing.Db().Activate(item.child, 1);
			}

			private readonly PrefetchConfigurationTestCase _enclosing;
			// ensure no further messages are exchange
		}

		public class Stimulus
		{
			public readonly MsgD[] expectedMessagesAfter;

			public Stimulus(MsgD[] expectedMessagesAfter)
			{
				this.expectedMessagesAfter = expectedMessagesAfter;
			}

			public virtual void ActUpon(IObjectSet result)
			{
				Assert.IsNotNull(((PrefetchConfigurationTestCase.Item)result.Next()));
			}
		}

		private void AssertMessages(IList actualMessages, MsgD[] expectedMessages)
		{
			Iterator4Assert.AreEqual(expectedMessages, Iterators.Iterator(actualMessages));
		}

		private void EnsureQueryGraphClassMetadataHasBeenExchanged()
		{
			// ensures classmetadata exists for query objects
			IQuery query = Client().Query();
			query.Constrain(typeof(PrefetchConfigurationTestCase.Item));
			query.Descend("child").Constrain(null).Not();
			query.Descend("child").Constrain(null);
			Assert.AreEqual(0, query.Execute().Count);
		}

		private void StoreFlatItemGraph()
		{
			StoreAllAndPurge(new PrefetchConfigurationTestCase.Item[] { new PrefetchConfigurationTestCase.Item
				(), new PrefetchConfigurationTestCase.Item(), new PrefetchConfigurationTestCase.Item
				(), new PrefetchConfigurationTestCase.Item(), new PrefetchConfigurationTestCase.Item
				() });
		}

		private void StoreAllAndPurge(PrefetchConfigurationTestCase.Item[] items)
		{
			for (int itemIndex = 0; itemIndex < items.Length; ++itemIndex)
			{
				PrefetchConfigurationTestCase.Item item = items[itemIndex];
				StoreAndPurge(item);
			}
			Client().Commit();
		}

		private void StoreAndPurge(PrefetchConfigurationTestCase.Item item)
		{
			Client().Store(item);
			Purge(item);
		}

		private void Purge(PrefetchConfigurationTestCase.Item item)
		{
			Client().Purge(item);
			if (null != item.child)
			{
				Purge(item.child);
			}
		}

		public class Item
		{
			public Item(PrefetchConfigurationTestCase.Item child)
			{
				this.child = child;
			}

			public Item()
			{
			}

			public PrefetchConfigurationTestCase.Item child;
		}

		public class RootItem : PrefetchConfigurationTestCase.Item
		{
			public RootItem() : base()
			{
			}

			public RootItem(PrefetchConfigurationTestCase.Item child) : base(child)
			{
			}
		}
	}
}
#endif // !SILVERLIGHT
