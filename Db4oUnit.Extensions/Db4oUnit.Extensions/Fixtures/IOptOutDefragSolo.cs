namespace Db4oUnit.Extensions.Fixtures
{
	/// <summary>
	/// Marker interface to denote that implementing test cases should be excluded
	/// from running against a defragmenting fixture.
	/// </summary>
	/// <remarks>
	/// Marker interface to denote that implementing test cases should be excluded
	/// from running against a defragmenting fixture.
	/// </remarks>
	public interface IOptOutDefragSolo : Db4oUnit.Extensions.Fixtures.IOptOutFromTestFixture
	{
	}
}
