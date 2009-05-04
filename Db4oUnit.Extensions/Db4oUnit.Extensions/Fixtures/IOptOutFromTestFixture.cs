/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

namespace Db4oUnit.Extensions.Fixtures
{
	/// <summary>
	/// 'Abstract' marker interface to denote that implementing test cases should be
	/// excluded from running against specific fixtures.
	/// </summary>
	/// <remarks>
	/// 'Abstract' marker interface to denote that implementing test cases should be
	/// excluded from running against specific fixtures. Concrete marker interfaces
	/// for specific fixtures should extend OptOutFromTestFixture.
	/// </remarks>
	public interface IOptOutFromTestFixture
	{
	}
}
