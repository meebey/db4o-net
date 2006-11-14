namespace Db4oUnit.Extensions.Fixtures
{
	/// <summary>
	/// Marker interface to denote that implementing test cases should be excluded
	/// from running with the Client/Server fixture.
	/// </summary>
	/// <remarks>
	/// Marker interface to denote that implementing test cases should be excluded
	/// from running with the Client/Server fixture.
	/// </remarks>
	public interface IOptOutCS : Db4oUnit.Extensions.Fixtures.IOptOutFromTestFixture
	{
	}
}
