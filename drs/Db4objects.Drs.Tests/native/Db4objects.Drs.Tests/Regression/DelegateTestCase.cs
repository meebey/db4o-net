using System;
using System.Collections.Generic;
using Db4objects.Db4o;
using Db4oUnit;

namespace Db4objects.Drs.Tests.Regression
{
	class DelegateTestCase : DrsTestCase
	{
		public class Item
		{
			public event EventHandler Foo;

			public string Value;

			public object untyped;

			public object[] array;

			public Item(string value)
			{
				Value = value;
			}

			public int HandlerCount
			{
				get
				{
					if (Foo == null) return 0;
					return Foo.GetInvocationList().Length;
				}
			}
		}

		public void TestDelegateFields()
		{
			Item item = new Item("the item");
			item.Foo += delegate { };
			item.untyped = StringAction();
			item.array = new object[] { StringAction() };

			Item replicated = Replicate(item);
			Assert.IsNotNull(replicated);
			Assert.AreEqual(item.Value, replicated.Value);
			Assert.AreEqual(0, replicated.HandlerCount);
			Assert.IsNull(replicated.untyped);
			Assert.AreEqual(1, replicated.array.Length);
			Assert.IsNull(replicated.array[0]);
		}

		private static Action<string> StringAction()
		{
			return new System.Action<string>(Console.WriteLine);
		}

		public class DictionaryHolder
		{
			public Dictionary<string, Action<string>> actionDictionary;
			public object untyped;
			public object[] array;
		}

		public void TestDictionaryHolder()
		{
			DictionaryHolder item = new DictionaryHolder();
			item.actionDictionary = NewActionDictionary();
			item.untyped = NewActionDictionary();
			item.array = new object[] { NewActionDictionary() };

			DictionaryHolder replicated = Replicate(item);
			Assert.IsNotNull(replicated.actionDictionary);
			Assert.IsNotNull(replicated.untyped);
			Assert.IsNotNull(replicated.array[0]);
		}

		private static Dictionary<string, Action<string>> NewActionDictionary()
		{
			Dictionary<string, Action<string>> d = new Dictionary<string, Action<string>>();
			d["print"] = StringAction();
			return d;
		}

		private T Replicate<T>(T item)
		{
			StoreToA(item);

			ReplicateAll();

			return QueryReplicated<T>();
		}

		private T QueryReplicated<T>()
		{
			IObjectSet found = B().Provider().GetStoredObjects(typeof(T));
			Assert.AreEqual(1, found.Count);
			return (T)found.Next();
		}

		private void ReplicateAll()
		{
			ReplicateAll(A().Provider(), B().Provider());
		}

		private void StoreToA(object item)
		{
			A().Provider().StoreNew(item);
			A().Provider().Commit();
		}
	}
}
