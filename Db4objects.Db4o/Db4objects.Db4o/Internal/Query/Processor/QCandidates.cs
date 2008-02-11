/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Internal.Fieldindex;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <summary>
	/// Holds the tree of
	/// <see cref="QCandidate">QCandidate</see>
	/// objects and the list of
	/// <see cref="QCon">QCon</see>
	/// during query evaluation.
	/// The query work (adding and removing nodes) happens here.
	/// Candidates during query evaluation.
	/// <see cref="QCandidate">QCandidate</see>
	/// objects are stored in i_root
	/// </summary>
	/// <exclude></exclude>
	public sealed class QCandidates : IVisitor4
	{
		public readonly LocalTransaction i_trans;

		public Tree i_root;

		private List4 i_constraints;

		internal ClassMetadata i_yapClass;

		private QField i_field;

		internal QCon i_currentConstraint;

		internal Tree i_ordered;

		private int _majorOrderingID;

		private IDGenerator _idGenerator;

		private bool _loadedFromClassIndex;

		internal QCandidates(LocalTransaction a_trans, ClassMetadata a_yapClass, QField a_field
			)
		{
			// Transaction necessary as reference to stream
			// root of the QCandidate tree
			// collection of all constraints
			// possible class information
			// possible field information
			// current executing constraint, only set where needed
			// QOrder tree
			// 
			i_trans = a_trans;
			i_yapClass = a_yapClass;
			i_field = a_field;
			if (a_field == null || a_field.i_yapField == null || !(a_field.i_yapField.GetHandler
				() is ClassMetadata))
			{
				return;
			}
			ClassMetadata yc = (ClassMetadata)a_field.i_yapField.GetHandler();
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

		public QCandidate AddByIdentity(QCandidate candidate)
		{
			i_root = Tree.Add(i_root, candidate);
			if (candidate._size == 0)
			{
				// This means that the candidate was already present
				// and QCandidate does not allow duplicates.
				// In this case QCandidate#isDuplicateOf will have
				// placed the existing QCandidate in the i_root
				// variable of the new candidate. We return it here: 
				return candidate.GetRoot();
			}
			return candidate;
		}

		internal void AddConstraint(QCon a_constraint)
		{
			i_constraints = new List4(i_constraints, a_constraint);
		}

		internal void AddOrder(QOrder a_order)
		{
			i_ordered = Tree.Add(i_ordered, a_order);
		}

		internal void ApplyOrdering(Tree orderedCandidates, int orderingID)
		{
			if (orderedCandidates == null || i_root == null)
			{
				return;
			}
			int absoluteOrderingID = Math.Abs(orderingID);
			bool major = TreatOrderingIDAsMajor(absoluteOrderingID);
			if (major && !IsUnordered())
			{
				SwapMajorOrderToMinor();
			}
			HintNewOrder(orderedCandidates, major);
			i_root = RecreateTreeFromCandidates();
			if (major)
			{
				_majorOrderingID = absoluteOrderingID;
			}
		}

		public QCandidate ReadSubCandidate(QueryingReadContext context, ITypeHandler4 handler
			)
		{
			ObjectID objectID = ObjectID.NotPossible;
			try
			{
				int offset = context.Offset();
				if (handler is IReadsObjectIds)
				{
					objectID = ((IReadsObjectIds)handler).ReadObjectID(context);
				}
				if (objectID.IsValid())
				{
					return new QCandidate(this, null, objectID._id, true);
				}
				if (objectID == ObjectID.NotPossible)
				{
					context.Seek(offset);
					object obj = context.Read(handler);
					if (obj != null)
					{
						return new QCandidate(this, obj, 0, true);
					}
				}
			}
			catch (Exception)
			{
			}
			// FIXME: Catchall
			return null;
		}

		private Tree RecreateTreeFromCandidates()
		{
			Collection4 col = CollectCandidates();
			Tree newTree = null;
			IEnumerator i = col.GetEnumerator();
			while (i.MoveNext())
			{
				QCandidate candidate = (QCandidate)i.Current;
				candidate._preceding = null;
				candidate._subsequent = null;
				candidate._size = 1;
				newTree = Tree.Add(newTree, candidate);
			}
			return newTree;
		}

		private Collection4 CollectCandidates()
		{
			Collection4 col = new Collection4();
			i_root.Traverse(new _IVisitor4_166(this, col));
			return col;
		}

		private sealed class _IVisitor4_166 : IVisitor4
		{
			public _IVisitor4_166(QCandidates _enclosing, Collection4 col)
			{
				this._enclosing = _enclosing;
				this.col = col;
			}

			public void Visit(object a_object)
			{
				QCandidate candidate = (QCandidate)a_object;
				col.Add(candidate);
			}

			private readonly QCandidates _enclosing;

			private readonly Collection4 col;
		}

		private void HintNewOrder(Tree orderedCandidates, bool major)
		{
			int[] currentOrder = new int[] { 0 };
			QOrder[] lastOrder = new QOrder[] { null };
			orderedCandidates.Traverse(new _IVisitor4_179(this, lastOrder, currentOrder, major
				));
		}

		private sealed class _IVisitor4_179 : IVisitor4
		{
			public _IVisitor4_179(QCandidates _enclosing, QOrder[] lastOrder, int[] currentOrder
				, bool major)
			{
				this._enclosing = _enclosing;
				this.lastOrder = lastOrder;
				this.currentOrder = currentOrder;
				this.major = major;
			}

			public void Visit(object a_object)
			{
				QOrder qo = (QOrder)a_object;
				if (!qo.IsEqual(lastOrder[0]))
				{
					currentOrder[0]++;
				}
				QCandidate candidate = qo._candidate.GetRoot();
				candidate.HintOrder(currentOrder[0], major);
				lastOrder[0] = qo;
			}

			private readonly QCandidates _enclosing;

			private readonly QOrder[] lastOrder;

			private readonly int[] currentOrder;

			private readonly bool major;
		}

		private void SwapMajorOrderToMinor()
		{
			i_root.Traverse(new _IVisitor4_193(this));
		}

		private sealed class _IVisitor4_193 : IVisitor4
		{
			public _IVisitor4_193(QCandidates _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				QCandidate candidate = (QCandidate)obj;
				Order order = (Order)candidate._order;
				order.SwapMajorToMinor();
			}

			private readonly QCandidates _enclosing;
		}

		private bool TreatOrderingIDAsMajor(int absoluteOrderingID)
		{
			return (IsUnordered()) || (IsMoreRelevantOrderingID(absoluteOrderingID));
		}

		private bool IsUnordered()
		{
			return _majorOrderingID == 0;
		}

		private bool IsMoreRelevantOrderingID(int absoluteOrderingID)
		{
			return absoluteOrderingID < _majorOrderingID;
		}

		internal void Collect(Db4objects.Db4o.Internal.Query.Processor.QCandidates a_candidates
			)
		{
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				QCon qCon = (QCon)i.Current;
				SetCurrentConstraint(qCon);
				qCon.Collect(a_candidates);
			}
			SetCurrentConstraint(null);
		}

		internal void Execute()
		{
			if (DTrace.enabled)
			{
				DTrace.QueryProcess.Log();
			}
			FieldIndexProcessorResult result = ProcessFieldIndexes();
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

		public IEnumerator ExecuteSnapshot(Collection4 executionPath)
		{
			IIntIterator4 indexIterator = new IntIterator4Adaptor(IterateIndex(ProcessFieldIndexes
				()));
			Tree idRoot = TreeInt.AddAll(null, indexIterator);
			IEnumerator snapshotIterator = new TreeKeyIterator(idRoot);
			IEnumerator singleObjectQueryIterator = SingleObjectSodaProcessor(snapshotIterator
				);
			return MapIdsToExecutionPath(singleObjectQueryIterator, executionPath);
		}

		private IEnumerator SingleObjectSodaProcessor(IEnumerator indexIterator)
		{
			return new _MappingIterator_246(this, indexIterator);
		}

		private sealed class _MappingIterator_246 : MappingIterator
		{
			public _MappingIterator_246(QCandidates _enclosing, IEnumerator baseArg1) : base(
				baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override object Map(object current)
			{
				int id = ((int)current);
				QCandidate candidate = new QCandidate(this._enclosing, null, id, true);
				this._enclosing.i_root = candidate;
				this._enclosing.Evaluate();
				if (!candidate.Include())
				{
					return MappingIterator.Skip;
				}
				return current;
			}

			private readonly QCandidates _enclosing;
		}

		public IEnumerator ExecuteLazy(Collection4 executionPath)
		{
			IEnumerator indexIterator = IterateIndex(ProcessFieldIndexes());
			IEnumerator singleObjectQueryIterator = SingleObjectSodaProcessor(indexIterator);
			return MapIdsToExecutionPath(singleObjectQueryIterator, executionPath);
		}

		private IEnumerator IterateIndex(FieldIndexProcessorResult result)
		{
			if (result.NoMatch())
			{
				return Iterators.EmptyIterator;
			}
			if (result.FoundIndex())
			{
				return result.IterateIDs();
			}
			if (i_yapClass.IsPrimitive())
			{
				return Iterators.EmptyIterator;
			}
			return BTreeClassIndexStrategy.Iterate(i_yapClass, i_trans);
		}

		private IEnumerator MapIdsToExecutionPath(IEnumerator singleObjectQueryIterator, 
			Collection4 executionPath)
		{
			if (executionPath == null)
			{
				return singleObjectQueryIterator;
			}
			IEnumerator res = singleObjectQueryIterator;
			IEnumerator executionPathIterator = executionPath.GetEnumerator();
			while (executionPathIterator.MoveNext())
			{
				string fieldName = (string)executionPathIterator.Current;
				IEnumerator mapIdToFieldIdsIterator = new _MappingIterator_292(this, fieldName, res
					);
				res = new CompositeIterator4(mapIdToFieldIdsIterator);
			}
			return res;
		}

		private sealed class _MappingIterator_292 : MappingIterator
		{
			public _MappingIterator_292(QCandidates _enclosing, string fieldName, IEnumerator
				 baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
				this.fieldName = fieldName;
			}

			protected override object Map(object current)
			{
				int id = ((int)current);
				StatefulBuffer reader = this._enclosing.Stream().ReadWriterByID(this._enclosing.i_trans
					, id);
				if (reader == null)
				{
					return MappingIterator.Skip;
				}
				ObjectHeader oh = new ObjectHeader(this._enclosing.Stream(), reader);
				Tree idTree = oh.ClassMetadata().CollectFieldIDs(oh._marshallerFamily, oh._headerAttributes
					, null, reader, fieldName);
				return new TreeKeyIterator(idTree);
			}

			private readonly QCandidates _enclosing;

			private readonly string fieldName;
		}

		public ObjectContainerBase Stream()
		{
			return i_trans.Container();
		}

		public int ClassIndexEntryCount()
		{
			return i_yapClass.IndexEntryCount(i_trans);
		}

		private FieldIndexProcessorResult ProcessFieldIndexes()
		{
			if (i_constraints == null)
			{
				return FieldIndexProcessorResult.NoIndexFound;
			}
			return new FieldIndexProcessor(this).Run();
		}

		internal void Evaluate()
		{
			if (i_constraints == null)
			{
				return;
			}
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				QCon qCon = (QCon)i.Current;
				qCon.SetCandidates(this);
				qCon.EvaluateSelf();
			}
			i = IterateConstraints();
			while (i.MoveNext())
			{
				((QCon)i.Current).EvaluateSimpleChildren();
			}
			i = IterateConstraints();
			while (i.MoveNext())
			{
				((QCon)i.Current).EvaluateEvaluations();
			}
			i = IterateConstraints();
			while (i.MoveNext())
			{
				((QCon)i.Current).EvaluateCreateChildrenCandidates();
			}
			i = IterateConstraints();
			while (i.MoveNext())
			{
				((QCon)i.Current).EvaluateCollectChildren();
			}
			i = IterateConstraints();
			while (i.MoveNext())
			{
				((QCon)i.Current).EvaluateChildren();
			}
		}

		internal bool IsEmpty()
		{
			bool[] ret = new bool[] { true };
			Traverse(new _IVisitor4_377(this, ret));
			return ret[0];
		}

		private sealed class _IVisitor4_377 : IVisitor4
		{
			public _IVisitor4_377(QCandidates _enclosing, bool[] ret)
			{
				this._enclosing = _enclosing;
				this.ret = ret;
			}

			public void Visit(object obj)
			{
				if (((QCandidate)obj)._include)
				{
					ret[0] = false;
				}
			}

			private readonly QCandidates _enclosing;

			private readonly bool[] ret;
		}

		internal bool Filter(IVisitor4 a_host)
		{
			if (i_root != null)
			{
				i_root.Traverse(a_host);
				i_root = i_root.Filter(new _IPredicate4_390(this));
			}
			return i_root != null;
		}

		private sealed class _IPredicate4_390 : IPredicate4
		{
			public _IPredicate4_390(QCandidates _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Match(object a_candidate)
			{
				return ((QCandidate)a_candidate)._include;
			}

			private readonly QCandidates _enclosing;
		}

		internal int GenerateCandidateId()
		{
			if (_idGenerator == null)
			{
				_idGenerator = new IDGenerator();
			}
			return -_idGenerator.Next();
		}

		public IEnumerator IterateConstraints()
		{
			if (i_constraints == null)
			{
				return Iterators.EmptyIterator;
			}
			return new Iterator4Impl(i_constraints);
		}

		internal sealed class TreeIntBuilder
		{
			public TreeInt tree;

			public void Add(TreeInt node)
			{
				tree = (TreeInt)Tree.Add(tree, node);
			}
		}

		internal void LoadFromClassIndex()
		{
			if (!IsEmpty())
			{
				return;
			}
			QCandidates.TreeIntBuilder result = new QCandidates.TreeIntBuilder();
			IClassIndexStrategy index = i_yapClass.Index();
			index.TraverseAll(i_trans, new _IVisitor4_428(this, result));
			i_root = result.tree;
			DiagnosticProcessor dp = i_trans.Container()._handlers._diagnosticProcessor;
			if (dp.Enabled())
			{
				dp.LoadedFromClassIndex(i_yapClass);
			}
			_loadedFromClassIndex = true;
		}

		private sealed class _IVisitor4_428 : IVisitor4
		{
			public _IVisitor4_428(QCandidates _enclosing, QCandidates.TreeIntBuilder result)
			{
				this._enclosing = _enclosing;
				this.result = result;
			}

			public void Visit(object obj)
			{
				result.Add(new QCandidate(this._enclosing, null, ((int)obj), true));
			}

			private readonly QCandidates _enclosing;

			private readonly QCandidates.TreeIntBuilder result;
		}

		internal void SetCurrentConstraint(QCon a_constraint)
		{
			i_currentConstraint = a_constraint;
		}

		internal void Traverse(IVisitor4 a_visitor)
		{
			if (i_root != null)
			{
				i_root.Traverse(a_visitor);
			}
		}

		// FIXME: This method should go completely.
		//        We changed the code to create the QCandidates graph in two steps:
		//        (1) call fitsIntoExistingConstraintHierarchy to determine whether
		//            or not we need more QCandidates objects
		//        (2) add all constraints
		//        This method tries to do both in one, which results in missing
		//        constraints. Not all are added to all QCandiates.
		//        Right methodology is in 
		//        QQueryBase#createCandidateCollection
		//        and
		//        QQueryBase#createQCandidatesList
		internal bool TryAddConstraint(QCon a_constraint)
		{
			if (i_field != null)
			{
				QField qf = a_constraint.GetField();
				if (qf != null)
				{
					if (i_field.i_name != null && !i_field.i_name.Equals(qf.i_name))
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
			ClassMetadata yc = a_constraint.GetYapClass();
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
			AddConstraint(a_constraint);
			return false;
		}

		public void Visit(object a_tree)
		{
			QCandidate parent = (QCandidate)a_tree;
			if (parent.CreateChild(this))
			{
				return;
			}
			// No object found.
			// All children constraints are necessarily false.
			// Check immediately.
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				((QCon)i.Current).VisitOnNull(parent.GetRoot());
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			i_root.Traverse(new _IVisitor4_512(this, sb));
			return sb.ToString();
		}

		private sealed class _IVisitor4_512 : IVisitor4
		{
			public _IVisitor4_512(QCandidates _enclosing, StringBuilder sb)
			{
				this._enclosing = _enclosing;
				this.sb = sb;
			}

			public void Visit(object obj)
			{
				QCandidate candidate = (QCandidate)obj;
				sb.Append(" ");
				sb.Append(candidate._key);
			}

			private readonly QCandidates _enclosing;

			private readonly StringBuilder sb;
		}

		public void ClearOrdering()
		{
			i_ordered = null;
		}

		public Transaction Transaction()
		{
			return i_trans;
		}

		public bool WasLoadedFromClassIndex()
		{
			return _loadedFromClassIndex;
		}

		public bool FitsIntoExistingConstraintHierarchy(QCon constraint)
		{
			if (i_field != null)
			{
				QField qf = constraint.GetField();
				if (qf != null)
				{
					if (i_field.i_name != null && !i_field.i_name.Equals(qf.i_name))
					{
						return false;
					}
				}
			}
			if (i_yapClass == null || constraint.IsNullConstraint())
			{
				return true;
			}
			ClassMetadata classMetadata = constraint.GetYapClass();
			if (classMetadata == null)
			{
				return false;
			}
			classMetadata = i_yapClass.GetHigherOrCommonHierarchy(classMetadata);
			if (classMetadata == null)
			{
				return false;
			}
			i_yapClass = classMetadata;
			return true;
		}
	}
}
