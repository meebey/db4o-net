namespace Db4objects.Db4o.Inside.Cluster
{
	/// <exclude></exclude>
	public class ClusterConstraints : Db4objects.Db4o.Inside.Cluster.ClusterConstraint
		, Db4objects.Db4o.Query.IConstraints
	{
		public ClusterConstraints(Db4objects.Db4o.Cluster.Cluster cluster, Db4objects.Db4o.Query.IConstraint[]
			 constraints) : base(cluster, constraints)
		{
		}

		public virtual Db4objects.Db4o.Query.IConstraint[] ToArray()
		{
			lock (_cluster)
			{
				Db4objects.Db4o.Foundation.Collection4 all = new Db4objects.Db4o.Foundation.Collection4
					();
				for (int i = 0; i < _constraints.Length; i++)
				{
					Db4objects.Db4o.Inside.Cluster.ClusterConstraint c = (Db4objects.Db4o.Inside.Cluster.ClusterConstraint
						)_constraints[i];
					for (int j = 0; j < c._constraints.Length; j++)
					{
						all.Add(c._constraints[j]);
					}
				}
				Db4objects.Db4o.Query.IConstraint[] res = new Db4objects.Db4o.Query.IConstraint[all
					.Size()];
				all.ToArray(res);
				return res;
			}
		}
	}
}
