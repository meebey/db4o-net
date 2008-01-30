namespace Db4oUnit.Extensions
{
	/// <summary>
	/// Additional helper methods to make it easier to create
	/// test cases in c#.
	/// </summary>
	public partial class AbstractDb4oTestCase
	{
		protected T RetrieveOnlyInstance<T>()
		{
			return (T) RetrieveOnlyInstance(typeof(T));
		}
	}
}
