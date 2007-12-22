/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Callbacks;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Query.Result;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <summary>QQuery is the users hook on our graph.</summary>
	/// <remarks>
	/// QQuery is the users hook on our graph.
	/// A QQuery is defined by it's constraints.
	/// NOTE: This is just a 'partial' base class to allow for variant implementations
	/// in db4oj and db4ojdk1.2. It assumes that itself is an instance of QQuery
	/// and should never be used explicitly.
	/// </remarks>
	/// <exclude></exclude>
	public abstract class QQueryBase : IUnversioned
	{
		[System.NonSerialized]
		private static readonly IDGenerator i_orderingGenerator = new IDGenerator();

		[System.NonSerialized]
		internal Transaction _trans;

		public Collection4 i_constraints = new Collection4();

		public QQuery i_parent;

		public string i_field;

		[System.NonSerialized]
		private QueryEvaluationMode _evaluationMode;

		public int _evaluationModeAsInt;

		public IQueryComparator _comparator;

		[System.NonSerialized]
		private readonly QQuery _this;

		protected QQueryBase()
		{
			_this = Cast(this);
		}

		protected QQueryBase(Transaction a_trans, QQuery a_parent, string a_field)
		{
			_this = Cast(this);
			_trans = a_trans;
			i_parent = a_parent;
			i_field = a_field;
		}

		internal virtual void AddConstraint(QCon a_constraint)
		{
			i_constraints.Add(a_constraint);
		}

		private void AddConstraint(Collection4 col, object obj)
		{
			if (AttachToExistingConstraints(col, obj, true))
			{
				return;
			}
			if (AttachToExistingConstraints(col, obj, false))
			{
				return;
			}
			QConObject newConstraint = new QConObject(_trans, null, null, obj);
			AddConstraint(newConstraint);
			col.Add(newConstraint);
		}

		private bool AttachToExistingConstraints(Collection4 col, object obj, bool onlyForPaths
			)
		{
			bool found = false;
			IEnumerator j = IterateConstraints();
			while (j.MoveNext())
			{
				QCon existingConstraint = (QCon)j.Current;
				bool[] removeExisting = new bool[] { false };
				if (!onlyForPaths || (existingConstraint is QConPath))
				{
					QCon newConstraint = existingConstraint.ShareParent(obj, removeExisting);
					if (newConstraint != null)
					{
						AddConstraint(newConstraint);
						col.Add(newConstraint);
						if (removeExisting[0])
						{
							RemoveConstraint(existingConstraint);
						}
						found = true;
						if (!onlyForPaths)
						{
							return true;
						}
					}
				}
			}
			return found;
		}

		/// <summary>Search for slot that corresponds to class.</summary>
		/// <remarks>
		/// Search for slot that corresponds to class. <br />If not found add it.
		/// <br />Constrain it. <br />
		/// </remarks>
		public virtual IConstraint Constrain(object example)
		{
			lock (StreamLock())
			{
				example = Platform4.GetClassForType(example);
				IReflectClass claxx = ReflectClassForClass(example);
				if (claxx != null)
				{
					return AddClassConstraint(claxx);
				}
				QConEvaluation eval = Platform4.EvaluationCreate(_trans, example);
				if (eval != null)
				{
					return AddEvaluationToAllConstraints(eval);
				}
				Collection4 constraints = new Collection4();
				AddConstraint(constraints, example);
				return ToConstraint(constraints);
			}
		}

		private IConstraint AddEvaluationToAllConstraints(QConEvaluation eval)
		{
			if (i_constraints.Size() == 0)
			{
				_trans.Container().ClassCollection().IterateTopLevelClasses(new _IVisitor4_128(this
					));
			}
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				((QCon)i.Current).AddConstraint(eval);
			}
			return null;
		}

		private sealed class _IVisitor4_128 : IVisitor4
		{
			public _IVisitor4_128(QQueryBase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				ClassMetadata classMetadata = (ClassMetadata)obj;
				QConClass qcc = new QConClass(this._enclosing._trans, classMetadata.ClassReflector
					());
				this._enclosing.AddConstraint(qcc);
			}

			private readonly QQueryBase _enclosing;
		}

		private IConstraint AddClassConstraint(IReflectClass claxx)
		{
			if (claxx.Equals(Stream()._handlers.ICLASS_OBJECT))
			{
				return null;
			}
			Collection4 col = new Collection4();
			if (claxx.IsInterface())
			{
				return AddInterfaceConstraint(claxx);
			}
			IEnumerator constraintsIterator = IterateConstraints();
			while (constraintsIterator.MoveNext())
			{
				QCon existingConstraint = (QConObject)constraintsIterator.Current;
				bool[] removeExisting = new bool[] { false };
				QCon newConstraint = existingConstraint.ShareParentForClass(claxx, removeExisting
					);
				if (newConstraint != null)
				{
					AddConstraint(newConstraint);
					col.Add(newConstraint);
					if (removeExisting[0])
					{
						RemoveConstraint(existingConstraint);
					}
				}
			}
			if (col.Size() == 0)
			{
				QConClass qcc = new QConClass(_trans, claxx);
				AddConstraint(qcc);
				return qcc;
			}
			return ToConstraint(col);
		}

		private IConstraint AddInterfaceConstraint(IReflectClass claxx)
		{
			Collection4 classes = Stream().ClassCollection().ForInterface(claxx);
			if (classes.Size() == 0)
			{
				QConClass qcc = new QConClass(_trans, null, null, claxx);
				AddConstraint(qcc);
				return qcc;
			}
			IEnumerator i = classes.GetEnumerator();
			IConstraint constr = null;
			while (i.MoveNext())
			{
				ClassMetadata yapClass = (ClassMetadata)i.Current;
				IReflectClass yapClassClaxx = yapClass.ClassReflector();
				if (yapClassClaxx != null)
				{
					if (!yapClassClaxx.IsInterface())
					{
						if (constr == null)
						{
							constr = Constrain(yapClassClaxx);
						}
						else
						{
							constr = constr.Or(Constrain(yapClass.ClassReflector()));
						}
					}
				}
			}
			return constr;
		}

		private IReflectClass ReflectClassForClass(object example)
		{
			if (example is IReflectClass)
			{
				return (IReflectClass)example;
			}
			if (example is Type)
			{
				return _trans.Reflector().ForClass((Type)example);
			}
			return null;
		}

		public virtual IConstraints Constraints()
		{
			lock (StreamLock())
			{
				IConstraint[] constraints = new IConstraint[i_constraints.Size()];
				i_constraints.ToArray(constraints);
				return new QConstraints(_trans, constraints);
			}
		}

		public virtual IQuery Descend(string a_field)
		{
			lock (StreamLock())
			{
				QQuery query = new QQuery(_trans, _this, a_field);
				int[] run = new int[] { 1 };
				if (!Descend1(query, a_field, run))
				{
					if (run[0] == 1)
					{
						run[0] = 2;
						if (!Descend1(query, a_field, run))
						{
							new QConUnconditional(_trans, false).Attach(query, a_field);
						}
					}
				}
				return query;
			}
		}

		private bool Descend1(QQuery query, string a_field, int[] run)
		{
			bool[] foundClass = new bool[] { false };
			if (run[0] == 2 || i_constraints.Size() == 0)
			{
				run[0] = 0;
				bool[] anyClassCollected = new bool[] { false };
				Stream().ClassCollection().AttachQueryNode(a_field, new _IVisitor4_257(this, anyClassCollected
					));
			}
			LoadConstraints();
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				if (((QCon)i.Current).Attach(query, a_field))
				{
					foundClass[0] = true;
				}
			}
			return foundClass[0];
		}

		private sealed class _IVisitor4_257 : IVisitor4
		{
			public _IVisitor4_257(QQueryBase _enclosing, bool[] anyClassCollected)
			{
				this._enclosing = _enclosing;
				this.anyClassCollected = anyClassCollected;
			}

			public void Visit(object obj)
			{
				object[] pair = ((object[])obj);
				ClassMetadata parentYc = (ClassMetadata)pair[0];
				FieldMetadata yf = (FieldMetadata)pair[1];
				ClassMetadata childYc = yf.HandlerClassMetadata(this._enclosing.Stream());
				bool take = true;
				if (childYc is UntypedFieldHandler)
				{
					if (anyClassCollected[0])
					{
						take = false;
					}
					else
					{
						anyClassCollected[0] = true;
					}
				}
				if (take)
				{
					QConClass qcc = new QConClass(this._enclosing._trans, null, yf.QField(this._enclosing
						._trans), parentYc.ClassReflector());
					this._enclosing.AddConstraint(qcc);
				}
			}

			private readonly QQueryBase _enclosing;

			private readonly bool[] anyClassCollected;
		}

		public virtual IObjectSet Execute()
		{
			lock (StreamLock())
			{
				ICallbacks callbacks = Stream().Callbacks();
				callbacks.QueryOnStarted(_trans, Cast(this));
				IQueryResult qresult = GetQueryResult();
				callbacks.QueryOnFinished(_trans, Cast(this));
				return new ObjectSetFacade(qresult);
			}
		}

		public virtual IQueryResult GetQueryResult()
		{
			lock (StreamLock())
			{
				if (i_constraints.Size() == 0)
				{
					return Stream().GetAll(_trans);
				}
				IQueryResult result = ClassOnlyQuery();
				if (result != null)
				{
					return result;
				}
				return Stream().ExecuteQuery(_this);
			}
		}

		protected virtual ObjectContainerBase Stream()
		{
			return _trans.Container();
		}

		private IQueryResult ClassOnlyQuery()
		{
			if (i_constraints.Size() != 1 || _comparator != null)
			{
				return null;
			}
			IConstraint constr = SingleConstraint();
			if (constr.GetType() != typeof(QConClass))
			{
				return null;
			}
			QConClass clazzconstr = (QConClass)constr;
			ClassMetadata clazz = clazzconstr.i_yapClass;
			if (clazz == null)
			{
				return null;
			}
			if (clazzconstr.HasChildren() || clazz.IsArray())
			{
				return null;
			}
			IQueryResult queryResult = Stream().ClassOnlyQuery(_trans, clazz);
			if (queryResult == null)
			{
				return null;
			}
			Sort(queryResult);
			return queryResult;
		}

		private IConstraint SingleConstraint()
		{
			return (IConstraint)i_constraints.SingleElement();
		}

		public class CreateCandidateCollectionResult
		{
			public readonly bool checkDuplicates;

			public readonly bool topLevel;

			public readonly List4 candidateCollection;

			public CreateCandidateCollectionResult(List4 candidateCollection_, bool checkDuplicates_
				, bool topLevel_)
			{
				candidateCollection = candidateCollection_;
				topLevel = topLevel_;
				checkDuplicates = checkDuplicates_;
			}
		}

		public virtual IEnumerator ExecuteSnapshot()
		{
			QQueryBase.CreateCandidateCollectionResult r = CreateCandidateCollection();
			Collection4 executionPath = ExecutionPath(r);
			IEnumerator candidatesIterator = new Iterator4Impl(r.candidateCollection);
			Collection4 snapshots = new Collection4();
			while (candidatesIterator.MoveNext())
			{
				QCandidates candidates = (QCandidates)candidatesIterator.Current;
				snapshots.Add(candidates.ExecuteSnapshot(executionPath));
			}
			IEnumerator snapshotsIterator = snapshots.GetEnumerator();
			CompositeIterator4 resultingIDs = new CompositeIterator4(snapshotsIterator);
			if (!r.checkDuplicates)
			{
				return resultingIDs;
			}
			return CheckDuplicates(resultingIDs);
		}

		public virtual IEnumerator ExecuteLazy()
		{
			LoadConstraints();
			QQueryBase.CreateCandidateCollectionResult r = CreateCandidateCollection();
			Collection4 executionPath = ExecutionPath(r);
			IEnumerator candidateCollection = new Iterator4Impl(r.candidateCollection);
			MappingIterator executeCandidates = new _MappingIterator_408(this, executionPath, 
				candidateCollection);
			CompositeIterator4 resultingIDs = new CompositeIterator4(executeCandidates);
			if (!r.checkDuplicates)
			{
				return resultingIDs;
			}
			return CheckDuplicates(resultingIDs);
		}

		private sealed class _MappingIterator_408 : MappingIterator
		{
			public _MappingIterator_408(QQueryBase _enclosing, Collection4 executionPath, IEnumerator
				 baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
				this.executionPath = executionPath;
			}

			protected override object Map(object current)
			{
				return ((QCandidates)current).ExecuteLazy(executionPath);
			}

			private readonly QQueryBase _enclosing;

			private readonly Collection4 executionPath;
		}

		private MappingIterator CheckDuplicates(CompositeIterator4 executeAllCandidates)
		{
			return new _MappingIterator_424(this, executeAllCandidates);
		}

		private sealed class _MappingIterator_424 : MappingIterator
		{
			public _MappingIterator_424(QQueryBase _enclosing, CompositeIterator4 baseArg1) : 
				base(baseArg1)
			{
				this._enclosing = _enclosing;
				this.ids = new TreeInt(0);
			}

			private TreeInt ids;

			protected override object Map(object current)
			{
				int id = ((int)current);
				if (this.ids.Find(id) != null)
				{
					return MappingIterator.SKIP;
				}
				this.ids = (TreeInt)this.ids.Add(new TreeInt(id));
				return current;
			}

			private readonly QQueryBase _enclosing;
		}

		private Collection4 ExecutionPath(QQueryBase.CreateCandidateCollectionResult r)
		{
			return r.topLevel ? null : FieldPathFromTop();
		}

		private void LoadConstraints()
		{
			IEnumerator constraints = IterateConstraints();
			while (constraints.MoveNext())
			{
				((QConObject)constraints.Current).ByIdentity();
			}
		}

		public virtual void ExecuteLocal(IdListQueryResult result)
		{
			LoadConstraints();
			QQueryBase.CreateCandidateCollectionResult r = CreateCandidateCollection();
			bool checkDuplicates = r.checkDuplicates;
			bool topLevel = r.topLevel;
			List4 candidateCollection = r.candidateCollection;
			if (candidateCollection != null)
			{
				Collection4 executionPath = topLevel ? null : FieldPathFromTop();
				IEnumerator i = new Iterator4Impl(candidateCollection);
				while (i.MoveNext())
				{
					((QCandidates)i.Current).Execute();
				}
				if (candidateCollection._next != null)
				{
					checkDuplicates = true;
				}
				if (checkDuplicates)
				{
					result.CheckDuplicates();
				}
				ObjectContainerBase stream = Stream();
				i = new Iterator4Impl(candidateCollection);
				while (i.MoveNext())
				{
					QCandidates candidates = (QCandidates)i.Current;
					if (topLevel)
					{
						candidates.Traverse(result);
					}
					else
					{
						candidates.Traverse(new _IVisitor4_489(this, executionPath, stream, result));
					}
				}
			}
			Sort(result);
		}

		private sealed class _IVisitor4_489 : IVisitor4
		{
			public _IVisitor4_489(QQueryBase _enclosing, Collection4 executionPath, ObjectContainerBase
				 stream, IdListQueryResult result)
			{
				this._enclosing = _enclosing;
				this.executionPath = executionPath;
				this.stream = stream;
				this.result = result;
			}

			public void Visit(object a_object)
			{
				QCandidate candidate = (QCandidate)a_object;
				if (candidate.Include())
				{
					TreeInt ids = new TreeInt(candidate._key);
					TreeInt[] idsNew = new TreeInt[1];
					IEnumerator itPath = executionPath.GetEnumerator();
					while (itPath.MoveNext())
					{
						idsNew[0] = null;
						string fieldName = (string)(itPath.Current);
						if (ids != null)
						{
							ids.Traverse(new _IVisitor4_500(this, stream, idsNew, fieldName));
						}
						ids = idsNew[0];
					}
					if (ids != null)
					{
						ids.Traverse(new _IVisitor4_520(this, result));
					}
				}
			}

			private sealed class _IVisitor4_500 : IVisitor4
			{
				public _IVisitor4_500(_IVisitor4_489 _enclosing, ObjectContainerBase stream, TreeInt
					[] idsNew, string fieldName)
				{
					this._enclosing = _enclosing;
					this.stream = stream;
					this.idsNew = idsNew;
					this.fieldName = fieldName;
				}

				public void Visit(object treeInt)
				{
					int id = ((TreeInt)treeInt)._key;
					StatefulBuffer reader = stream.ReadWriterByID(this._enclosing._enclosing._trans, 
						id);
					if (reader != null)
					{
						ObjectHeader oh = new ObjectHeader(stream, reader);
						idsNew[0] = oh.ClassMetadata().CollectFieldIDs(oh._marshallerFamily, oh._headerAttributes
							, idsNew[0], reader, fieldName);
					}
				}

				private readonly _IVisitor4_489 _enclosing;

				private readonly ObjectContainerBase stream;

				private readonly TreeInt[] idsNew;

				private readonly string fieldName;
			}

			private sealed class _IVisitor4_520 : IVisitor4
			{
				public _IVisitor4_520(_IVisitor4_489 _enclosing, IdListQueryResult result)
				{
					this._enclosing = _enclosing;
					this.result = result;
				}

				public void Visit(object treeInt)
				{
					result.AddKeyCheckDuplicates(((TreeInt)treeInt)._key);
				}

				private readonly _IVisitor4_489 _enclosing;

				private readonly IdListQueryResult result;
			}

			private readonly QQueryBase _enclosing;

			private readonly Collection4 executionPath;

			private readonly ObjectContainerBase stream;

			private readonly IdListQueryResult result;
		}

		private Collection4 FieldPathFromTop()
		{
			QQueryBase q = this;
			Collection4 fieldPath = new Collection4();
			while (q.i_parent != null)
			{
				fieldPath.Prepend(q.i_field);
				q = q.i_parent;
			}
			return fieldPath;
		}

		private void LogConstraints()
		{
		}

		public virtual QQueryBase.CreateCandidateCollectionResult CreateCandidateCollection
			()
		{
			bool checkDuplicates = false;
			bool topLevel = true;
			List4 candidateCollection = null;
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				QCon qcon = (QCon)i.Current;
				QCon old = qcon;
				qcon = qcon.GetRoot();
				if (qcon != old)
				{
					checkDuplicates = true;
					topLevel = false;
				}
				ClassMetadata yc = qcon.GetYapClass();
				if (yc == null)
				{
					break;
				}
				candidateCollection = AddConstraintToCandidateCollection(candidateCollection, qcon
					);
			}
			return new QQueryBase.CreateCandidateCollectionResult(candidateCollection, checkDuplicates
				, topLevel);
		}

		private List4 AddConstraintToCandidateCollection(List4 candidateCollection, QCon 
			qcon)
		{
			if (candidateCollection != null)
			{
				if (TryToAddToExistingCandidate(candidateCollection, qcon))
				{
					return candidateCollection;
				}
			}
			QCandidates candidates = new QCandidates((LocalTransaction)_trans, qcon.GetYapClass
				(), null);
			candidates.AddConstraint(qcon);
			return new List4(candidateCollection, candidates);
		}

		private bool TryToAddToExistingCandidate(List4 candidateCollection, QCon qcon)
		{
			IEnumerator j = new Iterator4Impl(candidateCollection);
			while (j.MoveNext())
			{
				QCandidates candidates = (QCandidates)j.Current;
				if (candidates.TryAddConstraint(qcon))
				{
					return true;
				}
			}
			return false;
		}

		public Transaction GetTransaction()
		{
			return _trans;
		}

		internal virtual IEnumerator IterateConstraints()
		{
			return new Collection4(i_constraints).GetEnumerator();
		}

		public virtual IQuery OrderAscending()
		{
			lock (StreamLock())
			{
				SetOrdering(i_orderingGenerator.Next());
				return _this;
			}
		}

		public virtual IQuery OrderDescending()
		{
			lock (StreamLock())
			{
				SetOrdering(-i_orderingGenerator.Next());
				return _this;
			}
		}

		private void SetOrdering(int ordering)
		{
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				((QCon)i.Current).SetOrdering(ordering);
			}
		}

		public virtual void Marshall()
		{
			LoadConstraints();
			_evaluationModeAsInt = _evaluationMode.AsInt();
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				((QCon)i.Current).GetRoot().Marshall();
			}
		}

		public virtual void Unmarshall(Transaction a_trans)
		{
			_evaluationMode = QueryEvaluationMode.FromInt(_evaluationModeAsInt);
			_trans = a_trans;
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				((QCon)i.Current).Unmarshall(a_trans);
			}
		}

		internal virtual void RemoveConstraint(QCon a_constraint)
		{
			i_constraints.Remove(a_constraint);
		}

		internal virtual IConstraint ToConstraint(Collection4 constraints)
		{
			if (constraints.Size() == 1)
			{
				return (IConstraint)constraints.SingleElement();
			}
			else
			{
				if (constraints.Size() > 0)
				{
					IConstraint[] constraintArray = new IConstraint[constraints.Size()];
					constraints.ToArray(constraintArray);
					return new QConstraints(_trans, constraintArray);
				}
			}
			return null;
		}

		protected virtual object StreamLock()
		{
			return Stream()._lock;
		}

		public virtual IQuery SortBy(IQueryComparator comparator)
		{
			_comparator = comparator;
			return _this;
		}

		private void Sort(IQueryResult result)
		{
			if (_comparator != null)
			{
				result.Sort(_comparator);
			}
		}

		private static QQuery Cast(QQueryBase obj)
		{
			return (QQuery)obj;
		}

		public virtual bool RequiresSort()
		{
			if (_comparator != null)
			{
				return true;
			}
			IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				QCon qCon = (QCon)i.Current;
				if (qCon.RequiresSort())
				{
					return true;
				}
			}
			return false;
		}

		public virtual IQueryComparator Comparator()
		{
			return _comparator;
		}

		public virtual QueryEvaluationMode EvaluationMode()
		{
			return _evaluationMode;
		}

		public virtual void EvaluationMode(QueryEvaluationMode mode)
		{
			_evaluationMode = mode;
		}
	}
}
