/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Query
{
	/// <summary>
	/// constraint to limit the objects returned upon
	/// <see cref="IQuery.Execute">query execution</see>
	/// .
	/// <br /><br />
	/// Constraints are constructed by calling
	/// <see cref="IQuery.Constrain">Query.constrain()</see>
	/// .
	/// <br /><br />
	/// Constraints can be joined with the methods
	/// <see cref="IConstraint.And">IConstraint.And</see>
	/// and
	/// <see cref="IConstraint.Or">IConstraint.Or</see>
	/// .
	/// <br /><br />
	/// The methods to modify the constraint evaluation algorithm may
	/// be merged, to construct combined evaluation rules.
	/// Examples:
	/// <ul>
	/// <li> <code>Constraint#smaller().equal()</code> for "smaller or equal" </li>
	/// <li> <code>Constraint#not().like()</code> for "not like" </li>
	/// <li> <code>Constraint#not().greater().equal()</code> for "not greater or equal" </li>
	/// </ul>
	/// </summary>
	public interface IConstraint
	{
		/// <summary>links two Constraints for AND evaluation.</summary>
		/// <remarks>links two Constraints for AND evaluation.</remarks>
		/// <param name="with">
		/// the other
		/// <see cref="IConstraint">IConstraint</see>
		/// </param>
		/// <returns>
		/// a new
		/// <see cref="IConstraint">IConstraint</see>
		/// , that can be used for further calls
		/// to
		/// <see cref="IConstraint.And">and()</see>
		/// and
		/// <see cref="IConstraint.Or">or()</see>
		/// </returns>
		IConstraint And(IConstraint with);

		/// <summary>links two Constraints for OR evaluation.</summary>
		/// <remarks>links two Constraints for OR evaluation.</remarks>
		/// <param name="with">
		/// the other
		/// <see cref="IConstraint">IConstraint</see>
		/// </param>
		/// <returns>
		/// a new
		/// <see cref="IConstraint">IConstraint</see>
		/// , that can be used for further calls
		/// to
		/// <see cref="IConstraint.And">and()</see>
		/// and
		/// <see cref="IConstraint.Or">or()</see>
		/// </returns>
		IConstraint Or(IConstraint with);

		/// <summary>sets the evaluation mode to <code>==</code>.</summary>
		/// <remarks>sets the evaluation mode to <code>==</code>.</remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Equal();

		/// <summary>sets the evaluation mode to <code>&gt;</code>.</summary>
		/// <remarks>sets the evaluation mode to <code>&gt;</code>.</remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Greater();

		/// <summary>sets the evaluation mode to <code>&lt;</code>.</summary>
		/// <remarks>sets the evaluation mode to <code>&lt;</code>.</remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Smaller();

		/// <summary>sets the evaluation mode to identity comparison.</summary>
		/// <remarks>sets the evaluation mode to identity comparison.</remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Identity();

		/// <summary>sets the evaluation mode to "like" comparison.</summary>
		/// <remarks>
		/// sets the evaluation mode to "like" comparison.
		/// <br /><br />Constraints are compared to the first characters of a field.<br /><br />
		/// </remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Like();

		/// <summary>sets the evaluation mode to containment comparison.</summary>
		/// <remarks>sets the evaluation mode to containment comparison.</remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Contains();

		/// <summary>sets the evaluation mode to string startsWith comparison.</summary>
		/// <remarks>sets the evaluation mode to string startsWith comparison.</remarks>
		/// <param name="caseSensitive">comparison will be case sensitive if true, case insensitive otherwise
		/// 	</param>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint StartsWith(bool caseSensitive);

		/// <summary>sets the evaluation mode to string endsWith comparison.</summary>
		/// <remarks>sets the evaluation mode to string endsWith comparison.</remarks>
		/// <param name="caseSensitive">comparison will be case sensitive if true, case insensitive otherwise
		/// 	</param>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint EndsWith(bool caseSensitive);

		/// <summary>turns on not() comparison.</summary>
		/// <remarks>turns on not() comparison.</remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Not();

		/// <summary>
		/// returns the Object the query graph was constrained with to
		/// create this
		/// <see cref="IConstraint">IConstraint</see>
		/// .
		/// </summary>
		/// <returns>Object the constraining object.</returns>
		object GetObject();
	}
}
