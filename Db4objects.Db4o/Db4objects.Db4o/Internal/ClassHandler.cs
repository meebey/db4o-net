/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class ClassHandler
	{
		private readonly ClassMetadata _classMetadata;

		public ClassHandler(ClassMetadata classMetadata)
		{
			_classMetadata = classMetadata;
		}

		public virtual bool CustomizedNewInstance()
		{
			return ConfigInstantiates();
		}

		private bool ConfigInstantiates()
		{
			return Config() != null && Config().Instantiates();
		}

		public virtual object InstantiateObject(StatefulBuffer buffer, MarshallerFamily mf
			)
		{
			if (ConfigInstantiates())
			{
				return _classMetadata.InstantiateFromConfig(buffer.GetStream(), buffer, mf);
			}
			return _classMetadata.InstantiateFromReflector(buffer.GetStream());
		}

		public virtual Config4Class Config()
		{
			return _classMetadata.Config();
		}

		public virtual IReflectClass ClassSubstitute()
		{
			return null;
		}

		public virtual bool IgnoreAncestor()
		{
			return false;
		}
	}
}
