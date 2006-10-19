namespace Db4objects.Db4o.Cluster
{
	/// <summary>allows running Queries against multiple ObjectContainers.</summary>
	/// <remarks>allows running Queries against multiple ObjectContainers.</remarks>
	/// <exclude></exclude>
	public class Cluster
	{
		public readonly Db4objects.Db4o.IObjectContainer[] _objectContainers;

		public Cluster(Db4objects.Db4o.IObjectContainer[] objectContainers)
		{
			if (objectContainers == null)
			{
				throw new System.ArgumentNullException();
			}
			if (objectContainers.Length < 1)
			{
				throw new System.ArgumentException();
			}
			for (int i = 0; i < objectContainers.Length; i++)
			{
				if (objectContainers[i] == null)
				{
					throw new System.ArgumentException();
				}
			}
			_objectContainers = objectContainers;
		}

		/// <summary>
		/// starts a query against all ObjectContainers in
		/// this Cluster.
		/// </summary>
		/// <remarks>
		/// starts a query against all ObjectContainers in
		/// this Cluster.
		/// </remarks>
		/// <returns>the Query</returns>
		public virtual Db4objects.Db4o.Query.IQuery Query()
		{
			lock (this)
			{
				Db4objects.Db4o.Query.IQuery[] queries = new Db4objects.Db4o.Query.IQuery[_objectContainers
					.Length];
				for (int i = 0; i < _objectContainers.Length; i++)
				{
					queries[i] = _objectContainers[i].Query();
				}
				return new Db4objects.Db4o.Inside.Cluster.ClusterQuery(this, queries);
			}
		}

		/// <summary>
		/// returns the ObjectContainer in this cluster where the passed object
		/// is stored or null, if the object is not stored to any ObjectContainer
		/// in this cluster
		/// </summary>
		/// <param name="obj">the object</param>
		/// <returns>the ObjectContainer</returns>
		public virtual Db4objects.Db4o.IObjectContainer ObjectContainerFor(object obj)
		{
			lock (this)
			{
				for (int i = 0; i < _objectContainers.Length; i++)
				{
					if (_objectContainers[i].Ext().IsStored(obj))
					{
						return _objectContainers[i];
					}
				}
			}
			return null;
		}
	}
}
