/* Copyright (C) 2004-2007   db4objects Inc.   http://www.db4o.com */
using Db4objects.Db4o.Config;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.TA.Tests.CLI1
{
	using StringIntP = Pair<string, int>;
	using IntC = Container<int>;
	using IntCStringIntP = Pair<Container<int>, Pair<string, int>>;
	using IntCStringIntC = Container<Pair<Container<int>, Pair<string, int>>>;
	using IntCStringIntIntP = Pair<Container<Pair<Container<int>, Pair<string, int>>>, int>;
	using IntCStringIntIntPC = Container<Pair<Container<Pair<Container<int>, Pair<string, int>>>, int>>;

	class Container<T> : ActivatableImpl where T: struct 
	{
		private string _name;
		private T _value;

		public Container(string name, T value)
		{
			_name = name;
			_value = value;
		}

		/// <summary>
		/// Activatable based implementation. Activates the
		/// object before field access.
		/// </summary>
		public string Name
		{
			get
			{
				Activate();
				return _name;
			}
		}

		/// <summary>
		/// Activatable based implementation. Activates the
		/// object before field access.
		/// </summary>
		public T Value
		{
			get
			{
				Activate();
				return _value;
			}
		}

		/// <summary>
		/// Bypass activation and access the field directly.
		/// </summary>
		public T PassThroughValue
		{
			get { return _value; }
		}

		public string PassThroughName
		{
			get { return _name; }
		}
	}

	struct Pair<T0, T1> where T1: struct
	{
		public T0 First;
		public T1 Second;

		public Pair(T0 first, T1 second)
		{
			First = first;
			Second = second;
		}
	}

	class ValueTypeActivationTestCase : AbstractDb4oTestCase
	{
		protected override void Configure(IConfiguration config)
		{
			config.Add(new TransparentActivationSupport());
		}

		// the object graph goes like this:
		// container
		//	pair
		//		first: container
		//			pair:
		//				first: container(21)
		//				second: pair
		//					first: "foo"
		//					second: 11
		//		second: 42

		protected override void Store()
		{
			Store(
				new IntCStringIntIntPC(
					"root",
					new IntCStringIntIntP(
						new IntCStringIntC(
							"child",
							new IntCStringIntP(
								new IntC("grandchild", 21),
								new StringIntP("foo", 11)
							)
						),
						42
					)
				)
			);
		}

		public void TestDepth0()
		{
			IntCStringIntIntPC root = GetRoot();
			AssertContainerNotActivated(root);
		}

		public void TestDepth1()
		{
			IntCStringIntIntPC root = GetRoot();
			Assert.AreEqual("root", root.Name, "ta");
			Assert.IsNotNull(root.Value.First, "ta");
			Assert.AreEqual(42, root.Value.Second, "ta");

			AssertContainerNotActivated(root.Value.First);
		}

		public void TestDepthN()
		{
			IntCStringIntIntPC root = GetRoot();
			AssertContainerNotActivated(root.Value.First.Value.First);
			Assert.AreEqual(21, root.Value.First.Value.First.Value);
			Assert.AreEqual(new StringIntP("foo", 11), root.Value.First.Value.Second);
			
		}

		private static void AssertContainerNotActivated<T>(Container<T> container) where T: struct
		{
			Assert.IsNull(container.PassThroughName, "depth(0) shouldn't activate ref member");
			Assert.AreEqual(default(T), container.PassThroughValue, "depth(0) shouldn't activate nested value types");
		}

		private IntCStringIntIntPC GetRoot()
		{
			return (IntCStringIntIntPC)NewQuery(typeof(IntCStringIntIntPC)).Execute().Next();
		}
	}
}
