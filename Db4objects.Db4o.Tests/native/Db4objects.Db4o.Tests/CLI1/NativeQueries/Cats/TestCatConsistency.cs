using System.Collections;
using Db4objects.Db4o.Query;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1.NativeQueries.Cats
{
	public class TestCatConsistency : AbstractDb4oTestCase
	{
		override protected void Store()
		{
			StoreCats();
		}

		public void Test()
		{
			try
			{
				Db().Configure().OptimizeNativeQueries(true);
				RunTests();
				Db().Configure().OptimizeNativeQueries(false);
				RunTests();
			}
			finally
			{
				Db().Configure().OptimizeNativeQueries(true);
			}

		}

		public class NoneFound : Predicate
		{
			public bool Match(Cat cat)
			{
				return cat._age == 7;
			}
		}

		public class AgeOne : Predicate
		{
			public bool Match(Cat cat)
			{
				return cat._age == 1;
			}
		}

		public class FatherAgeOne : Predicate
		{
			public bool Match(Cat cat)
			{
				return cat._father._age == 1;
			}
		}

		public class GrandFatherName : Predicate
		{
			public bool Match(Cat cat)
			{
				return cat._father._father._firstName == "Edwin";
			}
		}

		public class OrFatherName : Predicate
		{
			public bool Match(Cat cat)
			{
				return (cat._father._father != null && cat._father._father._firstName == "Edwin")
					|| cat._father._firstName == "Edwin";
			}
		}

		public class AddToAge : Predicate
		{
			public bool Match(Cat cat)
			{
				return cat._age + 1 == 2;
			}
		}

		public class TwoGetters : Predicate
		{
			public bool Match(Cat cat)
			{
				return cat.GetFirstName() == "Occam"
					&& cat.GetAge() == 1;
			}
		}

		public class CalculatedGetter : Predicate
		{
			public bool Match(Cat cat)
			{
				return cat.GetFullName() == "Achat Leo Lenis";
			}
		}

		public class GetterNull : Predicate
		{
			public bool Match(Cat cat)
			{
				return cat.GetFullName() == null;
			}
		}

		public class StartsWith : Predicate
		{
			public bool Match(Cat cat)
			{
				return cat._firstName.StartsWith("A");
			}
		}

		public void RunTests()
		{
			Expect(new NoneFound());
			Expect(new AgeOne(), "Occam", "Vahiné");
			Expect(new FatherAgeOne(), "Achat", "Acrobat");
			Expect(new GrandFatherName(), "Achat", "Acrobat");
			Expect(new OrFatherName(), "Achat", "Acrobat", "Occam");
			Expect(new AddToAge(), "Occam", "Vahiné");
			Expect(new TwoGetters(), "Occam");
			Expect(new CalculatedGetter(), "Achat");
			Expect(new GetterNull());
			Expect(new StartsWith(), "Achat", "Acrobat");

#if NET_2_0

			Expect<Cat>(delegate(Cat cat)
			{
				return cat._age == 7;
			});
			Expect<Cat>(delegate(Cat cat)
			{
				return cat._age == 1;
			}, "Occam", "Vahiné");
			Expect<Cat>(delegate(Cat cat)
			{
				return cat._father._age == 1;
			}, "Achat", "Acrobat");
			Expect<Cat>(delegate(Cat cat)
			{
				return cat._father._father._firstName == "Edwin";
			}, "Achat", "Acrobat");
			Expect<Cat>(delegate(Cat cat)
			{
				return (cat._father._father != null &&
						cat._father._father._firstName == "Edwin")
					|| cat._father._firstName == "Edwin";
			}, "Achat", "Acrobat", "Occam");
			Expect<Cat>(delegate(Cat cat)
			{
				return cat._age + 1 == 2;
			}, "Occam", "Vahiné");
			Expect<Cat>(delegate(Cat cat)
			{
				return cat.GetFirstName() == "Occam"
					&& cat.GetAge() == 1;
			}, "Occam");
			Expect<Cat>(delegate(Cat cat)
			{
				return cat.GetFullName() == "Achat Leo Lenis";
			}, "Achat");
			Expect<Cat>(delegate(Cat cat)
			{
				return cat.GetFullName() == null;
			});
			Expect<Cat>(delegate(Cat cat)
			{
				return cat._firstName.StartsWith("A");
			}, "Achat", "Acrobat");
#endif

		}

		public void StoreCats()
		{

			Cat winni = new Cat();
			winni._sex = Animal.MALE;
			winni._firstName = "Edwin";
			winni._lastName = "Sanddrops";
			winni._age = 12;

			Cat bachi = new Cat();
			bachi._sex = Animal.FEMALE;
			bachi._firstName = "Frau Bachmann";
			bachi._lastName = "von der Bärenhöhle";
			bachi._age = 10;

			Cat occam = new Cat();
			occam._sex = Animal.MALE;
			occam._firstName = "Occam";
			occam._lastName = "von der Bärenhöhle";
			occam._age = 1;
			occam._father = winni;
			occam._mother = bachi;

			Cat zora = new Cat();
			zora._sex = Animal.FEMALE;
			zora._firstName = "Vahiné";
			zora._lastName = "des Fauves et Or";
			zora._age = 1;

			Cat achat = new Cat();
			achat._sex = Animal.FEMALE;
			achat._firstName = "Achat";
			achat._lastName = "Leo Lenis";
			achat._father = occam;
			achat._mother = zora;

			Cat acrobat = new Cat();
			acrobat._sex = Animal.FEMALE;
			acrobat._firstName = "Acrobat";
			acrobat._lastName = "Leo Lenis";
			acrobat._father = occam;
			acrobat._mother = zora;

			Store(achat);
			Store(acrobat);

			Cat trulla = new Cat();
			trulla._firstName = "Trulla";
			Store(trulla);
		}

		private void Expect(Predicate predicate, params string[] names)
		{
			Expect(Db().Query(predicate), names);
		}

#if NET_2_0
		private void Expect<Extent>(System.Predicate<Extent> match, params string[] names)
		{
			System.Collections.Generic.IList<Extent> list = Db().Query(match);
			Expect(ToUntypedList(list), names);
		}

		private static IList ToUntypedList<T>(System.Collections.Generic.IList<T> list)
		{
			ArrayList untypedList = new ArrayList(list.Count);
			foreach (object item in list)
			{
				untypedList.Add(item);
			}
			return untypedList;
		}
#endif

		private static void Expect(IList list, string[] names)
		{
			if (names == null)
			{
				names = new string[0];
			}
			
			foreach (string name in names)
			{
				Assert.IsTrue(ContainsCat(list, name), "Expected '" + name + "'");
			}
		}

		private static bool ContainsCat(IList list, string name)
		{
			foreach (Cat cat in list)
			{
				if (cat._firstName == name)
				{
					return true;
				}
			}
			return false;
		}
	}
}
