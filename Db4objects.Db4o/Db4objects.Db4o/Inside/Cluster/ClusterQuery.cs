namespace Db4objects.Db4o.Inside.Cluster
{
	/// <exclude></exclude>
	public class ClusterQuery : Db4objects.Db4o.Query.IQuery
	{
		private readonly Db4objects.Db4o.Cluster.Cluster _cluster;

		private readonly Db4objects.Db4o.Query.IQuery[] _queries;

		public ClusterQuery(Db4objects.Db4o.Cluster.Cluster cluster, Db4objects.Db4o.Query.IQuery[]
			 queries)
		{
			_cluster = cluster;
			_queries = queries;
		}

		public virtual Db4objects.Db4o.Query.IConstraint Constrain(object constraint)
		{
			lock (_cluster)
			{
				Db4objects.Db4o.Query.IConstraint[] constraints = new Db4objects.Db4o.Query.IConstraint
					[_queries.Length];
				for (int i = 0; i < constraints.Length; i++)
				{
					constraints[i] = _queries[i].Constrain(constraint);
				}
				return new Db4objects.Db4o.Inside.Cluster.ClusterConstraint(_cluster, constraints
					);
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraints Constraints()
		{
			lock (_cluster)
			{
				Db4objects.Db4o.Query.IConstraint[] constraints = new Db4objects.Db4o.Query.IConstraint
					[_queries.Length];
				for (int i = 0; i < constraints.Length; i++)
				{
					constraints[i] = _queries[i].Constraints();
				}
				return new Db4objects.Db4o.Inside.Cluster.ClusterConstraints(_cluster, constraints
					);
			}
		}

		public virtual Db4objects.Db4o.Query.IQuery Descend(string fieldName)
		{
			lock (_cluster)
			{
				Db4objects.Db4o.Query.IQuery[] queries = new Db4objects.Db4o.Query.IQuery[_queries
					.Length];
				for (int i = 0; i < queries.Length; i++)
				{
					queries[i] = _queries[i].Descend(fieldName);
				}
				return new Db4objects.Db4o.Inside.Cluster.ClusterQuery(_cluster, queries);
			}
		}

		public virtual Db4objects.Db4o.IObjectSet Execute()
		{
			lock (_cluster)
			{
				return new Db4objects.Db4o.Inside.Query.ObjectSetFacade(new Db4objects.Db4o.Inside.Cluster.ClusterQueryResult
					(_cluster, _queries));
			}
		}

		public virtual Db4objects.Db4o.Query.IQuery OrderAscending()
		{
			Db4objects.Db4o.Inside.Exceptions4.NotSupported();
			return this;
		}

		public virtual Db4objects.Db4o.Query.IQuery OrderDescending()
		{
			Db4objects.Db4o.Inside.Exceptions4.NotSupported();
			return this;
		}

		public virtual Db4objects.Db4o.Query.IQuery SortBy(Db4objects.Db4o.Query.IQueryComparator
			 comparator)
		{
			Db4objects.Db4o.Inside.Exceptions4.NotSupported();
			return this;
		}
	}
}
