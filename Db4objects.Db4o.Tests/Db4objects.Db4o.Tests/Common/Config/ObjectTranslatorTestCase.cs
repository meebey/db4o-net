namespace Db4objects.Db4o.Tests.Common.Config
{
	public class ObjectTranslatorTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Thing
		{
			public string name;

			public Thing(string name)
			{
				this.name = name;
			}
		}

		public class ThingCounterTranslator : Db4objects.Db4o.Config.IObjectConstructor
		{
			private Db4objects.Db4o.Foundation.Hashtable4 _countCache = new Db4objects.Db4o.Foundation.Hashtable4
				();

			public virtual void OnActivate(Db4objects.Db4o.IObjectContainer container, object
				 applicationObject, object storedObject)
			{
			}

			public virtual object OnStore(Db4objects.Db4o.IObjectContainer container, object 
				applicationObject)
			{
				Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase.Thing t = (Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase.Thing
					)applicationObject;
				AddToCache(t);
				return t.name;
			}

			private void AddToCache(Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase.Thing
				 t)
			{
				object o = (object)_countCache.Get(t.name);
				if (o == null)
				{
					o = 0;
				}
				_countCache.Put(t.name, ((int)o) + 1);
			}

			public virtual int GetCount(Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase.Thing
				 t)
			{
				object o = (int)_countCache.Get(t.name);
				if (o == null)
				{
					return 0;
				}
				return ((int)o);
			}

			public virtual object OnInstantiate(Db4objects.Db4o.IObjectContainer container, object
				 storedObject)
			{
				string name = (string)storedObject;
				return new Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase.Thing(name
					);
			}

			public virtual System.Type StoredClass()
			{
				return typeof(string);
			}
		}

		private Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase.ThingCounterTranslator
			 _trans;

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase.Thing)
				).Translate(_trans = new Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase.ThingCounterTranslator
				());
		}

		protected override void Store()
		{
			Db().Set(new Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase.Thing("jbe"
				));
		}

		public virtual void _testTranslationCount()
		{
			Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase.Thing t = (Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase.Thing
				)RetrieveOnlyInstance(typeof(Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase.Thing)
				);
			Db4oUnit.Assert.IsNotNull(t);
			Db4oUnit.Assert.AreEqual("jbe", t.name);
			Db4oUnit.Assert.AreEqual(1, _trans.GetCount(t));
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Config.ObjectTranslatorTestCase().RunSolo();
		}
	}
}
