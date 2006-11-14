namespace Db4objects.Db4o.Query
{
	/// <summary>
	/// candidate for
	/// <see cref="Db4objects.Db4o.Query.IEvaluation">Db4objects.Db4o.Query.IEvaluation</see>
	/// callbacks.
	/// <br /><br />
	/// During
	/// <see cref="Db4objects.Db4o.Query.IQuery.Execute">query execution</see>
	/// all registered
	/// <see cref="Db4objects.Db4o.Query.IEvaluation">Db4objects.Db4o.Query.IEvaluation</see>
	/// callback
	/// handlers are called with
	/// <see cref="Db4objects.Db4o.Query.ICandidate">Db4objects.Db4o.Query.ICandidate</see>
	/// proxies that represent the persistent objects that
	/// meet all other
	/// <see cref="Db4objects.Db4o.Query.IQuery">Db4objects.Db4o.Query.IQuery</see>
	/// criteria.
	/// <br /><br />
	/// A
	/// <see cref="Db4objects.Db4o.Query.ICandidate">Db4objects.Db4o.Query.ICandidate</see>
	/// provides access to the persistent object it
	/// represents and allows to specify, whether it is to be included in the
	/// <see cref="Db4objects.Db4o.IObjectSet">Db4objects.Db4o.IObjectSet</see>
	/// resultset.
	/// </summary>
	public interface ICandidate
	{
		/// <summary>
		/// returns the persistent object that is represented by this query
		/// <see cref="Db4objects.Db4o.Query.ICandidate">Db4objects.Db4o.Query.ICandidate</see>
		/// .
		/// </summary>
		/// <returns>Object the persistent object.</returns>
		object GetObject();

		/// <summary>
		/// specify whether the Candidate is to be included in the
		/// <see cref="Db4objects.Db4o.IObjectSet">Db4objects.Db4o.IObjectSet</see>
		/// resultset.
		/// <br /><br />
		/// This method may be called multiple times. The last call prevails.
		/// </summary>
		/// <param name="flag">inclusion.</param>
		void Include(bool flag);

		/// <summary>
		/// returns the
		/// <see cref="Db4objects.Db4o.IObjectContainer">Db4objects.Db4o.IObjectContainer</see>
		/// the Candidate object is stored in.
		/// </summary>
		/// <returns>
		/// the
		/// <see cref="Db4objects.Db4o.IObjectContainer">Db4objects.Db4o.IObjectContainer</see>
		/// </returns>
		Db4objects.Db4o.IObjectContainer ObjectContainer();
	}
}
