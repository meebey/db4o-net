/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Query
{
	/// <summary>handle to a node in a S.O.D.A.</summary>
	/// <remarks>
	/// handle to a node in a S.O.D.A. query graph.
	/// &lt;br&gt;&lt;br&gt;
	/// A node in the query graph can represent multiple
	/// classes, one class or an attribute of a class.&lt;br&gt;&lt;br&gt;The graph
	/// is automatically extended with attributes of added constraints
	/// (see
	/// <see cref="IQuery.Constrain">IQuery.Constrain</see>
	/// ) and upon calls to
	/// <see cref="IQuery.Descend">IQuery.Descend</see>
	/// that request nodes that do not yet exist.
	/// &lt;br&gt;&lt;br&gt;
	/// References to joined nodes in the query graph can be obtained
	/// by "walking" along the nodes of the graph with the method
	/// <see cref="IQuery.Descend">IQuery.Descend</see>
	/// .
	/// &lt;br&gt;&lt;br&gt;
	/// <see cref="IQuery.Execute">IQuery.Execute</see>
	/// evaluates the entire graph against all persistent objects.
	/// &lt;br&gt;&lt;br&gt;
	/// <see cref="IQuery.Execute">IQuery.Execute</see>
	/// can be called from any
	/// <see cref="IQuery">IQuery</see>
	/// node
	/// of the graph. It will return an
	/// <see cref="IObjectSet">IObjectSet</see>
	/// filled with
	/// objects of the class/classes that the node, it was called from,
	/// represents.&lt;br&gt;&lt;br&gt;
	/// &lt;b&gt;Note:&lt;br&gt;
	/// <see cref="Predicate">Native queries</see>
	/// are the recommended main query
	/// interface of db4o.&lt;/b&gt;
	/// </remarks>
	public interface IQuery
	{
		/// <summary>adds a constraint to this node.</summary>
		/// <remarks>
		/// adds a constraint to this node.
		/// &lt;br&gt;&lt;br&gt;
		/// If the constraint contains attributes that are not yet
		/// present in the query graph, the query graph is extended
		/// accordingly.
		/// &lt;br&gt;&lt;br&gt;
		/// Special behaviour for:
		/// &lt;ul&gt;
		/// &lt;li&gt; class
		/// <see cref="Type">Type</see>
		/// : confine the result to objects of one
		/// class or to objects implementing an interface.&lt;/li&gt;
		/// &lt;li&gt; interface
		/// <see cref="IEvaluation">IEvaluation</see>
		/// : run
		/// evaluation callbacks against all candidates.&lt;/li&gt;
		/// &lt;/ul&gt;
		/// </remarks>
		/// <param name="constraint">the constraint to be added to this Query.</param>
		/// <returns>
		/// 
		/// <see cref="IConstraint">IConstraint</see>
		/// a new
		/// <see cref="IConstraint">IConstraint</see>
		/// for this
		/// query node or &lt;code&gt;null&lt;/code&gt; for objects implementing the
		/// <see cref="IEvaluation">IEvaluation</see>
		/// interface.
		/// </returns>
		IConstraint Constrain(object constraint);

		/// <summary>
		/// returns a
		/// <see cref="IConstraints">IConstraints</see>
		/// object that holds an array of all constraints on this node.
		/// </summary>
		/// <returns>
		/// 
		/// <see cref="IConstraints">IConstraints</see>
		/// on this query node.
		/// </returns>
		IConstraints Constraints();

		/// <summary>returns a reference to a descendant node in the query graph.</summary>
		/// <remarks>
		/// returns a reference to a descendant node in the query graph.
		/// &lt;br&gt;&lt;br&gt;If the node does not exist, it will be created.
		/// &lt;br&gt;&lt;br&gt;
		/// All classes represented in the query node are tested, whether
		/// they contain a field with the specified field name. The
		/// descendant Query node will be created from all possible candidate
		/// classes.
		/// </remarks>
		/// <param name="fieldName">path to the descendant.</param>
		/// <returns>
		/// descendant
		/// <see cref="IQuery">IQuery</see>
		/// node
		/// </returns>
		IQuery Descend(string fieldName);

		/// <summary>
		/// executes the
		/// <see cref="IQuery">IQuery</see>
		/// .
		/// </summary>
		/// <returns>
		/// 
		/// <see cref="IObjectSet">IObjectSet</see>
		/// - the result of the
		/// <see cref="IQuery">IQuery</see>
		/// .
		/// </returns>
		IObjectSet Execute();

		/// <summary>
		/// adds an ascending ordering criteria to this node of the
		/// query graph.
		/// </summary>
		/// <remarks>
		/// adds an ascending ordering criteria to this node of the
		/// query graph.
		/// &lt;p&gt;
		/// If multiple ordering criteria are applied, the chronological
		/// order of method calls is relevant: criteria created by 'earlier' calls are
		/// considered more significant, i.e. 'later' criteria only have an effect
		/// for elements that are considered equal by all 'earlier' criteria.
		/// &lt;p&gt;
		/// As an example, consider a type with two int fields, and an instance set
		/// {(a:1,b:3),(a:2,b:2),(a:1,b:2),(a:2,b:3)}. The call sequence [orderAscending(a),
		/// orderDescending(b)] will result in [(&lt;b&gt;a:1&lt;/b&gt;,b:3),(&lt;b&gt;a:1&lt;/b&gt;,b:2),(&lt;b&gt;a:2&lt;/b&gt;,b:3),(&lt;b&gt;a:2&lt;/b&gt;,b:2)].
		/// </remarks>
		/// <returns>
		/// this
		/// <see cref="IQuery">IQuery</see>
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
		/// &lt;br&gt;&lt;br&gt;
		/// For semantics of multiple calls setting ordering criteria, see
		/// <see cref="IQuery.OrderAscending">IQuery.OrderAscending</see>
		/// .
		/// </remarks>
		/// <returns>
		/// this
		/// <see cref="IQuery">IQuery</see>
		/// object to allow the chaining of method calls.
		/// </returns>
		IQuery OrderDescending();

		/// <summary>Sort the resulting ObjectSet by the given comparator.</summary>
		/// <remarks>Sort the resulting ObjectSet by the given comparator.</remarks>
		/// <param name="comparator">The comparator to apply.</param>
		/// <returns>
		/// this
		/// <see cref="IQuery">IQuery</see>
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
