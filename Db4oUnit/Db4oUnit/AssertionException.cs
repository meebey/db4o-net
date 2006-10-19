namespace Db4oUnit
{
	[System.Serializable]
	public class AssertionException : System.Exception
	{
		private const long serialVersionUID = 900088031151055525L;

		public AssertionException(string s) : base(s)
		{
		}
	}
}
