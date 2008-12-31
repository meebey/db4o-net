/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Query
{
	/// <summary>handle to a node in a S.O.D.A.</summary>
	/// <remarks>
	/// handle to a node in a S.O.D.A. query graph.
	/// <br /><br />
	/// A node in the query graph can represent multiple
	/// classes, one class or an attribute of a class.<br /><br />The graph
	/// is automatically extended with attributes of added constraints
	/// (see
	/// <see cref="Db4objects.Db4o.Query.IQuery.Constrain">Db4objects.Db4o.Query.IQuery.Constrain
	/// 	</see>
	/// ) and upon calls to
	/// <see cref="Db4objects.Db4o.Query.IQuery.Descend">Db4objects.Db4o.Query.IQuery.Descend
	/// 	</see>
	/// that request nodes that do not yet exist.
	/// <br /><br />
	/// References to joined nodes in the query graph can be obtained
	/// by "walking" along the nodes of the graph with the method
	/// <see cref="Db4objects.Db4o.Query.IQuery.Descend">Db4objects.Db4o.Query.IQuery.Descend
	/// 	</see>
	/// .
	/// <br /><br />
	/// <see cref="Db4objects.Db4o.Query.IQuery.Execute">Db4objects.Db4o.Query.IQuery.Execute
	/// 	</see>
	/// evaluates the entire graph against all persistent objects.
	/// <br /><br />
	/// <see cref="Db4objects.Db4o.Query.IQuery.Execute">Db4objects.Db4o.Query.IQuery.Execute
	/// 	</see>
	/// can be called from any
	/// <see cref="Db4objects.Db4o.Query.IQuery">Db4objects.Db4o.Query.IQuery</see>
	/// node
	/// of the graph. It will return an
	/// <see cref="Db4objects.Db4o.IObjectSet">Db4objects.Db4o.IObjectSet</see>
	/// filled with
	/// objects of the class/classes that the node, it was called from,
	/// represents.<br /><br />
	/// <b>Note:<br />
	/// <see cref="Db4objects.Db4o.Query.Predicate">Native queries</see>
	/// are the recommended main query
	/// interface of db4o.</b>
	/// </remarks>
	public interface IQuery
	{
		/// <summary>adds a constraint to this node.</summary>
		/// <remarks>
		/// adds a constraint to this node.
		/// <br /><br />
		/// If the constraint contains attributes that are not yet
		/// present in the query graph, the query graph is extended
		/// accordingly.
		/// <br /><br />
		/// Special behaviour for:
		/// <ul>
		/// <li> class
		/// <see cref="System.Type">System.Type</see>
		/// : confine the result to objects of one
		/// class or to objects implementing an interface.</li>
		/// <li> interface
		/// <see cref="Db4objects.Db4o.Query.IEvaluation">Db4objects.Db4o.Query.IEvaluation</see>
		/// : run
		/// evaluation callbacks against all candidates.</li>
		/// </ul>
		/// </remarks>
		/// <param name="constraint">the constraint to be added to this Query.</param>
		/// <returns>
		/// 
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// a new
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// for this
		/// query node or <code>null</code> for objects implementing the
		/// <see cref="Db4objects.Db4o.Query.IEvaluation">Db4objects.Db4o.Query.IEvaluation</see>
		/// interface.
		/// </returns>
		IConstraint Constrain(object constraint);

		/// <summary>
		/// returns a
		/// <see cref="Db4objects.Db4o.Query.IConstraints">Db4objects.Db4o.Query.IConstraints
		/// 	</see>
		/// object that holds an array of all constraints on this node.
		/// </summary>
		/// <returns>
		/// 
		/// <see cref="Db4objects.Db4o.Query.IConstraints">Db4objects.Db4o.Query.IConstraints
		/// 	</see>
		/// on this query node.
		/// </returns>
		IConstraints Constraints();

		/// <summary>returns a reference to a descendant node in the query graph.</summary>
		/// <remarks>
		/// returns a reference to a descendant node in the query graph.
		/// <br /><br />If the node does not exist, it will be created.
		/// <br /><br />
		/// All classes represented in the query node are tested, whether
		/// they contain a field with the specified field name. The
		/// descendant Query node will be created from all possible candidate
		/// classes.
		/// </remarks>
		/// <param name="fieldName">path to the descendant.</param>
		/// <returns>
		/// descendant
		/// <see cref="Db4objects.Db4o.Query.IQuery">Db4objects.Db4o.Query.IQuery</see>
		/// node
		/// </returns>
		IQuery Descend(string fieldName);

		/// <summary>
		/// executes the
		/// <see cref="Db4objects.Db4o.Query.IQuery">Db4objects.Db4o.Query.IQuery</see>
		/// .
		/// </summary>
		/// <returns>
		/// 
		/// <see cref="Db4objects.Db4o.IObjectSet">Db4objects.Db4o.IObjectSet</see>
		/// - the result of the
		/// <see cref="Db4objects.Db4o.Query.IQuery">Db4objects.Db4o.Query.IQuery</see>
		/// .
		/// </returns>
		IObjectSet Execute();

		/// <summary>
		/// adds an ascending ordering criteria to this node of
		/// the query graph.
		/// </summary>
		/// <remarks>
		/// adds an ascending ordering criteria to this node of
		/// the query graph.
		/// <p>
		/// If multiple ordering criteria are applied, the chronological
		/// order of method calls is relevant: criteria created by 'earlier' calls are
		/// considered more significant, i.e. 'later' criteria only have an effect
		/// for elements that are considered equal by all 'earlier' criteria.
		/// <p>
		/// As an example, consider a type with two int fields, and an instance set
		/// {(a:1,b:3),(a:2,b:2),(a:1,b:2),(a:2,b:3)}. The call sequence [orderAscending(a),
		/// orderDescending(b)] will result in [(<b>a:1</b>,b:3),(<b>a:1</b>,b:2),(<b>a:2</b>,b:3),(<b>a:2</b>,b:2)].
		/// </remarks>
		/// <returns>
		/// this
		/// <see cref="Db4objects.Db4o.Query.IQuery">Db4objects.Db4o.Query.IQuery</see>
		/// object to allow the chaining of method calls.
		/// </returns>
		IQuery OrderAscending();

		/// <summary>
		/// adds a descending order criteria to this node of
		/// the query graph.
		/// </summary>
		/// <remarks>
		/// adds a descending order criteria to this node of
		/// the query graph.
		/// <br /><br />
		/// For semantics of multiple calls setting ordering criteria, see
		/// <see cref="Db4objects.Db4o.Query.IQuery.OrderAscending">Db4objects.Db4o.Query.IQuery.OrderAscending
		/// 	</see>
		/// .
		/// </remarks>
		/// <returns>
		/// this
		/// <see cref="Db4objects.Db4o.Query.IQuery">Db4objects.Db4o.Query.IQuery</see>
		/// object to allow the chaining of method calls.
		/// </returns>
		IQuery OrderDescending();

		/// <summary>Sort the resulting ObjectSet by the given comparator.</summary>
		/// <remarks>Sort the resulting ObjectSet by the given comparator.</remarks>
		/// <param name="comparator">The comparator to apply.</param>
		/// <returns>
		/// this
		/// <see cref="Db4objects.Db4o.Query.IQuery">Db4objects.Db4o.Query.IQuery</see>
		/// object to allow the chaining of method calls.
		/// </returns>
		IQuery SortBy(IQueryComparator comparator);
		//    /**
		//     * defines a Query node to be represented as a column in the array
		//     * returned in every element of the ObjectSet upon query execution. 
		//     * @param node the Query node to be represented
		//     * @param column the column in the result array 
		//     */
		//    public void result(Query node, int column);
		//
	}
}
