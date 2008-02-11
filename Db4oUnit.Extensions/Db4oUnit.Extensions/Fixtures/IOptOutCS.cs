/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions.Fixtures;

namespace Db4oUnit.Extensions.Fixtures
{
	/// <summary>
	/// Marker interface to denote that implementing test cases should be excluded
	/// from running both with the embedded and networking Client/Server fixture.
	/// </summary>
	/// <remarks>
	/// Marker interface to denote that implementing test cases should be excluded
	/// from running both with the embedded and networking Client/Server fixture.
	/// </remarks>
	public interface IOptOutCS : IOptOutFromTestFixture
	{
	}
}
