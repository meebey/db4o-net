/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Reflect;

namespace Db4oUnit.Extensions
{
	public class ExcludingReflector : Db4objects.Db4o.Reflect.Net.NetReflector
	{
		private readonly Collection4 _excludedClasses;

		public ExcludingReflector(Type[] excludedClasses)
		{
			_excludedClasses = new Collection4();
			for (int claxxIndex = 0; claxxIndex < excludedClasses.Length; ++claxxIndex)
			{
				Type claxx = excludedClasses[claxxIndex];
				_excludedClasses.Add(claxx.FullName);
			}
		}

		public override IReflectClass ForName(string className)
		{
			if (_excludedClasses.Contains(className))
			{
				return null;
			}
			return base.ForName(className);
		}

		public override IReflectClass ForClass(Type clazz)
		{
			if (_excludedClasses.Contains(clazz.FullName))
			{
				return null;
			}
			return base.ForClass(clazz);
		}
	}
}
