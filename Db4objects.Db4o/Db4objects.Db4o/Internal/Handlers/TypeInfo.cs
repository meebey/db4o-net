/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fieldhandlers;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class TypeInfo
	{
		public ClassMetadata classMetadata;

		public IFieldHandler fieldHandler;

		public ITypeHandler4 typeHandler;

		public IReflectClass classReflector;

		public TypeInfo(ClassMetadata classMetadata_, IFieldHandler fieldHandler_, ITypeHandler4
			 typeHandler_, IReflectClass classReflector_)
		{
			// TODO: remove when no longer needed in HandlerRegistry
			classMetadata = classMetadata_;
			fieldHandler = fieldHandler_;
			typeHandler = typeHandler_;
			classReflector = classReflector_;
		}
	}
}
