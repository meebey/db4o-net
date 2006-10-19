namespace Db4objects.Db4o.Inside.Cluster
{
	/// <exclude></exclude>
	public class ClusterConstraint : Db4objects.Db4o.Query.IConstraint
	{
		internal readonly Db4objects.Db4o.Cluster.Cluster _cluster;

		internal readonly Db4objects.Db4o.Query.IConstraint[] _constraints;

		public ClusterConstraint(Db4objects.Db4o.Cluster.Cluster cluster, Db4objects.Db4o.Query.IConstraint[]
			 constraints)
		{
			_cluster = cluster;
			_constraints = constraints;
		}

		private Db4objects.Db4o.Inside.Cluster.ClusterConstraint Compatible(Db4objects.Db4o.Query.IConstraint
			 with)
		{
			if (!(with is Db4objects.Db4o.Inside.Cluster.ClusterConstraint))
			{
				throw new System.ArgumentException();
			}
			Db4objects.Db4o.Inside.Cluster.ClusterConstraint other = (Db4objects.Db4o.Inside.Cluster.ClusterConstraint
				)with;
			if (other._constraints.Length != _constraints.Length)
			{
				throw new System.ArgumentException();
			}
			return other;
		}

		public virtual Db4objects.Db4o.Query.IConstraint And(Db4objects.Db4o.Query.IConstraint
			 with)
		{
			return Join(with, true);
		}

		public virtual Db4objects.Db4o.Query.IConstraint Or(Db4objects.Db4o.Query.IConstraint
			 with)
		{
			return Join(with, false);
		}

		private Db4objects.Db4o.Query.IConstraint Join(Db4objects.Db4o.Query.IConstraint 
			with, bool isAnd)
		{
			lock (_cluster)
			{
				Db4objects.Db4o.Inside.Cluster.ClusterConstraint other = Compatible(with);
				Db4objects.Db4o.Query.IConstraint[] newConstraints = new Db4objects.Db4o.Query.IConstraint
					[_constraints.Length];
				for (int i = 0; i < _constraints.Length; i++)
				{
					newConstraints[i] = isAnd ? _constraints[i].And(other._constraints[i]) : _constraints
						[i].Or(other._constraints[i]);
				}
				return new Db4objects.Db4o.Inside.Cluster.ClusterConstraint(_cluster, newConstraints
					);
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraint Equal()
		{
			lock (_cluster)
			{
				for (int i = 0; i < _constraints.Length; i++)
				{
					_constraints[i].Equal();
				}
				return this;
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraint Greater()
		{
			lock (_cluster)
			{
				for (int i = 0; i < _constraints.Length; i++)
				{
					_constraints[i].Greater();
				}
				return this;
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraint Smaller()
		{
			lock (_cluster)
			{
				for (int i = 0; i < _constraints.Length; i++)
				{
					_constraints[i].Smaller();
				}
				return this;
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraint Identity()
		{
			lock (_cluster)
			{
				for (int i = 0; i < _constraints.Length; i++)
				{
					_constraints[i].Identity();
				}
				return this;
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraint Like()
		{
			lock (_cluster)
			{
				for (int i = 0; i < _constraints.Length; i++)
				{
					_constraints[i].Like();
				}
				return this;
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraint StartsWith(bool caseSensitive)
		{
			lock (_cluster)
			{
				for (int i = 0; i < _constraints.Length; i++)
				{
					_constraints[i].StartsWith(caseSensitive);
				}
				return this;
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraint EndsWith(bool caseSensitive)
		{
			lock (_cluster)
			{
				for (int i = 0; i < _constraints.Length; i++)
				{
					_constraints[i].EndsWith(caseSensitive);
				}
				return this;
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraint Contains()
		{
			lock (_cluster)
			{
				for (int i = 0; i < _constraints.Length; i++)
				{
					_constraints[i].Contains();
				}
				return this;
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraint Not()
		{
			lock (_cluster)
			{
				for (int i = 0; i < _constraints.Length; i++)
				{
					_constraints[i].Not();
				}
				return this;
			}
		}

		public virtual object GetObject()
		{
			Db4objects.Db4o.Inside.Exceptions4.NotSupported();
			return null;
		}
	}
}
