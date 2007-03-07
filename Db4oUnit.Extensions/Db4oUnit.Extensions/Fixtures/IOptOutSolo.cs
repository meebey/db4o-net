namespace Db4oUnit.Extensions.Fixtures
{
	/// <summary>
	/// Marker interface to denote that implementing test cases should be excluded
	/// from running with a Solo fixture.
	/// </summary>
	/// <remarks>
	/// Marker interface to denote that implementing test cases should be excluded
	/// from running with a Solo fixture.
	/// </remarks>
	public interface IOptOutSolo : Db4oUnit.Extensions.Fixtures.IOptOutFromTestFixture
	{
	}
}
