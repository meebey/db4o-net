namespace Db4oUnit
{
	[System.Serializable]
	public class TearDownFailureException : Db4oUnit.TestException
	{
		private const long serialVersionUID = -5998743679496701084L;

		public TearDownFailureException(System.Exception cause) : base(cause)
		{
		}
	}
}
