/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <summary>Base class for all constraints on queries.</summary>
	/// <remarks>Base class for all constraints on queries.</remarks>
	/// <exclude></exclude>
	public abstract class QCon : IConstraint, IVisitor4, IUnversioned
	{
		internal static readonly IDGenerator idGenerator = new IDGenerator();

		[System.NonSerialized]
		internal QCandidates i_candidates;

		public Collection4 i_childrenCandidates;

		public List4 _children;

		public QE i_evaluator = QE.Default;

		public int i_id;

		public Collection4 i_joins;

		public int i_orderID = 0;

		public Db4objects.Db4o.Internal.Query.Processor.QCon i_parent;

		public bool i_removed = false;

		[System.NonSerialized]
		internal Db4objects.Db4o.Internal.Transaction i_trans;

		public QCon()
		{
		}

		internal QCon(Db4objects.Db4o.Internal.Transaction a_trans)
		{
			i_id = idGenerator.Next();
			i_trans = a_trans;
		}

		internal virtual Db4objects.Db4o.Internal.Query.Processor.QCon AddConstraint(Db4objects.Db4o.Internal.Query.Processor.QCon
			 a_child)
		{
			_children = new List4(_children, a_child);
			return a_child;
		}

		public virtual ObjectContainerBase Container()
		{
			return Transaction().Container();
		}

		public virtual Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return i_trans;
		}

		internal virtual void AddJoin(QConJoin a_join)
		{
			if (i_joins == null)
			{
				i_joins = new Collection4();
			}
			i_joins.Add(a_join);
		}

		internal virtual Db4objects.Db4o.Internal.Query.Processor.QCon AddSharedConstraint
			(QField a_field, object a_object)
		{
			QConObject newConstraint = new QConObject(i_trans, this, a_field, a_object);
			AddConstraint(newConstraint);
			return newConstraint;
		}

		public virtual IConstraint And(IConstraint andWith)
		{
			lock (StreamLock())
			{
				return Join(andWith, true);
			}
		}

		internal virtual void ApplyOrdering()
		{
			if (HasOrdering())
			{
				Db4objects.Db4o.Internal.Query.Processor.QCon root = GetRoot();
				root.i_candidates.ApplyOrdering(i_candidates.i_ordered, i_orderID);
			}
		}

		internal virtual bool Attach(QQuery query, string a_field)
		{
			Db4objects.Db4o.Internal.Query.Processor.QCon qcon = this;
			ClassMetadata yc = GetYapClass();
			bool[] foundField = new bool[] { false };
			ForEachChildField(a_field, new _IVisitor4_112(this, foundField, query));
			if (foundField[0])
			{
				return true;
			}
			QField qf = null;
			if (yc == null || yc.HoldsAnyClass())
			{
				int[] count = new int[] { 0 };
				FieldMetadata[] yfs = new FieldMetadata[] { null };
				i_trans.Container().ClassCollection().AttachQueryNode(a_field, new _IVisitor4_130
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
					qf = new QField(i_trans, a_field, null, 0, 0);
				}
			}
			else
			{
				if (yc.CustomizedNewInstance())
				{
					i_trans.Container()._handlers._diagnosticProcessor.DescendIntoTranslator(yc, a_field
						);
				}
				FieldMetadata yf = yc.FieldMetadataForName(a_field);
				if (yf != null)
				{
					qf = yf.QField(i_trans);
				}
				if (qf == null)
				{
					qf = new QField(i_trans, a_field, null, 0, 0);
				}
			}
			QConPath qcp = new QConPath(i_trans, qcon, qf);
			query.AddConstraint(qcp);
			qcon.AddConstraint(qcp);
			return true;
		}

		private sealed class _IVisitor4_112 : IVisitor4
		{
			public _IVisitor4_112(QCon _enclosing, bool[] foundField, QQuery query)
			{
				this._enclosing = _enclosing;
				this.foundField = foundField;
				this.query = query;
			}

			public void Visit(object obj)
			{
				foundField[0] = true;
				query.AddConstraint((Db4objects.Db4o.Internal.Query.Processor.QCon)obj);
			}

			private readonly QCon _enclosing;

			private readonly bool[] foundField;

			private readonly QQuery query;
		}

		private sealed class _IVisitor4_130 : IVisitor4
		{
			public _IVisitor4_130(QCon _enclosing, FieldMetadata[] yfs, int[] count)
			{
				this._enclosing = _enclosing;
				this.yfs = yfs;
				this.count = count;
			}

			public void Visit(object obj)
			{
				yfs[0] = (FieldMetadata)((object[])obj)[1];
				count[0]++;
			}

			private readonly QCon _enclosing;

			private readonly FieldMetadata[] yfs;

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

		/// <param name="candidates"></param>
		internal virtual void Collect(QCandidates candidates)
		{
		}

		public virtual IConstraint Contains()
		{
			throw NotSupported();
		}

		internal virtual void CreateCandidates(Collection4 a_candidateCollection)
		{
			IEnumerator j = a_candidateCollection.GetEnumerator();
			while (j.MoveNext())
			{
				QCandidates candidates = (QCandidates)j.Current;
				if (candidates.TryAddConstraint(this))
				{
					i_candidates = candidates;
					return;
				}
			}
			i_candidates = new QCandidates((LocalTransaction)i_trans, GetYapClass(), GetField
				());
			i_candidates.AddConstraint(this);
			a_candidateCollection.Add(i_candidates);
		}

		internal virtual void DoNotInclude(QCandidate a_root)
		{
			if (DTrace.enabled)
			{
				DTrace.Donotinclude.Log(i_id);
			}
			if (i_parent != null)
			{
				i_parent.Visit1(a_root, this, false);
			}
			else
			{
				a_root.DoNotInclude();
			}
		}

		public virtual IConstraint Equal()
		{
			throw NotSupported();
		}

		/// <param name="candidate"></param>
		internal virtual bool Evaluate(QCandidate candidate)
		{
			throw Exceptions4.VirtualException();
		}

		internal virtual void EvaluateChildren()
		{
			IEnumerator i = i_childrenCandidates.GetEnumerator();
			while (i.MoveNext())
			{
				((QCandidates)i.Current).Evaluate();
			}
		}

		internal virtual void EvaluateCollectChildren()
		{
			if (DTrace.enabled)
			{
				DTrace.CollectChildren.Log(i_id);
			}
			IEnumerator i = i_childrenCandidates.GetEnumerator();
			while (i.MoveNext())
			{
				((QCandidates)i.Current).Collect(i_candidates);
			}
		}

		internal virtual void EvaluateCreateChildrenCandidates()
		{
			i_childrenCandidates = new Collection4();
			IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).CreateCandidates(i_childrenCandidates
					);
			}
		}

		internal virtual void EvaluateEvaluations()
		{
			IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).EvaluateEvaluationsExec
					(i_candidates, true);
			}
		}

		/// <param name="candidates"></param>
		/// <param name="rereadObject"></param>
		internal virtual void EvaluateEvaluationsExec(QCandidates candidates, bool rereadObject
			)
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
			IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Internal.Query.Processor.QCon qcon = (Db4objects.Db4o.Internal.Query.Processor.QCon
					)i.Current;
				i_candidates.SetCurrentConstraint(qcon);
				qcon.SetCandidates(i_candidates);
				qcon.EvaluateSimpleExec(i_candidates);
				qcon.ApplyOrdering();
				i_candidates.ClearOrdering();
			}
			i_candidates.SetCurrentConstraint(null);
		}

		/// <param name="candidates"></param>
		internal virtual void EvaluateSimpleExec(QCandidates candidates)
		{
		}

		internal virtual void ExchangeConstraint(Db4objects.Db4o.Internal.Query.Processor.QCon
			 a_exchange, Db4objects.Db4o.Internal.Query.Processor.QCon a_with)
		{
			List4 previous = null;
			List4 current = _children;
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
			_children = new List4(_children, a_with);
		}

		internal virtual void ForEachChildField(string name, IVisitor4 visitor)
		{
			IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				object obj = i.Current;
				if (obj is QConObject)
				{
					if (((QConObject)obj).i_field.i_name.Equals(name))
					{
						visitor.Visit(obj);
					}
				}
			}
		}

		public virtual QField GetField()
		{
			return null;
		}

		public virtual object GetObject()
		{
			throw NotSupported();
		}

		internal virtual Db4objects.Db4o.Internal.Query.Processor.QCon GetRoot()
		{
			if (i_parent != null)
			{
				return i_parent.GetRoot();
			}
			return this;
		}

		internal virtual Db4objects.Db4o.Internal.Query.Processor.QCon ProduceTopLevelJoin
			()
		{
			if (!HasJoins())
			{
				return this;
			}
			IEnumerator i = IterateJoins();
			if (i_joins.Size() == 1)
			{
				i.MoveNext();
				return ((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).ProduceTopLevelJoin
					();
			}
			Collection4 col = new Collection4();
			while (i.MoveNext())
			{
				col.Ensure(((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).ProduceTopLevelJoin
					());
			}
			i = col.GetEnumerator();
			i.MoveNext();
			Db4objects.Db4o.Internal.Query.Processor.QCon qcon = (Db4objects.Db4o.Internal.Query.Processor.QCon
				)i.Current;
			if (col.Size() == 1)
			{
				return qcon;
			}
			while (i.MoveNext())
			{
				qcon = (Db4objects.Db4o.Internal.Query.Processor.QCon)qcon.And((IConstraint)i.Current
					);
			}
			return qcon;
		}

		internal virtual ClassMetadata GetYapClass()
		{
			return null;
		}

		public virtual IConstraint Greater()
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

		public virtual Db4objects.Db4o.Internal.Query.Processor.QCon Parent()
		{
			return i_parent;
		}

		public virtual bool HasOrJoins()
		{
			Collection4 lookedAt = new Collection4();
			return HasOrJoins(lookedAt);
		}

		internal virtual bool HasOrJoins(Collection4 lookedAt)
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
			IEnumerator i = IterateJoins();
			while (i.MoveNext())
			{
				QConJoin join = (QConJoin)i.Current;
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

		public virtual bool HasOrJoinWith(QConObject y)
		{
			IEnumerator i = IterateJoins();
			while (i.MoveNext())
			{
				QConJoin join = (QConJoin)i.Current;
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

		public virtual bool HasObjectInParentPath(object obj)
		{
			if (i_parent != null)
			{
				return i_parent.HasObjectInParentPath(obj);
			}
			return false;
		}

		public virtual IConstraint Identity()
		{
			throw NotSupported();
		}

		public virtual IConstraint ByExample()
		{
			throw NotSupported();
		}

		public virtual int IdentityID()
		{
			return 0;
		}

		internal virtual bool IsNot()
		{
			return i_evaluator is QENot;
		}

		internal virtual bool IsNullConstraint()
		{
			return false;
		}

		internal virtual IEnumerator IterateJoins()
		{
			if (i_joins == null)
			{
				return Iterators.EmptyIterator;
			}
			return i_joins.GetEnumerator();
		}

		public virtual IEnumerator IterateChildren()
		{
			if (_children == null)
			{
				return Iterators.EmptyIterator;
			}
			return new Iterator4Impl(_children);
		}

		internal virtual IConstraint Join(IConstraint a_with, bool a_and)
		{
			if (!(a_with is Db4objects.Db4o.Internal.Query.Processor.QCon))
			{
				return null;
			}
			if (a_with == this)
			{
				return this;
			}
			return Join1((Db4objects.Db4o.Internal.Query.Processor.QCon)a_with, a_and);
		}

		internal virtual IConstraint Join1(Db4objects.Db4o.Internal.Query.Processor.QCon 
			a_with, bool a_and)
		{
			if (a_with is QConstraints)
			{
				int j = 0;
				Collection4 joinHooks = new Collection4();
				IConstraint[] constraints = ((QConstraints)a_with).ToArray();
				for (j = 0; j < constraints.Length; j++)
				{
					joinHooks.Ensure(((Db4objects.Db4o.Internal.Query.Processor.QCon)constraints[j]).
						JoinHook());
				}
				IConstraint[] joins = new IConstraint[joinHooks.Size()];
				j = 0;
				IEnumerator i = joinHooks.GetEnumerator();
				while (i.MoveNext())
				{
					joins[j++] = Join((IConstraint)i.Current, a_and);
				}
				return new QConstraints(i_trans, joins);
			}
			Db4objects.Db4o.Internal.Query.Processor.QCon myHook = JoinHook();
			Db4objects.Db4o.Internal.Query.Processor.QCon otherHook = a_with.JoinHook();
			if (myHook == otherHook)
			{
				return myHook;
			}
			QConJoin cj = new QConJoin(i_trans, myHook, otherHook, a_and);
			myHook.AddJoin(cj);
			otherHook.AddJoin(cj);
			return cj;
		}

		internal virtual Db4objects.Db4o.Internal.Query.Processor.QCon JoinHook()
		{
			return ProduceTopLevelJoin();
		}

		public virtual IConstraint Like()
		{
			throw NotSupported();
		}

		public virtual IConstraint StartsWith(bool caseSensitive)
		{
			throw NotSupported();
		}

		public virtual IConstraint EndsWith(bool caseSensitive)
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
			IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).Marshall();
			}
		}

		public virtual IConstraint Not()
		{
			lock (StreamLock())
			{
				if (!(i_evaluator is QENot))
				{
					i_evaluator = new QENot(i_evaluator);
				}
				return this;
			}
		}

		private Exception NotSupported()
		{
			return new Exception("Not supported.");
		}

		/// <param name="other"></param>
		public virtual bool OnSameFieldAs(Db4objects.Db4o.Internal.Query.Processor.QCon other
			)
		{
			return false;
		}

		public virtual IConstraint Or(IConstraint orWith)
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
			IEnumerator i = IterateJoins();
			while (i.MoveNext())
			{
				QConJoin qcj = (QConJoin)i.Current;
				if (qcj.RemoveForParent(this))
				{
					i_joins.Remove(qcj);
				}
			}
			CheckLastJoinRemoved();
		}

		internal virtual void RemoveJoin(QConJoin a_join)
		{
			i_joins.Remove(a_join);
			CheckLastJoinRemoved();
		}

		internal virtual void RemoveNot()
		{
			if (IsNot())
			{
				i_evaluator = ((QENot)i_evaluator).i_evaluator;
			}
		}

		public virtual void SetCandidates(QCandidates a_candidates)
		{
			i_candidates = a_candidates;
		}

		internal virtual void SetOrdering(int a_ordering)
		{
			i_orderID = a_ordering;
		}

		public virtual int Ordering()
		{
			return i_orderID;
		}

		internal virtual void SetParent(Db4objects.Db4o.Internal.Query.Processor.QCon a_newParent
			)
		{
			i_parent = a_newParent;
		}

		/// <param name="obj"></param>
		/// <param name="removeExisting"></param>
		internal virtual Db4objects.Db4o.Internal.Query.Processor.QCon ShareParent(object
			 obj, bool[] removeExisting)
		{
			return null;
		}

		/// <param name="claxx"></param>
		/// <param name="removeExisting"></param>
		internal virtual QConClass ShareParentForClass(IReflectClass claxx, bool[] removeExisting
			)
		{
			return null;
		}

		public virtual IConstraint Smaller()
		{
			throw NotSupported();
		}

		protected virtual object StreamLock()
		{
			return i_trans.Container()._lock;
		}

		internal virtual bool SupportsOrdering()
		{
			return false;
		}

		internal virtual void Unmarshall(Db4objects.Db4o.Internal.Transaction a_trans)
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

		private void UnmarshallParent(Db4objects.Db4o.Internal.Transaction a_trans)
		{
			if (i_parent != null)
			{
				i_parent.Unmarshall(a_trans);
			}
		}

		private void UnmarshallChildren(Db4objects.Db4o.Internal.Transaction a_trans)
		{
			IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).Unmarshall(a_trans);
			}
		}

		private void UnmarshallJoins(Db4objects.Db4o.Internal.Transaction a_trans)
		{
			if (HasJoins())
			{
				IEnumerator i = IterateJoins();
				while (i.MoveNext())
				{
					((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).Unmarshall(a_trans);
				}
			}
		}

		public virtual void Visit(object obj)
		{
			QCandidate qc = (QCandidate)obj;
			Visit1(qc.GetRoot(), this, Evaluate(qc));
		}

		internal virtual void Visit(QCandidate a_root, bool res)
		{
			Visit1(a_root, this, i_evaluator.Not(res));
		}

		/// <param name="reason"></param>
		internal virtual void Visit1(QCandidate root, Db4objects.Db4o.Internal.Query.Processor.QCon
			 reason, bool res)
		{
			if (HasJoins())
			{
				IEnumerator i = IterateJoins();
				while (i.MoveNext())
				{
					root.Evaluate(new QPending((QConJoin)i.Current, this, res));
				}
			}
			else
			{
				if (!res)
				{
					DoNotInclude(root);
				}
			}
		}

		internal void VisitOnNull(QCandidate a_root)
		{
			IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).VisitOnNull(a_root);
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

		public virtual QE Evaluator()
		{
			return i_evaluator;
		}

		public virtual bool RequiresSort()
		{
			if (HasOrdering())
			{
				return true;
			}
			IEnumerator i = IterateChildren();
			while (i.MoveNext())
			{
				if (((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).RequiresSort())
				{
					return true;
				}
			}
			return false;
		}

		protected virtual bool HasOrdering()
		{
			return i_orderID != 0;
		}
	}
}
