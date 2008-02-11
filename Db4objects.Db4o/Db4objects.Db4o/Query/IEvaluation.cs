/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Query
{
	/// <summary>for implementation of callback evaluations.</summary>
	/// <remarks>
	/// for implementation of callback evaluations.
	/// &lt;br&gt;&lt;br&gt;
	/// To constrain a
	/// <see cref="IQuery">IQuery</see>
	/// node with your own callback
	/// &lt;code&gt;Evaluation&lt;/code&gt;, construct an object that implements the
	/// &lt;code&gt;Evaluation&lt;/code&gt; interface and register it by passing it
	/// to
	/// <see cref="IQuery.Constrain">IQuery.Constrain</see>
	/// .
	/// &lt;br&gt;&lt;br&gt;
	/// Evaluations are called as the last step during query execution,
	/// after all other constraints have been applied. Evaluations in higher
	/// level
	/// <see cref="IQuery">IQuery</see>
	/// nodes in the query graph are called first.
	/// &lt;br&gt;&lt;br&gt;Java client/server only:&lt;br&gt;
	/// db4o first attempts to use Java Serialization to allow to pass final
	/// variables to the server. Please make sure that all variables that are
	/// used within the evaluate() method are Serializable. This may include
	/// the class an anonymous Evaluation object is created in. If db4o is
	/// not successful at using Serialization, the Evaluation is transported
	/// to the server in a db4o MemoryFile. In this case final variables can
	/// not be restored.
	/// </remarks>
	public interface IEvaluation
	{
		/// <summary>
		/// callback method during
		/// <see cref="IQuery.Execute">query execution</see>
		/// .
		/// </summary>
		/// <param name="candidate">reference to the candidate persistent object.</param>
		void Evaluate(ICandidate candidate);
	}
}
