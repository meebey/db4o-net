/* Copyright (C) 2009  Versant Inc.  http://www.db4o.com */
#if NET_2_0 || CF_2_0
namespace System
{
	public delegate void Action();
}
#endif

namespace Db4objects.Db4o.Internal
{

	public partial interface IInternalObjectContainer
	{
		void WithEnvironment(System.Action action);
	}
}
