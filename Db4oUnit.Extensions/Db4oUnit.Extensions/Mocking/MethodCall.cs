/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit.Extensions.Mocking
{
	public class MethodCall
	{
		private sealed class _object_8 : object
		{
			public _object_8()
			{
			}

			public override string ToString()
			{
				return "...";
			}
		}

		public static readonly object IgnoredArgument = new _object_8();

		public readonly string methodName;

		public readonly object[] args;

		public MethodCall(string methodName, object[] args)
		{
			this.methodName = methodName;
			this.args = args;
		}

		public MethodCall(string methodName, object singleArg) : this(methodName, new object
			[] { singleArg })
		{
		}

		public MethodCall(string methodName, object arg1, object arg2) : this(methodName, 
			new object[] { arg1, arg2 })
		{
		}

		public override string ToString()
		{
			return methodName + "(" + Iterators.Join(Iterators.Iterate(args), ", ") + ")";
		}

		public override bool Equals(object obj)
		{
			if (null == obj)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				return false;
			}
			Db4oUnit.Extensions.Mocking.MethodCall other = (Db4oUnit.Extensions.Mocking.MethodCall
				)obj;
			if (!methodName.Equals(other.methodName))
			{
				return false;
			}
			if (args.Length != other.args.Length)
			{
				return false;
			}
			for (int i = 0; i < args.Length; ++i)
			{
				if (args[i] == IgnoredArgument)
				{
					continue;
				}
				if (!Check.ObjectsAreEqual(args[i], other.args[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
