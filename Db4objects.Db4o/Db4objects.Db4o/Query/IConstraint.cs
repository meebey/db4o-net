/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Query
{
	/// <summary>
	/// constraint to limit the objects returned upon
	/// <see cref="IQuery.Execute">query execution</see>
	/// .
	/// &lt;br&gt;&lt;br&gt;
	/// Constraints are constructed by calling
	/// <see cref="IQuery.Constrain">IQuery.Constrain</see>
	/// .
	/// &lt;br&gt;&lt;br&gt;
	/// Constraints can be joined with the methods
	/// <see cref="IConstraint.And">IConstraint.And</see>
	/// and
	/// <see cref="IConstraint.Or">IConstraint.Or</see>
	/// .
	/// &lt;br&gt;&lt;br&gt;
	/// The methods to modify the constraint evaluation algorithm may
	/// be merged, to construct combined evaluation rules.
	/// Examples:
	/// &lt;ul&gt;
	/// &lt;li&gt; &lt;code&gt;Constraint#smaller().equal()&lt;/code&gt; for "smaller or equal" &lt;/li&gt;
	/// &lt;li&gt; &lt;code&gt;Constraint#not().like()&lt;/code&gt; for "not like" &lt;/li&gt;
	/// &lt;li&gt; &lt;code&gt;Constraint#not().greater().equal()&lt;/code&gt; for "not greater or equal" &lt;/li&gt;
	/// &lt;/ul&gt;
	/// </summary>
	public interface IConstraint
	{
		/// <summary>links two Constraints for AND evaluation.</summary>
		/// <remarks>
		/// links two Constraints for AND evaluation.
		/// For example:&lt;br&gt;
		/// &lt;code&gt;query.constrain(Pilot.class);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.descend("points").constrain(101).smaller().and(query.descend("name").constrain("Test Pilot0"));	&lt;/code&gt;&lt;br&gt;
		/// will retrieve all pilots with points less than 101 and name as "Test Pilot0"&lt;br&gt;
		/// </remarks>
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
		/// <remarks>
		/// links two Constraints for OR evaluation.
		/// For example:&lt;br&gt;&lt;br&gt;
		/// &lt;code&gt;query.constrain(Pilot.class);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.descend("points").constrain(101).greater().or(query.descend("name").constrain("Test Pilot0"));&lt;/code&gt;&lt;br&gt;
		/// will retrieve all pilots with points more than 101 or pilots with the name "Test Pilot0"&lt;br&gt;
		/// </remarks>
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

		/// <summary>
		/// Used in conjunction with
		/// <see cref="IConstraint.Smaller">IConstraint.Smaller</see>
		/// or
		/// <see cref="IConstraint.Greater">IConstraint.Greater</see>
		/// to create constraints
		/// like "smaller or equal", "greater or equal".
		/// For example:&lt;br&gt;
		/// &lt;code&gt;query.constrain(Pilot.class);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.descend("points").constrain(101).smaller().equal();&lt;/code&gt;&lt;br&gt;
		/// will return all pilots with points &lt;= 101.&lt;br&gt;
		/// </summary>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Equal();

		/// <summary>sets the evaluation mode to &lt;code&gt;&gt;&lt;/code&gt;.</summary>
		/// <remarks>
		/// sets the evaluation mode to &lt;code&gt;&gt;&lt;/code&gt;.
		/// For example:&lt;br&gt;
		/// &lt;code&gt;query.constrain(Pilot.class);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.descend("points").constrain(101).greater()&lt;/code&gt;&lt;br&gt;
		/// will return all pilots with points &gt; 101.&lt;br&gt;
		/// </remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Greater();

		/// <summary>sets the evaluation mode to &lt;code&gt;&lt;&lt;/code&gt;.</summary>
		/// <remarks>
		/// sets the evaluation mode to &lt;code&gt;&lt;&lt;/code&gt;.
		/// For example:&lt;br&gt;
		/// &lt;code&gt;query.constrain(Pilot.class);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.descend("points").constrain(101).smaller()&lt;/code&gt;&lt;br&gt;
		/// will return all pilots with points &lt; 101.&lt;br&gt;
		/// </remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Smaller();

		/// <summary>sets the evaluation mode to identity comparison.</summary>
		/// <remarks>
		/// sets the evaluation mode to identity comparison. In this case only
		/// objects having the same database identity will be included in the result set.
		/// For example:&lt;br&gt;
		/// &lt;code&gt;Pilot pilot = new Pilot("Test Pilot1", 100);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;Car car = new Car("BMW", pilot);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;container.set(car);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;// Change the name, the pilot instance stays the same&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;pilot.setName("Test Pilot2");&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;// create a new car&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;car = new Car("Ferrari", pilot);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;container.set(car);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;Query query = container.query();&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.constrain(Car.class);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;// All cars having pilot with the same database identity&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;// will be retrieved. As we only created Pilot object once&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;// it should mean all car objects&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.descend("_pilot").constrain(pilot).identity();&lt;/code&gt;&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Identity();

		/// <summary>set the evaluation mode to object comparison (query by example).</summary>
		/// <remarks>set the evaluation mode to object comparison (query by example).</remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint ByExample();

		/// <summary>sets the evaluation mode to "like" comparison.</summary>
		/// <remarks>
		/// sets the evaluation mode to "like" comparison. This mode will include
		/// all objects having the constrain expression somewhere inside the string field.
		/// For example:&lt;br&gt;
		/// &lt;code&gt;Pilot pilot = new Pilot("Test Pilot1", 100);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;container.set(pilot);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt; ...&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.constrain(Pilot.class);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;// All pilots with the name containing "est" will be retrieved&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.descend("name").constrain("est").like();&lt;/code&gt;&lt;br&gt;
		/// </remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Like();

		/// <summary>sets the evaluation mode to containment comparison.</summary>
		/// <remarks>
		/// sets the evaluation mode to containment comparison.
		/// For example:&lt;br&gt;
		/// &lt;code&gt;Pilot pilot1 = new Pilot("Test 1", 1);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;list.add(pilot1);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;Pilot pilot2 = new Pilot("Test 2", 2);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;list.add(pilot2);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;Team team = new Team("Ferrari", list);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;container.set(team);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;Query query = container.query();&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.constrain(Team.class);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.descend("pilots").constrain(pilot2).contains();&lt;/code&gt;&lt;br&gt;
		/// will return the Team object as it contains pilot2.&lt;br&gt;
		/// If applied to a String object, this constrain will behave as
		/// <see cref="IConstraint.Like">IConstraint.Like</see>
		/// .
		/// </remarks>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint Contains();

		/// <summary>sets the evaluation mode to string startsWith comparison.</summary>
		/// <remarks>
		/// sets the evaluation mode to string startsWith comparison.
		/// For example:&lt;br&gt;
		/// &lt;code&gt;Pilot pilot = new Pilot("Test Pilot0", 100);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;container.set(pilot);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt; ...&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.constrain(Pilot.class);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.descend("name").constrain("Test").startsWith(true);&lt;/code&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="caseSensitive">comparison will be case sensitive if true, case insensitive otherwise
		/// 	</param>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint StartsWith(bool caseSensitive);

		/// <summary>sets the evaluation mode to string endsWith comparison.</summary>
		/// <remarks>
		/// sets the evaluation mode to string endsWith comparison.
		/// For example:&lt;br&gt;
		/// &lt;code&gt;Pilot pilot = new Pilot("Test Pilot0", 100);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;container.set(pilot);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt; ...&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.constrain(Pilot.class);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.descend("name").constrain("T0").endsWith(false);&lt;/code&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="caseSensitive">comparison will be case sensitive if true, case insensitive otherwise
		/// 	</param>
		/// <returns>
		/// this
		/// <see cref="IConstraint">IConstraint</see>
		/// to allow the chaining of method calls.
		/// </returns>
		IConstraint EndsWith(bool caseSensitive);

		/// <summary>turns on not() comparison.</summary>
		/// <remarks>
		/// turns on not() comparison. All objects not fullfilling the constrain condition will be returned.
		/// For example:&lt;br&gt;
		/// &lt;code&gt;Pilot pilot = new Pilot("Test Pilot1", 100);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;container.set(pilot);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt; ...&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.constrain(Pilot.class);&lt;/code&gt;&lt;br&gt;
		/// &lt;code&gt;query.descend("name").constrain("t0").endsWith(true).not();&lt;/code&gt;&lt;br&gt;
		/// </remarks>
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
