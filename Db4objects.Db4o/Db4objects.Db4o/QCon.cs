namespace Db4objects.Db4o
{
	/// <summary>Base class for all constraints on queries.</summary>
	/// <remarks>Base class for all constraints on queries.</remarks>
	/// <exclude></exclude>
	public abstract class QCon : Db4objects.Db4o.Query.IConstraint, Db4objects.Db4o.Foundation.IVisitor4
		, Db4objects.Db4o.Types.IUnversioned
	{
		internal static readonly Db4objects.Db4o.IDGenerator idGenerator = new Db4objects.Db4o.IDGenerator
			();

		[Db4objects.Db4o.Transient]
		internal Db4objects.Db4o.QCandidates i_candidates;

		public Db4objects.Db4o.Foundation.Collection4 i_childrenCandidates;

		public Db4objects.Db4o.Foundation.List4 _children;

		public Db4objects.Db4o.QE i_evaluator = Db4objects.Db4o.QE.DEFAULT;

		public int i_id;

		public Db4objects.Db4o.Foundation.Collection4 i_joins;

		public int i_orderID = 0;

		public Db4objects.Db4o.QCon i_parent;

		public bool i_removed = false;

		[Db4objects.Db4o.Transient]
		internal Db4objects.Db4o.Transaction i_trans;

		public QCon()
		{
		}

		internal QCon(Db4objects.Db4o.Transaction a_trans)
		{
			i_id = idGenerator.Next();
			i_trans = a_trans;
		}

		internal virtual Db4objects.Db4o.QCon AddConstraint(Db4objects.Db4o.QCon a_child)
		{
			_children = new Db4objects.Db4o.Foundation.List4(_children, a_child);
			return a_child;
		}

		public virtual Db4objects.Db4o.Transaction Transaction()
		{
			return i_trans;
		}

		internal virtual void AddJoin(Db4objects.Db4o.QConJoin a_join)
		{
			if (i_joins == null)
			{
				i_joins = new Db4objects.Db4o.Foundation.Collection4();
			}
			i_joins.Add(a_join);
		}

		internal virtual Db4objects.Db4o.QCon AddSharedConstraint(Db4objects.Db4o.QField 
			a_field, object a_object)
		{
			Db4objects.Db4o.QConObject newConstraint = new Db4objects.Db4o.QConObject(i_trans
				, this, a_field, a_object);
			AddConstraint(newConstraint);
			return newConstraint;
		}

		public virtual Db4objects.Db4o.Query.IConstraint And(Db4objects.Db4o.Query.IConstraint
			 andWith)
		{
			lock (StreamLock())
			{
				return Join(andWith, true);
			}
		}

		internal virtual void ApplyOrdering()
		{
			if (i_orderID != 0)
			{
				Db4objects.Db4o.QCon root = GetRoot();
				root.i_candidates.ApplyOrdering(i_candidates.i_ordered, i_orderID);
			}
		}

		internal virtual bool Attach(Db4objects.Db4o.QQuery query, string a_field)
		{
			Db4objects.Db4o.QCon qcon = this;
			Db4objects.Db4o.YapClass yc = GetYapClass();
			bool[] foundField = { false };
			ForEachChildField(a_field, new _AnonymousInnerClass106(this, foundField, query));
			if (foundField[0])
			{
				return true;
			}
			Db4objects.Db4o.QField qf = null;
			if (yc == null || yc.HoldsAnyClass())
			{
				int[] count = { 0 };
				Db4objects.Db4o.YapField[] yfs = { null };
				i_trans.Stream().ClassCollection().AttachQueryNode(a_field, new _AnonymousInnerClass124
					(this, yfs, count));
				if (count[0] == 0)
				{
					return false;
				}
				if (count[0] == 1)
				{
					qf = yfs[0].QField(i_trans);
				}
				else
				{
					qf = new Db4objects.Db4o.QField(i_trans, a_field, null, 0, 0);
				}
			}
			else
			{
				if (yc.ConfigInstantiates())
				{
					i_trans.Stream().i_handlers._diagnosticProcessor.DescendIntoTranslator(yc, a_field
						);
				}
				Db4objects.Db4o.YapField yf = yc.GetYapField(a_field);
				if (yf != null)
				{
					qf = yf.QField(i_trans);
				}
				if (qf == null)
				{
					qf = new Db4objects.Db4o.QField(i_trans, a_field, null, 0, 0);
				}
			}
			Db4objects.Db4o.QConPath qcp = new Db4objects.Db4o.QConPath(i_trans, qcon, qf);
			query.AddConstraint(qcp);
			qcon.AddConstraint(qcp);
			return true;
		}

		private sealed class _AnonymousInnerClass106 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass106(QCon _enclosing, bool[] foundField, Db4objects.Db4o.QQuery
				 query)
			{
				this._enclosing = _enclosing;
				this.foundField = foundField;
				this.query = query;
			}

			public void Visit(object obj)
			{
				foundField[0] = true;
				query.AddConstraint((Db4objects.Db4o.QCon)obj);
			}

			private readonly QCon _enclosing;

			private readonly bool[] foundField;

			private readonly Db4objects.Db4o.QQuery query;
		}

		private sealed class _AnonymousInnerClass124 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass124(QCon _enclosing, Db4objects.Db4o.YapField[] yfs, int[]
				 count)
			{
				this._enclosing = _enclosing;
				this.yfs = yfs;
				this.count = count;
			}

			public void Visit(object obj)
			{
				yfs[0] = (Db4objects.Db4o.YapField)((object[])obj)[1];
				count[0]++;
			}

			private readonly QCon _enclosing;

			private readonly Db4objects.Db4o.YapField[] yfs;

			private readonly int[] count;
		}

		public virtual bool CanBeIndexLeaf()
		{
			return false;
		}

		public virtual bool CanLoadByIndex()
		{
			return false;
		}

		internal virtual void CheckLastJoinRemoved()
		{
			if (i_joins.Size() == 0)
			{
				i_joins = null;
			}
		}

		internal virtual void Collect(Db4objects.Db4o.QCandidates a_candidates)
		{
		}

		public virtual Db4objects.Db4o.Query.IConstraint Contains()
		{
			throw NotSupported();
		}

		internal virtual void CreateCandidates(Db4objects.Db4o.Foundation.Collection4 a_candidateCollection
			)
		{
			System.Collections.IEnumerator j = a_candidateCollection.GetEnumerator();
			while (j.MoveNext())
			{
				Db4objects.Db4o.QCandidates candidates = (Db4objects.Db4o.QCandidates)j.Current;
				if (candidates.TryAddConstraint(this))
				{
					i_candidates = candidates;
					return;
				}
			}
			i_candidates = new Db4objects.Db4o.QCandidates(i_trans, GetYapClass(), GetField()
				);
			i_candidates.AddConstraint(this);
			a_candidateCollection.Add(i_candidates);
		}

		internal virtual void DoNotInclude(Db4objects.Db4o.QCandidate a_root)
		{
			if (i_parent != null)
			{
				i_parent.Visit1(a_root, this, false);
			}
			else
			{
				a_root.DoNotInclude();
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraint Equal()
		{
			throw NotSupported();
		}

		internal virtual bool Evaluate(Db4objects.Db4o.QCandidate a_candidate)
		{
			throw Db4objects.Db4o.Inside.Exceptions4.VirtualException();
		}

		internal virtual void EvaluateChildren()
		{
			System.Collections.IEnumerator i = i_childrenCandidates.GetEnumerator();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCandidates)i.Current).Evaluate();
			}
		}

		internal virtual void EvaluateCollectChildren()
		{
			System.Collections.IEnumerator i = i_childrenCandidates.GetEnumerator();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCandidates)i.Current).Collect(i_candidates);
			}
		}

		internal virtual void EvaluateCreateChildrenCandidates()
		{
			i_childrenCandidates = new Db4objects.Db4o.Foundation.Collection4();
			System.Collections.IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCon)i.Current).CreateCandidates(i_childrenCandidates);
			}
		}

		internal virtual void EvaluateEvaluations()
		{
			System.Collections.IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCon)i.Current).EvaluateEvaluationsExec(i_candidates, true);
			}
		}

		internal virtual void EvaluateEvaluationsExec(Db4objects.Db4o.QCandidates a_candidates
			, bool rereadObject)
		{
		}

		internal virtual void EvaluateSelf()
		{
			i_candidates.Filter(this);
		}

		internal virtual void EvaluateSimpleChildren()
		{
			if (_children == null)
			{
				return;
			}
			System.Collections.IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				Db4objects.Db4o.QCon qcon = (Db4objects.Db4o.QCon)i.Current;
				i_candidates.SetCurrentConstraint(qcon);
				qcon.SetCandidates(i_candidates);
				qcon.EvaluateSimpleExec(i_candidates);
				qcon.ApplyOrdering();
			}
			i_candidates.SetCurrentConstraint(null);
		}

		internal virtual void EvaluateSimpleExec(Db4objects.Db4o.QCandidates a_candidates
			)
		{
		}

		internal virtual void ExchangeConstraint(Db4objects.Db4o.QCon a_exchange, Db4objects.Db4o.QCon
			 a_with)
		{
			Db4objects.Db4o.Foundation.List4 previous = null;
			Db4objects.Db4o.Foundation.List4 current = _children;
			while (current != null)
			{
				if (current._element == a_exchange)
				{
					if (previous == null)
					{
						_children = current._next;
					}
					else
					{
						previous._next = current._next;
					}
				}
				previous = current;
				current = current._next;
			}
			_children = new Db4objects.Db4o.Foundation.List4(_children, a_with);
		}

		internal virtual void ForEachChildField(string name, Db4objects.Db4o.Foundation.IVisitor4
			 visitor)
		{
			System.Collections.IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				object obj = i.Current;
				if (obj is Db4objects.Db4o.QConObject)
				{
					if (((Db4objects.Db4o.QConObject)obj).i_field.i_name.Equals(name))
					{
						visitor.Visit(obj);
					}
				}
			}
		}

		public virtual Db4objects.Db4o.QField GetField()
		{
			return null;
		}

		public virtual object GetObject()
		{
			throw NotSupported();
		}

		internal virtual Db4objects.Db4o.QCon GetRoot()
		{
			if (i_parent != null)
			{
				return i_parent.GetRoot();
			}
			return this;
		}

		internal virtual Db4objects.Db4o.QCon ProduceTopLevelJoin()
		{
			if (!HasJoins())
			{
				return this;
			}
			System.Collections.IEnumerator i = IterateJoins();
			if (i_joins.Size() == 1)
			{
				i.MoveNext();
				return ((Db4objects.Db4o.QCon)i.Current).ProduceTopLevelJoin();
			}
			Db4objects.Db4o.Foundation.Collection4 col = new Db4objects.Db4o.Foundation.Collection4
				();
			while (i.MoveNext())
			{
				col.Ensure(((Db4objects.Db4o.QCon)i.Current).ProduceTopLevelJoin());
			}
			i = col.GetEnumerator();
			i.MoveNext();
			Db4objects.Db4o.QCon qcon = (Db4objects.Db4o.QCon)i.Current;
			if (col.Size() == 1)
			{
				return qcon;
			}
			while (i.MoveNext())
			{
				qcon = (Db4objects.Db4o.QCon)qcon.And((Db4objects.Db4o.Query.IConstraint)i.Current
					);
			}
			return qcon;
		}

		internal virtual Db4objects.Db4o.YapClass GetYapClass()
		{
			return null;
		}

		public virtual Db4objects.Db4o.Query.IConstraint Greater()
		{
			throw NotSupported();
		}

		public virtual bool HasChildren()
		{
			return _children != null;
		}

		public virtual bool HasParent()
		{
			return i_parent != null;
		}

		public virtual Db4objects.Db4o.QCon Parent()
		{
			return i_parent;
		}

		public virtual bool HasOrJoins()
		{
			Db4objects.Db4o.Foundation.Collection4 lookedAt = new Db4objects.Db4o.Foundation.Collection4
				();
			return HasOrJoins(lookedAt);
		}

		internal virtual bool HasOrJoins(Db4objects.Db4o.Foundation.Collection4 lookedAt)
		{
			if (lookedAt.ContainsByIdentity(this))
			{
				return false;
			}
			lookedAt.Add(this);
			if (i_joins == null)
			{
				return false;
			}
			System.Collections.IEnumerator i = IterateJoins();
			while (i.MoveNext())
			{
				Db4objects.Db4o.QConJoin join = (Db4objects.Db4o.QConJoin)i.Current;
				if (join.IsOr())
				{
					return true;
				}
				if (join.HasOrJoins(lookedAt))
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool HasOrJoinWith(Db4objects.Db4o.QConObject y)
		{
			System.Collections.IEnumerator i = IterateJoins();
			while (i.MoveNext())
			{
				Db4objects.Db4o.QConJoin join = (Db4objects.Db4o.QConJoin)i.Current;
				if (join.IsOr())
				{
					if (y == join.GetOtherConstraint(this))
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual bool HasJoins()
		{
			if (i_joins == null)
			{
				return false;
			}
			return i_joins.Size() > 0;
		}

		internal virtual bool HasObjectInParentPath(object obj)
		{
			if (i_parent != null)
			{
				return i_parent.HasObjectInParentPath(obj);
			}
			return false;
		}

		public virtual Db4objects.Db4o.Query.IConstraint Identity()
		{
			throw NotSupported();
		}

		public virtual int IdentityID()
		{
			return 0;
		}

		internal virtual bool IsNot()
		{
			return i_evaluator is Db4objects.Db4o.QENot;
		}

		internal virtual bool IsNullConstraint()
		{
			return false;
		}

		internal virtual System.Collections.IEnumerator IterateJoins()
		{
			if (i_joins == null)
			{
				return Db4objects.Db4o.Foundation.Iterator4Impl.EMPTY;
			}
			return i_joins.GetEnumerator();
		}

		public virtual System.Collections.IEnumerator IterateChildren()
		{
			if (_children == null)
			{
				return Db4objects.Db4o.Foundation.Iterator4Impl.EMPTY;
			}
			return new Db4objects.Db4o.Foundation.Iterator4Impl(_children);
		}

		internal virtual Db4objects.Db4o.Query.IConstraint Join(Db4objects.Db4o.Query.IConstraint
			 a_with, bool a_and)
		{
			if (!(a_with is Db4objects.Db4o.QCon))
			{
				return null;
			}
			if (a_with == this)
			{
				return this;
			}
			return Join1((Db4objects.Db4o.QCon)a_with, a_and);
		}

		internal virtual Db4objects.Db4o.Query.IConstraint Join1(Db4objects.Db4o.QCon a_with
			, bool a_and)
		{
			if (a_with is Db4objects.Db4o.QConstraints)
			{
				int j = 0;
				Db4objects.Db4o.Foundation.Collection4 joinHooks = new Db4objects.Db4o.Foundation.Collection4
					();
				Db4objects.Db4o.Query.IConstraint[] constraints = ((Db4objects.Db4o.QConstraints)
					a_with).ToArray();
				for (j = 0; j < constraints.Length; j++)
				{
					joinHooks.Ensure(((Db4objects.Db4o.QCon)constraints[j]).JoinHook());
				}
				Db4objects.Db4o.Query.IConstraint[] joins = new Db4objects.Db4o.Query.IConstraint
					[joinHooks.Size()];
				j = 0;
				System.Collections.IEnumerator i = joinHooks.GetEnumerator();
				while (i.MoveNext())
				{
					joins[j++] = Join((Db4objects.Db4o.Query.IConstraint)i.Current, a_and);
				}
				return new Db4objects.Db4o.QConstraints(i_trans, joins);
			}
			Db4objects.Db4o.QCon myHook = JoinHook();
			Db4objects.Db4o.QCon otherHook = a_with.JoinHook();
			if (myHook == otherHook)
			{
				return myHook;
			}
			Db4objects.Db4o.QConJoin cj = new Db4objects.Db4o.QConJoin(i_trans, myHook, otherHook
				, a_and);
			myHook.AddJoin(cj);
			otherHook.AddJoin(cj);
			return cj;
		}

		internal virtual Db4objects.Db4o.QCon JoinHook()
		{
			return ProduceTopLevelJoin();
		}

		public virtual Db4objects.Db4o.Query.IConstraint Like()
		{
			throw NotSupported();
		}

		public virtual Db4objects.Db4o.Query.IConstraint StartsWith(bool caseSensitive)
		{
			throw NotSupported();
		}

		public virtual Db4objects.Db4o.Query.IConstraint EndsWith(bool caseSensitive)
		{
			throw NotSupported();
		}

		internal virtual void Log(string indent)
		{
		}

		internal virtual string LogObject()
		{
			return string.Empty;
		}

		internal virtual void Marshall()
		{
			System.Collections.IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCon)i.Current).Marshall();
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraint Not()
		{
			lock (StreamLock())
			{
				if (!(i_evaluator is Db4objects.Db4o.QENot))
				{
					i_evaluator = new Db4objects.Db4o.QENot(i_evaluator);
				}
				return this;
			}
		}

		private System.Exception NotSupported()
		{
			return new System.Exception("Not supported.");
		}

		public virtual bool OnSameFieldAs(Db4objects.Db4o.QCon other)
		{
			return false;
		}

		public virtual Db4objects.Db4o.Query.IConstraint Or(Db4objects.Db4o.Query.IConstraint
			 orWith)
		{
			lock (StreamLock())
			{
				return Join(orWith, false);
			}
		}

		internal virtual bool Remove()
		{
			if (!i_removed)
			{
				i_removed = true;
				RemoveChildrenJoins();
				return true;
			}
			return false;
		}

		internal virtual void RemoveChildrenJoins()
		{
			if (!HasJoins())
			{
				return;
			}
			System.Collections.IEnumerator i = IterateJoins();
			while (i.MoveNext())
			{
				Db4objects.Db4o.QConJoin qcj = (Db4objects.Db4o.QConJoin)i.Current;
				if (qcj.RemoveForParent(this))
				{
					i_joins.Remove(qcj);
				}
			}
			CheckLastJoinRemoved();
		}

		internal virtual void RemoveJoin(Db4objects.Db4o.QConJoin a_join)
		{
			i_joins.Remove(a_join);
			CheckLastJoinRemoved();
		}

		internal virtual void RemoveNot()
		{
			if (IsNot())
			{
				i_evaluator = ((Db4objects.Db4o.QENot)i_evaluator).i_evaluator;
			}
		}

		public virtual void SetCandidates(Db4objects.Db4o.QCandidates a_candidates)
		{
			i_candidates = a_candidates;
		}

		internal virtual void SetOrdering(int a_ordering)
		{
			i_orderID = a_ordering;
		}

		internal virtual void SetParent(Db4objects.Db4o.QCon a_newParent)
		{
			i_parent = a_newParent;
		}

		internal virtual Db4objects.Db4o.QCon ShareParent(object a_object, bool[] removeExisting
			)
		{
			return null;
		}

		internal virtual Db4objects.Db4o.QConClass ShareParentForClass(Db4objects.Db4o.Reflect.IReflectClass
			 a_class, bool[] removeExisting)
		{
			return null;
		}

		public virtual Db4objects.Db4o.Query.IConstraint Smaller()
		{
			throw NotSupported();
		}

		protected virtual object StreamLock()
		{
			return i_trans.Stream().i_lock;
		}

		internal virtual bool SupportsOrdering()
		{
			return false;
		}

		internal virtual void Unmarshall(Db4objects.Db4o.Transaction a_trans)
		{
			if (i_trans != null)
			{
				return;
			}
			i_trans = a_trans;
			UnmarshallParent(a_trans);
			UnmarshallJoins(a_trans);
			UnmarshallChildren(a_trans);
		}

		private void UnmarshallParent(Db4objects.Db4o.Transaction a_trans)
		{
			if (i_parent != null)
			{
				i_parent.Unmarshall(a_trans);
			}
		}

		private void UnmarshallChildren(Db4objects.Db4o.Transaction a_trans)
		{
			System.Collections.IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCon)i.Current).Unmarshall(a_trans);
			}
		}

		private void UnmarshallJoins(Db4objects.Db4o.Transaction a_trans)
		{
			if (HasJoins())
			{
				System.Collections.IEnumerator i = IterateJoins();
				while (i.MoveNext())
				{
					((Db4objects.Db4o.QCon)i.Current).Unmarshall(a_trans);
				}
			}
		}

		public virtual void Visit(object obj)
		{
			Db4objects.Db4o.QCandidate qc = (Db4objects.Db4o.QCandidate)obj;
			Visit1(qc.GetRoot(), this, Evaluate(qc));
		}

		internal virtual void Visit(Db4objects.Db4o.QCandidate a_root, bool res)
		{
			Visit1(a_root, this, i_evaluator.Not(res));
		}

		internal virtual void Visit1(Db4objects.Db4o.QCandidate a_root, Db4objects.Db4o.QCon
			 a_reason, bool res)
		{
			if (HasJoins())
			{
				System.Collections.IEnumerator i = IterateJoins();
				while (i.MoveNext())
				{
					a_root.Evaluate(new Db4objects.Db4o.QPending((Db4objects.Db4o.QConJoin)i.Current, 
						this, res));
				}
			}
			else
			{
				if (!res)
				{
					DoNotInclude(a_root);
				}
			}
		}

		internal void VisitOnNull(Db4objects.Db4o.QCandidate a_root)
		{
			System.Collections.IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCon)i.Current).VisitOnNull(a_root);
			}
			if (VisitSelfOnNull())
			{
				Visit(a_root, IsNullConstraint());
			}
		}

		internal virtual bool VisitSelfOnNull()
		{
			return true;
		}

		public virtual Db4objects.Db4o.QE Evaluator()
		{
			return i_evaluator;
		}

		public virtual bool RequiresSort()
		{
			if (i_orderID != 0)
			{
				return true;
			}
			System.Collections.IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				if (((Db4objects.Db4o.QCon)i.Current).RequiresSort())
				{
					return true;
				}
			}
			return false;
		}
	}
}
