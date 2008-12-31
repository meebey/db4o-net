/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Constraints;

namespace Db4objects.Db4o.Constraints
{
	/// <summary>
	/// db4o-specific exception.<br /><br />
	/// This exception can be thrown by a
	/// <see cref="Db4objects.Db4o.Constraints.UniqueFieldValueConstraint">Db4objects.Db4o.Constraints.UniqueFieldValueConstraint
	/// 	</see>
	/// on commit.
	/// </summary>
	/// <seealso cref="Db4objects.Db4o.Config.IObjectField.Indexed">Db4objects.Db4o.Config.IObjectField.Indexed
	/// 	</seealso>
	/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Add">Db4objects.Db4o.Config.IConfiguration.Add
	/// 	</seealso>
	[System.Serializable]
	public class UniqueFieldValueConstraintViolationException : ConstraintViolationException
	{
		/// <summary>
		/// Constructor with a message composed from the class and field
		/// name of the entity causing the exception.
		/// </summary>
		/// <remarks>
		/// Constructor with a message composed from the class and field
		/// name of the entity causing the exception.
		/// </remarks>
		/// <param name="className">class, which caused the exception</param>
		/// <param name="fieldName">field, which caused the exception</param>
		public UniqueFieldValueConstraintViolationException(string className, string fieldName
			) : base("class: " + className + " field: " + fieldName)
		{
		}
	}
}
