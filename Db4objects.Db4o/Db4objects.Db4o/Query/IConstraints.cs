/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Query
{
	/// <summary>
	/// set of
	/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
	/// objects.
	/// <br /><br />This extension of the
	/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
	/// interface allows
	/// setting the evaluation mode of all contained
	/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
	/// objects with single calls.
	/// <br /><br />
	/// See also
	/// <see cref="Db4objects.Db4o.Query.IQuery.Constraints">Db4objects.Db4o.Query.IQuery.Constraints
	/// 	</see>
	/// .
	/// </summary>
	public interface IConstraints : IConstraint
	{
		/// <summary>
		/// returns an array of the contained
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// objects.
		/// </summary>
		/// <returns>
		/// an array of the contained
		/// <see cref="Db4objects.Db4o.Query.IConstraint">Db4objects.Db4o.Query.IConstraint</see>
		/// objects.
		/// </returns>
		IConstraint[] ToArray();
	}
}
