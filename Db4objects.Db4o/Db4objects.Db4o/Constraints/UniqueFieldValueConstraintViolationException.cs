namespace Db4objects.Db4o.Constraints
{
	/// <summary>can be thrown by a UniqueFieldValueConstraint on commit.</summary>
	/// <remarks>can be thrown by a UniqueFieldValueConstraint on commit.</remarks>
	[System.Serializable]
	public class UniqueFieldValueConstraintViolationException : Db4objects.Db4o.Constraints.ConstraintViolationException
	{
		public UniqueFieldValueConstraintViolationException(string className, string fieldName
			) : base("class: " + className + " field: " + fieldName)
		{
		}
	}
}
