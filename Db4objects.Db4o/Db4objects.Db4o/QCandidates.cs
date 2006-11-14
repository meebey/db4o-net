namespace Db4objects.Db4o
{
	/// <summary>
	/// Holds the tree of
	/// <see cref="Db4objects.Db4o.QCandidate">Db4objects.Db4o.QCandidate</see>
	/// objects and the list of
	/// <see cref="Db4objects.Db4o.QCon">Db4objects.Db4o.QCon</see>
	/// during query evaluation.
	/// The query work (adding and removing nodes) happens here.
	/// Candidates during query evaluation.
	/// <see cref="Db4objects.Db4o.QCandidate">Db4objects.Db4o.QCandidate</see>
	/// objects are stored in i_root
	/// </summary>
	/// <exclude></exclude>
	public sealed class QCandidates : Db4objects.Db4o.Foundation.IVisitor4
	{
		public readonly Db4objects.Db4o.Transaction i_trans;

		public Db4objects.Db4o.Foundation.Tree i_root;

		private Db4objects.Db4o.Foundation.List4 i_constraints;

		internal Db4objects.Db4o.YapClass i_yapClass;

		private Db4objects.Db4o.QField i_field;

		internal Db4objects.Db4o.QCon i_currentConstraint;

		internal Db4objects.Db4o.Foundation.Tree i_ordered;

		private int i_orderID;

		private Db4objects.Db4o.IDGenerator _idGenerator;

		internal QCandidates(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.YapClass
			 a_yapClass, Db4objects.Db4o.QField a_field)
		{
			i_trans = a_trans;
			i_yapClass = a_yapClass;
			i_field = a_field;
			if (a_field == null || a_field.i_yapField == null || !(a_field.i_yapField.GetHandler
				() is Db4objects.Db4o.YapClass))
			{
				return;
			}
			Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)a_field.i_yapField.GetHandler
				();
			if (i_yapClass == null)
			{
				i_yapClass = yc;
			}
			else
			{
				yc = i_yapClass.GetHigherOrCommonHierarchy(yc);
				if (yc != null)
				{
					i_yapClass = yc;
				}
			}
		}

		public Db4objects.Db4o.QCandidate AddByIdentity(Db4objects.Db4o.QCandidate candidate
			)
		{
			i_root = Db4objects.Db4o.Foundation.Tree.Add(i_root, candidate);
			if (candidate._size == 0)
			{
				return candidate.GetRoot();
			}
			return candidate;
		}

		internal void AddConstraint(Db4objects.Db4o.QCon a_constraint)
		{
			i_constraints = new Db4objects.Db4o.Foundation.List4(i_constraints, a_constraint);
		}

		internal void AddOrder(Db4objects.Db4o.QOrder a_order)
		{
			i_ordered = Db4objects.Db4o.Foundation.Tree.Add(i_ordered, a_order);
		}

		internal void ApplyOrdering(Db4objects.Db4o.Foundation.Tree a_ordered, int a_orderID
			)
		{
			if (a_ordered == null || i_root == null)
			{
				return;
			}
			if (a_orderID > 0)
			{
				a_orderID = -a_orderID;
			}
			bool major = (a_orderID - i_orderID) < 0;
			if (major)
			{
				i_orderID = a_orderID;
			}
			int[] placement = { 0 };
			i_root.Traverse(new _AnonymousInnerClass109(this, major, placement));
			placement[0] = 1;
			a_ordered.Traverse(new _AnonymousInnerClass118(this, placement, major));
			Db4objects.Db4o.Foundation.Collection4 col = new Db4objects.Db4o.Foundation.Collection4
				();
			i_root.Traverse(new _AnonymousInnerClass129(this, col));
			Db4objects.Db4o.Foundation.Tree newTree = null;
			System.Collections.IEnumerator i = col.GetEnumerator();
			while (i.MoveNext())
			{
				Db4objects.Db4o.QCandidate candidate = (Db4objects.Db4o.QCandidate)i.Current;
				candidate._preceding = null;
				candidate._subsequent = null;
				candidate._size = 1;
				newTree = Db4objects.Db4o.Foundation.Tree.Add(newTree, candidate);
			}
			i_root = newTree;
		}

		private sealed class _AnonymousInnerClass109 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass109(QCandidates _enclosing, bool major, int[] placement
				)
			{
				this._enclosing = _enclosing;
				this.major = major;
				this.placement = placement;
			}

			public void Visit(object a_object)
			{
				((Db4objects.Db4o.QCandidate)a_object).HintOrder(0, major);
				((Db4objects.Db4o.QCandidate)a_object).HintOrder(placement[0]++, !major);
			}

			private readonly QCandidates _enclosing;

			private readonly bool major;

			private readonly int[] placement;
		}

		private sealed class _AnonymousInnerClass118 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass118(QCandidates _enclosing, int[] placement, bool major
				)
			{
				this._enclosing = _enclosing;
				this.placement = placement;
				this.major = major;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.QOrder qo = (Db4objects.Db4o.QOrder)a_object;
				Db4objects.Db4o.QCandidate candidate = qo._candidate.GetRoot();
				candidate.HintOrder(placement[0]++, major);
			}

			private readonly QCandidates _enclosing;

			private readonly int[] placement;

			private readonly bool major;
		}

		private sealed class _AnonymousInnerClass129 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass129(QCandidates _enclosing, Db4objects.Db4o.Foundation.Collection4
				 col)
			{
				this._enclosing = _enclosing;
				this.col = col;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.QCandidate candidate = (Db4objects.Db4o.QCandidate)a_object;
				col.Add(candidate);
			}

			private readonly QCandidates _enclosing;

			private readonly Db4objects.Db4o.Foundation.Collection4 col;
		}

		internal void Collect(Db4objects.Db4o.QCandidates a_candidates)
		{
			System.Collections.IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				Db4objects.Db4o.QCon qCon = (Db4objects.Db4o.QCon)i.Current;
				SetCurrentConstraint(qCon);
				qCon.Collect(a_candidates);
			}
			SetCurrentConstraint(null);
		}

		internal void Execute()
		{
			Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult result = ProcessFieldIndexes
				();
			if (result.FoundIndex())
			{
				i_root = result.ToQCandidate(this);
			}
			else
			{
				LoadFromClassIndex();
			}
			Evaluate();
		}

		public System.Collections.IEnumerator ExecuteSnapshot(Db4objects.Db4o.Foundation.Collection4
			 executionPath)
		{
			Db4objects.Db4o.Foundation.IIntIterator4 indexIterator = new Db4objects.Db4o.Foundation.IntIterator4Adaptor
				(IterateIndex(ProcessFieldIndexes()));
			Db4objects.Db4o.Foundation.Tree idRoot = Db4objects.Db4o.TreeInt.AddAll(null, indexIterator
				);
			System.Collections.IEnumerator snapshotIterator = new Db4objects.Db4o.Foundation.TreeKeyIterator
				(idRoot);
			System.Collections.IEnumerator singleObjectQueryIterator = SingleObjectSodaProcessor
				(snapshotIterator);
			return MapIdsToExecutionPath(singleObjectQueryIterator, executionPath);
		}

		private System.Collections.IEnumerator SingleObjectSodaProcessor(System.Collections.IEnumerator
			 indexIterator)
		{
			return new _AnonymousInnerClass182(this, indexIterator);
		}

		private sealed class _AnonymousInnerClass182 : Db4objects.Db4o.Foundation.MappingIterator
		{
			public _AnonymousInnerClass182(QCandidates _enclosing, System.Collections.IEnumerator
				 baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override object Map(object current)
			{
				int id = ((int)current);
				Db4objects.Db4o.QCandidate candidate = new Db4objects.Db4o.QCandidate(this._enclosing
					, null, id, true);
				this._enclosing.i_root = candidate;
				this._enclosing.Evaluate();
				if (!candidate.Include())
				{
					return Db4objects.Db4o.Foundation.MappingIterator.SKIP;
				}
				return current;
			}

			private readonly QCandidates _enclosing;
		}

		public System.Collections.IEnumerator ExecuteLazy(Db4objects.Db4o.Foundation.Collection4
			 executionPath)
		{
			System.Collections.IEnumerator indexIterator = IterateIndex(ProcessFieldIndexes()
				);
			System.Collections.IEnumerator singleObjectQueryIterator = SingleObjectSodaProcessor
				(indexIterator);
			return MapIdsToExecutionPath(singleObjectQueryIterator, executionPath);
		}

		private System.Collections.IEnumerator IterateIndex(Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult
			 result)
		{
			if (result.NoMatch())
			{
				return Db4objects.Db4o.Foundation.Iterator4Impl.EMPTY;
			}
			if (result.FoundIndex())
			{
				return result.IterateIDs();
			}
			if (i_yapClass.IsPrimitive())
			{
				return Db4objects.Db4o.Foundation.Iterator4Impl.EMPTY;
			}
			return Db4objects.Db4o.Inside.Classindex.BTreeClassIndexStrategy.Iterate(i_yapClass
				, i_trans);
		}

		private System.Collections.IEnumerator MapIdsToExecutionPath(System.Collections.IEnumerator
			 singleObjectQueryIterator, Db4objects.Db4o.Foundation.Collection4 executionPath
			)
		{
			if (executionPath == null)
			{
				return singleObjectQueryIterator;
			}
			System.Collections.IEnumerator res = singleObjectQueryIterator;
			System.Collections.IEnumerator executionPathIterator = executionPath.GetEnumerator
				();
			while (executionPathIterator.MoveNext())
			{
				string fieldName = (string)executionPathIterator.Current;
				System.Collections.IEnumerator mapIdToFieldIdsIterator = new _AnonymousInnerClass228
					(this, fieldName, res);
				res = new Db4objects.Db4o.Foundation.CompositeIterator4(mapIdToFieldIdsIterator);
			}
			return res;
		}

		private sealed class _AnonymousInnerClass228 : Db4objects.Db4o.Foundation.MappingIterator
		{
			public _AnonymousInnerClass228(QCandidates _enclosing, string fieldName, System.Collections.IEnumerator
				 baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
				this.fieldName = fieldName;
			}

			protected override object Map(object current)
			{
				int id = ((int)current);
				Db4objects.Db4o.YapWriter reader = this._enclosing.Stream().ReadWriterByID(this._enclosing
					.i_trans, id);
				if (reader == null)
				{
					return Db4objects.Db4o.Foundation.MappingIterator.SKIP;
				}
				Db4objects.Db4o.Inside.Marshall.ObjectHeader oh = new Db4objects.Db4o.Inside.Marshall.ObjectHeader
					(this._enclosing.Stream(), reader);
				Db4objects.Db4o.Foundation.Tree idTree = oh.YapClass().CollectFieldIDs(oh._marshallerFamily
					, oh._headerAttributes, null, reader, fieldName);
				return new Db4objects.Db4o.Foundation.TreeKeyIterator(idTree);
			}

			private readonly QCandidates _enclosing;

			private readonly string fieldName;
		}

		public Db4objects.Db4o.YapStream Stream()
		{
			return i_trans.Stream();
		}

		public int ClassIndexEntryCount()
		{
			return i_yapClass.IndexEntryCount(i_trans);
		}

		private Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult ProcessFieldIndexes
			()
		{
			if (i_constraints == null)
			{
				return Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult.NO_INDEX_FOUND;
			}
			return new Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessor(this).Run();
		}

		internal void Evaluate()
		{
			if (i_constraints == null)
			{
				return;
			}
			System.Collections.IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				Db4objects.Db4o.QCon qCon = (Db4objects.Db4o.QCon)i.Current;
				qCon.SetCandidates(this);
				qCon.EvaluateSelf();
			}
			i = IterateConstraints();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCon)i.Current).EvaluateSimpleChildren();
			}
			i = IterateConstraints();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCon)i.Current).EvaluateEvaluations();
			}
			i = IterateConstraints();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCon)i.Current).EvaluateCreateChildrenCandidates();
			}
			i = IterateConstraints();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCon)i.Current).EvaluateCollectChildren();
			}
			i = IterateConstraints();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCon)i.Current).EvaluateChildren();
			}
		}

		internal bool IsEmpty()
		{
			bool[] ret = new bool[] { true };
			Traverse(new _AnonymousInnerClass313(this, ret));
			return ret[0];
		}

		private sealed class _AnonymousInnerClass313 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass313(QCandidates _enclosing, bool[] ret)
			{
				this._enclosing = _enclosing;
				this.ret = ret;
			}

			public void Visit(object obj)
			{
				if (((Db4objects.Db4o.QCandidate)obj)._include)
				{
					ret[0] = false;
				}
			}

			private readonly QCandidates _enclosing;

			private readonly bool[] ret;
		}

		internal bool Filter(Db4objects.Db4o.Foundation.IVisitor4 a_host)
		{
			if (i_root != null)
			{
				i_root.Traverse(a_host);
				i_root = i_root.Filter(new _AnonymousInnerClass326(this));
			}
			return i_root != null;
		}

		private sealed class _AnonymousInnerClass326 : Db4objects.Db4o.Foundation.IPredicate4
		{
			public _AnonymousInnerClass326(QCandidates _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Match(object a_candidate)
			{
				return ((Db4objects.Db4o.QCandidate)a_candidate)._include;
			}

			private readonly QCandidates _enclosing;
		}

		internal int GenerateCandidateId()
		{
			if (_idGenerator == null)
			{
				_idGenerator = new Db4objects.Db4o.IDGenerator();
			}
			return -_idGenerator.Next();
		}

		public System.Collections.IEnumerator IterateConstraints()
		{
			if (i_constraints == null)
			{
				return Db4objects.Db4o.Foundation.Iterator4Impl.EMPTY;
			}
			return new Db4objects.Db4o.Foundation.Iterator4Impl(i_constraints);
		}

		internal sealed class TreeIntBuilder
		{
			public Db4objects.Db4o.TreeInt tree;

			public void Add(Db4objects.Db4o.TreeInt node)
			{
				tree = (Db4objects.Db4o.TreeInt)Db4objects.Db4o.Foundation.Tree.Add(tree, node);
			}
		}

		internal void LoadFromClassIndex()
		{
			if (!IsEmpty())
			{
				return;
			}
			Db4objects.Db4o.QCandidates.TreeIntBuilder result = new Db4objects.Db4o.QCandidates.TreeIntBuilder
				();
			Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy index = i_yapClass.Index();
			index.TraverseAll(i_trans, new _AnonymousInnerClass365(this, result));
			i_root = result.tree;
			Db4objects.Db4o.Inside.Diagnostic.DiagnosticProcessor dp = i_trans.Stream().i_handlers
				._diagnosticProcessor;
			if (dp.Enabled())
			{
				dp.LoadedFromClassIndex(i_yapClass);
			}
		}

		private sealed class _AnonymousInnerClass365 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass365(QCandidates _enclosing, Db4objects.Db4o.QCandidates.TreeIntBuilder
				 result)
			{
				this._enclosing = _enclosing;
				this.result = result;
			}

			public void Visit(object obj)
			{
				result.Add(new Db4objects.Db4o.QCandidate(this._enclosing, null, ((int)obj), true
					));
			}

			private readonly QCandidates _enclosing;

			private readonly Db4objects.Db4o.QCandidates.TreeIntBuilder result;
		}

		internal void SetCurrentConstraint(Db4objects.Db4o.QCon a_constraint)
		{
			i_currentConstraint = a_constraint;
		}

		internal void Traverse(Db4objects.Db4o.Foundation.IVisitor4 a_visitor)
		{
			if (i_root != null)
			{
				i_root.Traverse(a_visitor);
			}
		}

		internal bool TryAddConstraint(Db4objects.Db4o.QCon a_constraint)
		{
			if (i_field != null)
			{
				Db4objects.Db4o.QField qf = a_constraint.GetField();
				if (qf != null)
				{
					if (i_field.i_name != qf.i_name)
					{
						return false;
					}
				}
			}
			if (i_yapClass == null || a_constraint.IsNullConstraint())
			{
				AddConstraint(a_constraint);
				return true;
			}
			Db4objects.Db4o.YapClass yc = a_constraint.GetYapClass();
			if (yc != null)
			{
				yc = i_yapClass.GetHigherOrCommonHierarchy(yc);
				if (yc != null)
				{
					i_yapClass = yc;
					AddConstraint(a_constraint);
					return true;
				}
			}
			return false;
		}

		public void Visit(object a_tree)
		{
			Db4objects.Db4o.QCandidate parent = (Db4objects.Db4o.QCandidate)a_tree;
			if (parent.CreateChild(this))
			{
				return;
			}
			System.Collections.IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.QCon)i.Current).VisitOnNull(parent.GetRoot());
			}
		}
	}
}
