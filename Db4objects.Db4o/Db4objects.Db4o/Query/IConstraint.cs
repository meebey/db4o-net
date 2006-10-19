namespace Db4objects.Db4o.Query
{
	/// <summary>
	/// constraint to limit the objects returned upon
	/// <see cref="Db4objects.Db4o.Query.IQuery.Execute">query execution</see>
	/// .
	/// <br /><br />
	/// Constraints are constructed by calling
	/// <see cref="Db4objects.Db4o.Query.IQuery.Constrain">Query.constrain()</see>
	/// .
	/// <br /><br />
	/// Constraints can be joined with the methods
	/// <see cref="Db4objects.Db4o.Query.IConstraint.And">Db4objects.Db4o.Query.IConstraint.And
	/// 	</see>
	/// and
	/// <see cref="Db4objects.Db4o.Query.IConstraint.Or">Db4objects.Db4o.Query.IConstraint.Or
	/// 	</see>
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
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// </param>
		/// <returns>
		/// a new
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// , that can be used for further calls
		/// to
		/// <see cref="Db4objects.Db4o.Query.IConstraint.And">and()</see>
		/// and
		/// <see cref="Db4objects.Db4o.Query.IConstraint.Or">or()</see>
		/// </returns>
		Db4objects.Db4o.Query.IConstraint And(Db4objects.Db4o.Query.IConstraint with);

		/// <summary>links two Constraints for OR evaluation.</summary>
		/// <remarks>links two Constraints for OR evaluation.</remarks>
		/// <param name="with">
		/// the other
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// </param>
		/// <returns>
		/// a new
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// , that can be used for further calls
		/// to
		/// <see cref="Db4objects.Db4o.Query.IConstraint.And">and()</see>
		/// and
		/// <see cref="Db4objects.Db4o.Query.IConstraint.Or">or()</see>
		/// </returns>
		Db4objects.Db4o.Query.IConstraint Or(Db4objects.Db4o.Query.IConstraint with);

		/// <summary>sets the evaluation mode to <code>==</code>.</summary>
		/// <remarks>sets the evaluation mode to <code>==</code>.</remarks>
		/// <returns>
		/// this
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		Db4objects.Db4o.Query.IConstraint Equal();

		/// <summary>sets the evaluation mode to <code>&gt;</code>.</summary>
		/// <remarks>sets the evaluation mode to <code>&gt;</code>.</remarks>
		/// <returns>
		/// this
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		Db4objects.Db4o.Query.IConstraint Greater();

		/// <summary>sets the evaluation mode to <code>&lt;</code>.</summary>
		/// <remarks>sets the evaluation mode to <code>&lt;</code>.</remarks>
		/// <returns>
		/// this
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		Db4objects.Db4o.Query.IConstraint Smaller();

		/// <summary>sets the evaluation mode to identity comparison.</summary>
		/// <remarks>sets the evaluation mode to identity comparison.</remarks>
		/// <returns>
		/// this
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		Db4objects.Db4o.Query.IConstraint Identity();

		/// <summary>sets the evaluation mode to "like" comparison.</summary>
		/// <remarks>
		/// sets the evaluation mode to "like" comparison.
		/// <br /><br />Constraints are compared to the first characters of a field.<br /><br />
		/// </remarks>
		/// <returns>
		/// this
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		Db4objects.Db4o.Query.IConstraint Like();

		/// <summary>sets the evaluation mode to containment comparison.</summary>
		/// <remarks>sets the evaluation mode to containment comparison.</remarks>
		/// <returns>
		/// this
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		Db4objects.Db4o.Query.IConstraint Contains();

		/// <summary>sets the evaluation mode to string startsWith comparison.</summary>
		/// <remarks>sets the evaluation mode to string startsWith comparison.</remarks>
		/// <param name="caseSensitive">comparison will be case sensitive if true, case insensitive otherwise
		/// 	</param>
		/// <returns>
		/// this
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		Db4objects.Db4o.Query.IConstraint StartsWith(bool caseSensitive);

		/// <summary>sets the evaluation mode to string endsWith comparison.</summary>
		/// <remarks>sets the evaluation mode to string endsWith comparison.</remarks>
		/// <param name="caseSensitive">comparison will be case sensitive if true, case insensitive otherwise
		/// 	</param>
		/// <returns>
		/// this
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		Db4objects.Db4o.Query.IConstraint EndsWith(bool caseSensitive);

		/// <summary>turns on not() comparison.</summary>
		/// <remarks>turns on not() comparison.</remarks>
		/// <returns>
		/// this
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		Db4objects.Db4o.Query.IConstraint Not();

		/// <summary>
		/// returns the Object the query graph was constrained with to
		/// create this
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// .
		/// </summary>
		/// <returns>Object the constraining object.</returns>
		object GetObject();
	}
}
