namespace Db4objects.Db4o.Constraints
{
	/// <summary>base class for all constraint exceptions.</summary>
	/// <remarks>base class for all constraint exceptions.</remarks>
	[System.Serializable]
	public abstract class ConstraintViolationException : Db4objects.Db4o.Ext.Db4oException
	{
		public ConstraintViolationException(string msg) : base(msg)
		{
		}
	}
}
