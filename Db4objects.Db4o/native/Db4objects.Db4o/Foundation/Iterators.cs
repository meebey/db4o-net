using System.Collections;
using System.Collections.Generic;

namespace Db4objects.Db4o.Foundation
{
	public delegate B Function<A, B>(A a);

	public struct Tuple<A, B>
	{
		public A a;
		public B b;

		public Tuple(A a, B b)
		{
			this.a = a;
			this.b = b;
		}
	}

	public partial class Iterators
	{
		public static IEnumerable<T> Cast<T>(IEnumerable source)
		{
			foreach (object o in source) yield return (T) o;
		}

		public static IEnumerable<Tuple<object, object>> Zip(IEnumerable @as, IEnumerable bs)
		{
			return Zip(Cast<object>(@as), Cast<object>(bs));
		}

		public static IEnumerable<Tuple<A, B>> Zip<A, B>(IEnumerable<A> @as, IEnumerable<B> bs)
		{
			IEnumerator<B> bsEnumerator = bs.GetEnumerator();
			foreach (A a in @as)
			{
				if (!bsEnumerator.MoveNext())
				{
					yield break;
				}

				yield return new Tuple<A, B>(a, bsEnumerator.Current);
			}
		}
	}
}
