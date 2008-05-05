/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Reflect.Core;

namespace Db4objects.Db4o.Reflect.Core
{
	public class ReflectConstructorSpec
	{
		private IReflectConstructor _constructor;

		private object[] _args;

		public ReflectConstructorSpec(IReflectConstructor constructor, object[] args)
		{
			_constructor = constructor;
			_args = args;
		}

		public virtual object NewInstance()
		{
			return _constructor.NewInstance(_args);
		}
	}
}
