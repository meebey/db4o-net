/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Drs.Inside;

namespace Db4objects.Drs.Inside
{
	public interface ITestableReplicationProviderInside : IReplicationProviderInside, 
		ISimpleObjectContainer
	{
		bool SupportsMultiDimensionalArrays();

		bool SupportsHybridCollection();

		bool SupportsRollback();

		bool SupportsCascadeDelete();
	}
}
