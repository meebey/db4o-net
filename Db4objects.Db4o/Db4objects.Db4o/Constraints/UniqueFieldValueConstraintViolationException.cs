/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Constraints;

namespace Db4objects.Db4o.Constraints
{
	/// <summary>
	/// db4o-specific exception.<br /><br />
	/// This exception can be thrown by a
	/// <see cref="UniqueFieldValueConstraint">UniqueFieldValueConstraint</see>
	/// on commit.
	/// </summary>
	/// <seealso cref="IObjectField.Indexed">IObjectField.Indexed</seealso>
	/// <seealso cref="IConfiguration.Add">IConfiguration.Add</seealso>
	[System.Serializable]
	public class UniqueFieldValueConstraintViolationException : ConstraintViolationException
	{
		public UniqueFieldValueConstraintViolationException(string className, string fieldName
			) : base("class: " + className + " field: " + fieldName)
		{
		}
	}
}
