/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Config
{
	/// <summary>interface to configure the querying settings to be used by the query processor.
	/// 	</summary>
	/// <remarks>
	/// interface to configure the querying settings to be used by the query processor.
	/// &lt;br&gt;&lt;br&gt;All settings can be configured after opening an ObjectContainer.
	/// In a Client/Server setup the client-side configuration will be used.
	/// </remarks>
	public interface IQueryConfiguration
	{
		/// <summary>configures the query processor evaluation mode.</summary>
		/// <remarks>
		/// configures the query processor evaluation mode.
		/// &lt;br&gt;&lt;br&gt;The db4o query processor can run in three modes:&lt;br&gt;
		/// - &lt;b&gt;Immediate&lt;/b&gt; mode&lt;br&gt;
		/// - &lt;b&gt;Snapshot&lt;/b&gt; mode&lt;br&gt;
		/// - &lt;b&gt;Lazy&lt;/b&gt; mode&lt;br&gt;&lt;br&gt;
		/// In &lt;b&gt;Immediate&lt;/b&gt; mode, a query will be fully evaluated when
		/// <see cref="IQuery.Execute">IQuery.Execute</see>
		/// 
		/// is called. The complete
		/// <see cref="IObjectSet">IObjectSet</see>
		/// of all matching IDs is
		/// generated immediately.&lt;br&gt;&lt;br&gt;
		/// In &lt;b&gt;Snapshot&lt;/b&gt; mode, the
		/// <see cref="IQuery.Execute">IQuery.Execute</see>
		/// call will trigger all index
		/// processing immediately. A snapshot of the current state of all relevant indexes
		/// is taken for further processing by the SODA query processor. All non-indexed
		/// constraints and all evaluations will be run when the user application iterates
		/// through the resulting
		/// <see cref="IObjectSet">IObjectSet</see>
		/// .&lt;br&gt;&lt;br&gt;
		/// In &lt;b&gt;Lazy&lt;/b&gt; mode, the
		/// <see cref="IQuery.Execute">IQuery.Execute</see>
		/// call will only create an Iterator
		/// against the best index found. Further query processing (including all index
		/// processing) will happen when the user application iterates through the resulting
		/// <see cref="IObjectSet">IObjectSet</see>
		/// .&lt;br&gt;&lt;br&gt;
		/// Advantages and disadvantages of the individual modes:&lt;br&gt;&lt;br&gt;
		/// &lt;b&gt;Immediate&lt;/b&gt; mode&lt;br&gt;
		/// &lt;b&gt;+&lt;/b&gt; If the query is intended to iterate through the entire resulting
		/// <see cref="IObjectSet">IObjectSet</see>
		/// ,
		/// this mode will be slightly faster than the others.&lt;br&gt;
		/// &lt;b&gt;+&lt;/b&gt; The query will process without intermediate side effects from changed
		/// objects (by the caller or by other transactions).&lt;br&gt;
		/// &lt;b&gt;-&lt;/b&gt; Query processing can block the server for a long time.&lt;br&gt;
		/// &lt;b&gt;-&lt;/b&gt; In comparison to the other modes it will take longest until the first results
		/// are returned.&lt;br&gt;
		/// &lt;b&gt;-&lt;/b&gt; The ObjectSet will require a considerate amount of memory to hold the IDs of
		/// all found objects.&lt;br&gt;&lt;br&gt;
		/// &lt;b&gt;Snapshot&lt;/b&gt; mode&lt;br&gt;
		/// &lt;b&gt;+&lt;/b&gt; Index processing will happen without possible side effects from changes made
		/// by the caller or by other transaction.&lt;br&gt;
		/// &lt;b&gt;+&lt;/b&gt; Since index processing is fast, a server will not be blocked for a long time.&lt;br&gt;
		/// &lt;b&gt;-&lt;/b&gt; The entire candidate index will be loaded into memory. It will stay there
		/// until the query ObjectSet is garbage collected. In a C/S setup, the memory will
		/// be used on the server side.&lt;br&gt;&lt;br&gt;
		/// &lt;b&gt;Lazy&lt;/b&gt; mode&lt;br&gt;
		/// &lt;b&gt;+&lt;/b&gt; The call to
		/// <see cref="IQuery.Execute">IQuery.Execute</see>
		/// will return very fast. First results can be
		/// made available to the application before the query is fully processed.&lt;br&gt;
		/// &lt;b&gt;+&lt;/b&gt; A query will consume hardly any memory at all because no intermediate ID
		/// representation is ever created.&lt;br&gt;
		/// &lt;b&gt;-&lt;/b&gt; Lazy queries check candidates when iterating through the resulting
		/// <see cref="IObjectSet">IObjectSet</see>
		/// .
		/// In doing so the query processor takes changes into account that may have happened
		/// since the Query#execute()call: committed changes from other transactions, &lt;b&gt;and
		/// uncommitted changes from the calling transaction&lt;/b&gt;. There is a wide range
		/// of possible side effects. The underlying index may have changed. Objects themselves
		/// may have changed in the meanwhile. There even is the chance of creating an endless
		/// loop, if the caller of the iterates through the
		/// <see cref="IObjectSet">IObjectSet</see>
		/// and changes each
		/// object in a way that it is placed at the end of the index: The same objects can be
		/// revisited over and over. &lt;b&gt;In lazy mode it can make sense to work in a way one would
		/// work with collections to avoid concurrent modification exceptions.&lt;/b&gt; For instance one
		/// could iterate through the
		/// <see cref="IObjectSet">IObjectSet</see>
		/// first and store all objects to a temporary
		/// other collection representation before changing objects and storing them back to db4o.&lt;br&gt;&lt;br&gt;
		/// Note: Some method calls against a lazy
		/// <see cref="IObjectSet">IObjectSet</see>
		/// will require the query
		/// processor to create a snapshot or to evaluate the query fully. An example of such
		/// a call is
		/// <see cref="IObjectSet.Size">IObjectSet.Size</see>
		/// .
		/// &lt;br&gt;&lt;br&gt;
		/// The default query evaluation mode is &lt;b&gt;Immediate&lt;/b&gt; mode.
		/// &lt;br&gt;&lt;br&gt;
		/// Recommendations:&lt;br&gt;
		/// - &lt;b&gt;Lazy&lt;/b&gt; mode can be an excellent choice for single transaction read use,
		/// to keep memory consumption as low as possible.&lt;br&gt;
		/// - Client/Server applications with the risk of concurrent modifications should prefer
		/// &lt;b&gt;Snapshot&lt;/b&gt; mode to avoid side effects from other transactions.
		/// &lt;br&gt;&lt;br&gt;
		/// To change the evaluationMode, pass any of the three static
		/// <see cref="QueryEvaluationMode">QueryEvaluationMode</see>
		/// constants from the
		/// <see cref="QueryEvaluationMode">QueryEvaluationMode</see>
		/// class to this method:&lt;br&gt;
		/// -
		/// <see cref="QueryEvaluationMode.IMMEDIATE">QueryEvaluationMode.IMMEDIATE</see>
		/// &lt;br&gt;
		/// -
		/// <see cref="QueryEvaluationMode.SNAPSHOT">QueryEvaluationMode.SNAPSHOT</see>
		/// &lt;br&gt;
		/// -
		/// <see cref="QueryEvaluationMode.LAZY">QueryEvaluationMode.LAZY</see>
		/// &lt;br&gt;&lt;br&gt;
		/// This setting must be issued from the client side.
		/// </remarks>
		void EvaluationMode(QueryEvaluationMode mode);
	}
}
