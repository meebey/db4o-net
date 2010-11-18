/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Drs.Inside;

namespace Db4objects.Drs.Inside
{
	public interface ITestableReplicationProviderInside : IReplicationProviderInside, 
		ISimpleObjectContainer
	{
		bool SupportsMultiDimensionalArrays();

		bool SupportsHybridCollection();

		bool SupportsRollback();

		void Commit();
	}
}
