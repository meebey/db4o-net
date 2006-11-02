using System;
using System.Collections;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1.Events
{
	public class EventRegistryTestCase : AbstractDb4oTestCase
	{
		class Item
		{	
		}
		
		class EventRecorder
		{
			ArrayList _events = new ArrayList();

			public EventRecorder(IExtObjectContainer container)
			{
				IEventRegistry registry = EventRegistryFactory.ForObjectContainer(container);
				registry.Creating += new CancellableObjectEventHandler(OnCreating);
			}
			
			public string this[int index]
			{
				get { return (string)_events[index];  }
			}

			void OnCreating(object sender, CancellableObjectEventArgs args)
			{
				_events.Add("Creating");
				Assert.IsFalse(args.IsCancelled);
				args.Cancel();
			}
		}
		
		public void TestCreating()
		{
			EventRecorder recorder = new EventRecorder(Db());

			Store(new Item());

			Assert.AreEqual(0, Db().Get(typeof(Item)).Count);
			Assert.AreEqual("Creating", recorder[0]);
		}
	}
}
