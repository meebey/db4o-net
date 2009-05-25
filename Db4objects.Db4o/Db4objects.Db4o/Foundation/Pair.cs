/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Foundation
{
	public class Pair
	{
		public static Db4objects.Db4o.Foundation.Pair Of(object first, object second)
		{
			return new Db4objects.Db4o.Foundation.Pair(first, second);
		}

		public object first;

		public object second;

		public Pair(object first, object second)
		{
			this.first = first;
			this.second = second;
		}

		public override string ToString()
		{
			return "Pair.of(" + first + ", " + second + ")";
		}
	}
}
