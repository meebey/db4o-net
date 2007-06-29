/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class CustomizedClassHandler : ClassHandler
	{
		private readonly ICustomClassHandler _customHandler;

		public CustomizedClassHandler(ClassMetadata classMetadata, ICustomClassHandler customHandler
			) : base(classMetadata)
		{
			_customHandler = customHandler;
		}

		public override bool CustomizedNewInstance()
		{
			return _customHandler.CanNewInstance();
		}

		public override object InstantiateObject(StatefulBuffer buffer, MarshallerFamily 
			mf)
		{
			if (CustomizedNewInstance())
			{
				return _customHandler.NewInstance();
			}
			return base.InstantiateObject(buffer, mf);
		}

		public override IReflectClass ClassSubstitute()
		{
			return _customHandler.ClassSubstitute();
		}

		public override bool IgnoreAncestor()
		{
			return _customHandler.IgnoreAncestor();
		}
	}
}
