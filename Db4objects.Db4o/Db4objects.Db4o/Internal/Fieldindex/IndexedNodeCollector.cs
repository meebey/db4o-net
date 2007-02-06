namespace Db4objects.Db4o.Internal.Fieldindex
{
	public class IndexedNodeCollector
	{
		private readonly Db4objects.Db4o.Foundation.Collection4 _nodes;

		private readonly Db4objects.Db4o.Foundation.Hashtable4 _nodeCache;

		public IndexedNodeCollector(Db4objects.Db4o.Internal.Query.Processor.QCandidates 
			candidates)
		{
			_nodes = new Db4objects.Db4o.Foundation.Collection4();
			_nodeCache = new Db4objects.Db4o.Foundation.Hashtable4();
			CollectIndexedNodes(candidates);
		}

		public virtual System.Collections.IEnumerator GetNodes()
		{
			return _nodes.GetEnumerator();
		}

		private void CollectIndexedNodes(Db4objects.Db4o.Internal.Query.Processor.QCandidates
			 candidates)
		{
			CollectIndexedNodes(candidates.IterateConstraints());
			ImplicitlyAndJoinsOnSameField();
		}

		private void ImplicitlyAndJoinsOnSameField()
		{
			object[] nodes = _nodes.ToArray();
			for (int i = 0; i < nodes.Length; i++)
			{
				object node = nodes[i];
				if (node is Db4objects.Db4o.Internal.Fieldindex.OrIndexedLeaf)
				{
					Db4objects.Db4o.Internal.Fieldindex.OrIndexedLeaf current = (Db4objects.Db4o.Internal.Fieldindex.OrIndexedLeaf
						)node;
					Db4objects.Db4o.Internal.Fieldindex.OrIndexedLeaf other = FindJoinOnSameFieldAtSameLevel
						(current);
					if (null != other)
					{
						nodes[Db4objects.Db4o.Foundation.Arrays4.IndexOf(nodes, other)] = null;
						CollectImplicitAnd(current.GetConstraint(), current, other);
					}
				}
			}
		}

		private Db4objects.Db4o.Internal.Fieldindex.OrIndexedLeaf FindJoinOnSameFieldAtSameLevel
			(Db4objects.Db4o.Internal.Fieldindex.OrIndexedLeaf join)
		{
			System.Collections.IEnumerator i = _nodes.GetEnumerator();
			while (i.MoveNext())
			{
				if (i.Current == join)
				{
					continue;
				}
				if (i.Current is Db4objects.Db4o.Internal.Fieldindex.OrIndexedLeaf)
				{
					Db4objects.Db4o.Internal.Fieldindex.OrIndexedLeaf current = (Db4objects.Db4o.Internal.Fieldindex.OrIndexedLeaf
						)i.Current;
					if (current.GetIndex() == join.GetIndex() && ParentConstraint(current) == ParentConstraint
						(join))
					{
						return current;
					}
				}
			}
			return null;
		}

		private object ParentConstraint(Db4objects.Db4o.Internal.Fieldindex.OrIndexedLeaf
			 node)
		{
			return node.GetConstraint().Parent();
		}

		private void CollectIndexedNodes(System.Collections.IEnumerator qcons)
		{
			while (qcons.MoveNext())
			{
				Db4objects.Db4o.Internal.Query.Processor.QCon qcon = (Db4objects.Db4o.Internal.Query.Processor.QCon
					)qcons.Current;
				if (IsCached(qcon))
				{
					continue;
				}
				if (IsLeaf(qcon))
				{
					if (qcon.CanLoadByIndex() && qcon.CanBeIndexLeaf())
					{
						Db4objects.Db4o.Internal.Query.Processor.QConObject conObject = (Db4objects.Db4o.Internal.Query.Processor.QConObject
							)qcon;
						if (conObject.HasJoins())
						{
							CollectJoinedNode(conObject);
						}
						else
						{
							CollectStandaloneNode(conObject);
						}
					}
				}
				else
				{
					if (!qcon.HasJoins())
					{
						CollectIndexedNodes(qcon.IterateChildren());
					}
				}
			}
		}

		private bool IsCached(Db4objects.Db4o.Internal.Query.Processor.QCon qcon)
		{
			return null != _nodeCache.Get(qcon);
		}

		private void CollectStandaloneNode(Db4objects.Db4o.Internal.Query.Processor.QConObject
			 conObject)
		{
			Db4objects.Db4o.Internal.Fieldindex.IndexedLeaf existing = FindLeafOnSameField(conObject
				);
			if (existing != null)
			{
				CollectImplicitAnd(conObject, existing, new Db4objects.Db4o.Internal.Fieldindex.IndexedLeaf
					(conObject));
			}
			else
			{
				_nodes.Add(new Db4objects.Db4o.Internal.Fieldindex.IndexedLeaf(conObject));
			}
		}

		private void CollectJoinedNode(Db4objects.Db4o.Internal.Query.Processor.QConObject
			 constraintWithJoins)
		{
			Db4objects.Db4o.Foundation.Collection4 joins = CollectTopLevelJoins(constraintWithJoins
				);
			if (!CanJoinsBeSearchedByIndex(joins))
			{
				return;
			}
			if (1 == joins.Size())
			{
				_nodes.Add(NodeForConstraint((Db4objects.Db4o.Internal.Query.Processor.QCon)joins
					.SingleElement()));
				return;
			}
			CollectImplicitlyAndingJoins(joins, constraintWithJoins);
		}

		private bool AllHaveSamePath(Db4objects.Db4o.Foundation.Collection4 leaves)
		{
			System.Collections.IEnumerator i = leaves.GetEnumerator();
			i.MoveNext();
			Db4objects.Db4o.Internal.Query.Processor.QCon first = (Db4objects.Db4o.Internal.Query.Processor.QCon
				)i.Current;
			while (i.MoveNext())
			{
				if (!HaveSamePath(first, (Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current
					))
				{
					return false;
				}
			}
			return true;
		}

		private bool HaveSamePath(Db4objects.Db4o.Internal.Query.Processor.QCon x, Db4objects.Db4o.Internal.Query.Processor.QCon
			 y)
		{
			if (x == y)
			{
				return true;
			}
			if (!x.OnSameFieldAs(y))
			{
				return false;
			}
			if (!x.HasParent())
			{
				return !y.HasParent();
			}
			return HaveSamePath(x.Parent(), y.Parent());
		}

		private Db4objects.Db4o.Foundation.Collection4 CollectLeaves(Db4objects.Db4o.Foundation.Collection4
			 joins)
		{
			Db4objects.Db4o.Foundation.Collection4 leaves = new Db4objects.Db4o.Foundation.Collection4
				();
			CollectLeaves(leaves, joins);
			return leaves;
		}

		private void CollectLeaves(Db4objects.Db4o.Foundation.Collection4 leaves, Db4objects.Db4o.Foundation.Collection4
			 joins)
		{
			System.Collections.IEnumerator i = joins.GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Internal.Query.Processor.QConJoin join = ((Db4objects.Db4o.Internal.Query.Processor.QConJoin
					)i.Current);
				CollectLeavesFromJoin(leaves, join);
			}
		}

		private void CollectLeavesFromJoin(Db4objects.Db4o.Foundation.Collection4 leaves, 
			Db4objects.Db4o.Internal.Query.Processor.QConJoin join)
		{
			CollectLeavesFromJoinConstraint(leaves, join.i_constraint1);
			CollectLeavesFromJoinConstraint(leaves, join.i_constraint2);
		}

		private void CollectLeavesFromJoinConstraint(Db4objects.Db4o.Foundation.Collection4
			 leaves, Db4objects.Db4o.Internal.Query.Processor.QCon constraint)
		{
			if (constraint is Db4objects.Db4o.Internal.Query.Processor.QConJoin)
			{
				CollectLeavesFromJoin(leaves, (Db4objects.Db4o.Internal.Query.Processor.QConJoin)
					constraint);
			}
			else
			{
				if (!leaves.ContainsByIdentity(constraint))
				{
					leaves.Add(constraint);
				}
			}
		}

		private bool CanJoinsBeSearchedByIndex(Db4objects.Db4o.Foundation.Collection4 joins
			)
		{
			Db4objects.Db4o.Foundation.Collection4 leaves = CollectLeaves(joins);
			return AllHaveSamePath(leaves) && AllCanBeSearchedByIndex(leaves);
		}

		private bool AllCanBeSearchedByIndex(Db4objects.Db4o.Foundation.Collection4 leaves
			)
		{
			System.Collections.IEnumerator i = leaves.GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Internal.Query.Processor.QCon leaf = ((Db4objects.Db4o.Internal.Query.Processor.QCon
					)i.Current);
				if (!leaf.CanLoadByIndex())
				{
					return false;
				}
			}
			return true;
		}

		private void CollectImplicitlyAndingJoins(Db4objects.Db4o.Foundation.Collection4 
			joins, Db4objects.Db4o.Internal.Query.Processor.QConObject constraintWithJoins)
		{
			System.Collections.IEnumerator i = joins.GetEnumerator();
			i.MoveNext();
			Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange last = NodeForConstraint
				((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current);
			while (i.MoveNext())
			{
				Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange node = NodeForConstraint
					((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current);
				last = new Db4objects.Db4o.Internal.Fieldindex.AndIndexedLeaf(constraintWithJoins
					, node, last);
				_nodes.Add(last);
			}
		}

		private Db4objects.Db4o.Foundation.Collection4 CollectTopLevelJoins(Db4objects.Db4o.Internal.Query.Processor.QConObject
			 constraintWithJoins)
		{
			Db4objects.Db4o.Foundation.Collection4 joins = new Db4objects.Db4o.Foundation.Collection4
				();
			CollectTopLevelJoins(joins, constraintWithJoins);
			return joins;
		}

		private void CollectTopLevelJoins(Db4objects.Db4o.Foundation.Collection4 joins, Db4objects.Db4o.Internal.Query.Processor.QCon
			 constraintWithJoins)
		{
			System.Collections.IEnumerator i = constraintWithJoins.i_joins.GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Internal.Query.Processor.QConJoin join = (Db4objects.Db4o.Internal.Query.Processor.QConJoin
					)i.Current;
				if (!join.HasJoins())
				{
					if (!joins.ContainsByIdentity(join))
					{
						joins.Add(join);
					}
				}
				else
				{
					CollectTopLevelJoins(joins, join);
				}
			}
		}

		private Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange NewNodeForConstraint
			(Db4objects.Db4o.Internal.Query.Processor.QConJoin join)
		{
			Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange c1 = NodeForConstraint(
				join.i_constraint1);
			Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange c2 = NodeForConstraint(
				join.i_constraint2);
			if (join.IsOr())
			{
				return new Db4objects.Db4o.Internal.Fieldindex.OrIndexedLeaf(join.i_constraint1, 
					c1, c2);
			}
			return new Db4objects.Db4o.Internal.Fieldindex.AndIndexedLeaf(join.i_constraint1, 
				c1, c2);
		}

		private Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange NodeForConstraint
			(Db4objects.Db4o.Internal.Query.Processor.QCon con)
		{
			Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange node = (Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange
				)_nodeCache.Get(con);
			if (null != node || _nodeCache.ContainsKey(con))
			{
				return node;
			}
			node = NewNodeForConstraint(con);
			_nodeCache.Put(con, node);
			return node;
		}

		private Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange NewNodeForConstraint
			(Db4objects.Db4o.Internal.Query.Processor.QCon con)
		{
			if (con is Db4objects.Db4o.Internal.Query.Processor.QConJoin)
			{
				return NewNodeForConstraint((Db4objects.Db4o.Internal.Query.Processor.QConJoin)con
					);
			}
			return new Db4objects.Db4o.Internal.Fieldindex.IndexedLeaf((Db4objects.Db4o.Internal.Query.Processor.QConObject
				)con);
		}

		private void CollectImplicitAnd(Db4objects.Db4o.Internal.Query.Processor.QCon constraint
			, Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange x, Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange
			 y)
		{
			_nodes.Remove(x);
			_nodes.Remove(y);
			_nodes.Add(new Db4objects.Db4o.Internal.Fieldindex.AndIndexedLeaf(constraint, x, 
				y));
		}

		private Db4objects.Db4o.Internal.Fieldindex.IndexedLeaf FindLeafOnSameField(Db4objects.Db4o.Internal.Query.Processor.QConObject
			 conObject)
		{
			System.Collections.IEnumerator i = _nodes.GetEnumerator();
			while (i.MoveNext())
			{
				if (i.Current is Db4objects.Db4o.Internal.Fieldindex.IndexedLeaf)
				{
					Db4objects.Db4o.Internal.Fieldindex.IndexedLeaf leaf = (Db4objects.Db4o.Internal.Fieldindex.IndexedLeaf
						)i.Current;
					if (conObject.OnSameFieldAs(leaf.Constraint()))
					{
						return leaf;
					}
				}
			}
			return null;
		}

		private bool IsLeaf(Db4objects.Db4o.Internal.Query.Processor.QCon qcon)
		{
			return !qcon.HasChildren();
		}
	}
}
