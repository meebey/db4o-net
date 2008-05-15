/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Reflect.Core;

namespace Db4objects.Db4o.Reflect.Core
{
	public class ReflectConstructorSpec
	{
		private IReflectConstructor _constructor;

		private object[] _args;

		private TernaryBool _canBeInstantiated;

		public static readonly Db4objects.Db4o.Reflect.Core.ReflectConstructorSpec UnspecifiedConstructor
			 = new Db4objects.Db4o.Reflect.Core.ReflectConstructorSpec(TernaryBool.Unspecified
			);

		public static readonly Db4objects.Db4o.Reflect.Core.ReflectConstructorSpec InvalidConstructor
			 = new Db4objects.Db4o.Reflect.Core.ReflectConstructorSpec(TernaryBool.No);

		public ReflectConstructorSpec(IReflectConstructor constructor, object[] args)
		{
			_constructor = constructor;
			_args = args;
			_canBeInstantiated = TernaryBool.Yes;
		}

		private ReflectConstructorSpec(TernaryBool canBeInstantiated)
		{
			_canBeInstantiated = canBeInstantiated;
			_constructor = null;
		}

		public virtual object NewInstance()
		{
			if (_constructor == null)
			{
				return null;
			}
			return _constructor.NewInstance(_args);
		}

		public virtual TernaryBool CanBeInstantiated()
		{
			return _canBeInstantiated;
		}
	}
}
