/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
namespace Db4objects.Db4o.Internal
{
	using System;
	using Db4objects.Db4o.Internal.Query;
	using Db4objects.Db4o.Internal.Query.Result;
	using Db4objects.Db4o.Internal.Query.Processor;
	using Db4objects.Db4o.Ext;

	/// <summary>
	/// </summary>
	/// <exclude />
    public abstract class ObjectContainerBase : Db4objects.Db4o.Internal.PartialObjectContainer, System.IDisposable
	{
		internal ObjectContainerBase(Db4objects.Db4o.Config.IConfiguration config, Db4objects.Db4o.Internal.ObjectContainerBase a_parent)
			: base(config, a_parent)
		{
		}
		
	}
}
