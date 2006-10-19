namespace Db4oUnit
{
	[System.Serializable]
	public class SetupFailureException : Db4oUnit.TestException
	{
		private const long serialVersionUID = -7835097105469071064L;

		public SetupFailureException(System.Exception cause) : base(cause)
		{
		}
	}
}
