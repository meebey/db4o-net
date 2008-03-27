/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Query
{
	/// <summary>
	/// candidate for
	/// <see cref="IEvaluation">IEvaluation</see>
	/// callbacks.
	/// <br /><br />
	/// During
	/// <see cref="IQuery.Execute">query execution</see>
	/// all registered
	/// <see cref="IEvaluation">IEvaluation</see>
	/// callback
	/// handlers are called with
	/// <see cref="ICandidate">ICandidate</see>
	/// proxies that represent the persistent objects that
	/// meet all other
	/// <see cref="IQuery">IQuery</see>
	/// criteria.
	/// <br /><br />
	/// A
	/// <see cref="ICandidate">ICandidate</see>
	/// provides access to the persistent object it
	/// represents and allows to specify, whether it is to be included in the
	/// <see cref="IObjectSet">IObjectSet</see>
	/// resultset.
	/// </summary>
	public interface ICandidate
	{
		/// <summary>
		/// returns the persistent object that is represented by this query
		/// <see cref="ICandidate">ICandidate</see>
		/// .
		/// </summary>
		/// <returns>Object the persistent object.</returns>
		object GetObject();

		/// <summary>
		/// specify whether the Candidate is to be included in the
		/// <see cref="IObjectSet">IObjectSet</see>
		/// resultset.
		/// <br /><br />
		/// This method may be called multiple times. The last call prevails.
		/// </summary>
		/// <param name="flag">inclusion.</param>
		void Include(bool flag);

		/// <summary>
		/// returns the
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// the Candidate object is stored in.
		/// </summary>
		/// <returns>
		/// the
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// </returns>
		IObjectContainer ObjectContainer();
	}
}
