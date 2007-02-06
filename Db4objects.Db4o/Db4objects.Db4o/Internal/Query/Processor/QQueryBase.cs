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
	public abstract class QQueryBase : Db4objects.Db4o.Types.IUnversioned
	{
		[System.NonSerialized]
		private static readonly Db4objects.Db4o.Internal.IDGenerator i_orderingGenerator = 
			new Db4objects.Db4o.Internal.IDGenerator();

		[System.NonSerialized]
		internal Db4objects.Db4o.Internal.Transaction i_trans;

		public Db4objects.Db4o.Foundation.Collection4 i_constraints = new Db4objects.Db4o.Foundation.Collection4
			();

		public Db4objects.Db4o.Internal.Query.Processor.QQuery i_parent;

		public string i_field;

		[System.NonSerialized]
		private Db4objects.Db4o.Config.QueryEvaluationMode _evaluationMode;

		public int _evaluationModeAsInt;

		public Db4objects.Db4o.Query.IQueryComparator _comparator;

		[System.NonSerialized]
		private readonly Db4objects.Db4o.Internal.Query.Processor.QQuery _this;

		protected QQueryBase()
		{
			_this = Cast(this);
		}

		protected QQueryBase(Db4objects.Db4o.Internal.Transaction a_trans, Db4objects.Db4o.Internal.Query.Processor.QQuery
			 a_parent, string a_field)
		{
			_this = Cast(this);
			i_trans = a_trans;
			i_parent = a_parent;
			i_field = a_field;
		}

		internal virtual void AddConstraint(Db4objects.Db4o.Internal.Query.Processor.QCon
			 a_constraint)
		{
			i_constraints.Add(a_constraint);
		}

		private void AddConstraint(Db4objects.Db4o.Foundation.Collection4 col, object obj
			)
		{
			if (AttachToExistingConstraints(col, obj, true))
			{
				return;
			}
			if (AttachToExistingConstraints(col, obj, false))
			{
				return;
			}
			Db4objects.Db4o.Internal.Query.Processor.QConObject newConstraint = new Db4objects.Db4o.Internal.Query.Processor.QConObject
				(i_trans, null, null, obj);
			AddConstraint(newConstraint);
			col.Add(newConstraint);
		}

		private bool AttachToExistingConstraints(Db4objects.Db4o.Foundation.Collection4 col
			, object obj, bool onlyForPaths)
		{
			bool found = false;
			System.Collections.IEnumerator j = IterateConstraints();
			while (j.MoveNext())
			{
				Db4objects.Db4o.Internal.Query.Processor.QCon existingConstraint = (Db4objects.Db4o.Internal.Query.Processor.QCon
					)j.Current;
				bool[] removeExisting = { false };
				if (!onlyForPaths || (existingConstraint is Db4objects.Db4o.Internal.Query.Processor.QConPath
					))
				{
					Db4objects.Db4o.Internal.Query.Processor.QCon newConstraint = existingConstraint.
						ShareParent(obj, removeExisting);
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
		public virtual Db4objects.Db4o.Query.IConstraint Constrain(object example)
		{
			lock (StreamLock())
			{
				example = Db4objects.Db4o.Internal.Platform4.GetClassForType(example);
				Db4objects.Db4o.Reflect.IReflectClass claxx = ReflectClassForClass(example);
				if (claxx != null)
				{
					return AddClassConstraint(claxx);
				}
				Db4objects.Db4o.Internal.Query.Processor.QConEvaluation eval = Db4objects.Db4o.Internal.Platform4
					.EvaluationCreate(i_trans, example);
				if (eval != null)
				{
					return AddEvaluationToAllConstraints(eval);
				}
				Db4objects.Db4o.Foundation.Collection4 constraints = new Db4objects.Db4o.Foundation.Collection4
					();
				AddConstraint(constraints, example);
				return ToConstraint(constraints);
			}
		}

		private Db4objects.Db4o.Query.IConstraint AddEvaluationToAllConstraints(Db4objects.Db4o.Internal.Query.Processor.QConEvaluation
			 eval)
		{
			System.Collections.IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).AddConstraint(eval);
			}
			return null;
		}

		private Db4objects.Db4o.Query.IConstraint AddClassConstraint(Db4objects.Db4o.Reflect.IReflectClass
			 claxx)
		{
			if (claxx.Equals(Stream().i_handlers.ICLASS_OBJECT))
			{
				return null;
			}
			Db4objects.Db4o.Foundation.Collection4 col = new Db4objects.Db4o.Foundation.Collection4
				();
			if (claxx.IsInterface())
			{
				return AddInterfaceConstraint(claxx);
			}
			System.Collections.IEnumerator constraintsIterator = IterateConstraints();
			while (constraintsIterator.MoveNext())
			{
				Db4objects.Db4o.Internal.Query.Processor.QCon existingConstraint = (Db4objects.Db4o.Internal.Query.Processor.QConObject
					)constraintsIterator.Current;
				bool[] removeExisting = { false };
				Db4objects.Db4o.Internal.Query.Processor.QCon newConstraint = existingConstraint.
					ShareParentForClass(claxx, removeExisting);
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
				Db4objects.Db4o.Internal.Query.Processor.QConClass qcc = new Db4objects.Db4o.Internal.Query.Processor.QConClass
					(i_trans, null, null, claxx);
				AddConstraint(qcc);
				return qcc;
			}
			return ToConstraint(col);
		}

		private Db4objects.Db4o.Query.IConstraint AddInterfaceConstraint(Db4objects.Db4o.Reflect.IReflectClass
			 claxx)
		{
			Db4objects.Db4o.Foundation.Collection4 classes = Stream().ClassCollection().ForInterface
				(claxx);
			if (classes.Size() == 0)
			{
				Db4objects.Db4o.Internal.Query.Processor.QConClass qcc = new Db4objects.Db4o.Internal.Query.Processor.QConClass
					(i_trans, null, null, claxx);
				AddConstraint(qcc);
				return qcc;
			}
			System.Collections.IEnumerator i = classes.GetEnumerator();
			Db4objects.Db4o.Query.IConstraint constr = null;
			while (i.MoveNext())
			{
				Db4objects.Db4o.Internal.ClassMetadata yapClass = (Db4objects.Db4o.Internal.ClassMetadata
					)i.Current;
				Db4objects.Db4o.Reflect.IReflectClass yapClassClaxx = yapClass.ClassReflector();
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

		private Db4objects.Db4o.Reflect.IReflectClass ReflectClassForClass(object example
			)
		{
			if (example is Db4objects.Db4o.Reflect.IReflectClass)
			{
				return (Db4objects.Db4o.Reflect.IReflectClass)example;
			}
			if (example is System.Type)
			{
				return i_trans.Reflector().ForClass((System.Type)example);
			}
			return null;
		}

		public virtual Db4objects.Db4o.Query.IConstraints Constraints()
		{
			lock (StreamLock())
			{
				Db4objects.Db4o.Query.IConstraint[] constraints = new Db4objects.Db4o.Query.IConstraint
					[i_constraints.Size()];
				i_constraints.ToArray(constraints);
				return new Db4objects.Db4o.Internal.Query.Processor.QConstraints(i_trans, constraints
					);
			}
		}

		public virtual Db4objects.Db4o.Query.IQuery Descend(string a_field)
		{
			lock (StreamLock())
			{
				Db4objects.Db4o.Internal.Query.Processor.QQuery query = new Db4objects.Db4o.Internal.Query.Processor.QQuery
					(i_trans, _this, a_field);
				int[] run = { 1 };
				if (!Descend1(query, a_field, run))
				{
					if (run[0] == 1)
					{
						run[0] = 2;
						if (!Descend1(query, a_field, run))
						{
							return null;
						}
					}
				}
				return query;
			}
		}

		private bool Descend1(Db4objects.Db4o.Internal.Query.Processor.QQuery query, string
			 a_field, int[] run)
		{
			bool[] foundClass = { false };
			if (run[0] == 2 || i_constraints.Size() == 0)
			{
				run[0] = 0;
				bool[] anyClassCollected = { false };
				Stream().ClassCollection().AttachQueryNode(a_field, new _AnonymousInnerClass246(this
					, anyClassCollected));
			}
			System.Collections.IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				if (((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).Attach(query, a_field
					))
				{
					foundClass[0] = true;
				}
			}
			return foundClass[0];
		}

		private sealed class _AnonymousInnerClass246 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass246(QQueryBase _enclosing, bool[] anyClassCollected)
			{
				this._enclosing = _enclosing;
				this.anyClassCollected = anyClassCollected;
			}

			public void Visit(object obj)
			{
				object[] pair = ((object[])obj);
				Db4objects.Db4o.Internal.ClassMetadata parentYc = (Db4objects.Db4o.Internal.ClassMetadata
					)pair[0];
				Db4objects.Db4o.Internal.FieldMetadata yf = (Db4objects.Db4o.Internal.FieldMetadata
					)pair[1];
				Db4objects.Db4o.Internal.ClassMetadata childYc = yf.GetFieldYapClass(this._enclosing
					.Stream());
				bool take = true;
				if (childYc is Db4objects.Db4o.Internal.UntypedFieldHandler)
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
					Db4objects.Db4o.Internal.Query.Processor.QConClass qcc = new Db4objects.Db4o.Internal.Query.Processor.QConClass
						(this._enclosing.i_trans, null, yf.QField(this._enclosing.i_trans), parentYc.ClassReflector
						());
					this._enclosing.AddConstraint(qcc);
				}
			}

			private readonly QQueryBase _enclosing;

			private readonly bool[] anyClassCollected;
		}

		public virtual Db4objects.Db4o.IObjectSet Execute()
		{
			Db4objects.Db4o.Internal.Callbacks.ICallbacks callbacks = Stream().Callbacks();
			callbacks.OnQueryStarted(Cast(this));
			Db4objects.Db4o.Internal.Query.Result.IQueryResult qresult = GetQueryResult();
			callbacks.OnQueryFinished(Cast(this));
			return new Db4objects.Db4o.Internal.Query.ObjectSetFacade(qresult);
		}

		public virtual Db4objects.Db4o.Internal.Query.Result.IQueryResult GetQueryResult(
			)
		{
			lock (StreamLock())
			{
				if (i_constraints.Size() == 0)
				{
					return Stream().GetAll(i_trans);
				}
				Db4objects.Db4o.Internal.Query.Result.IQueryResult result = ClassOnlyQuery();
				if (result != null)
				{
					return result;
				}
				return Stream().ExecuteQuery(_this);
			}
		}

		protected virtual Db4objects.Db4o.Internal.ObjectContainerBase Stream()
		{
			return i_trans.Stream();
		}

		private Db4objects.Db4o.Internal.Query.Result.IQueryResult ClassOnlyQuery()
		{
			if (i_constraints.Size() != 1 || _comparator != null)
			{
				return null;
			}
			Db4objects.Db4o.Query.IConstraint constr = SingleConstraint();
			if (constr.GetType() != typeof(Db4objects.Db4o.Internal.Query.Processor.QConClass)
				)
			{
				return null;
			}
			Db4objects.Db4o.Internal.Query.Processor.QConClass clazzconstr = (Db4objects.Db4o.Internal.Query.Processor.QConClass
				)constr;
			Db4objects.Db4o.Internal.ClassMetadata clazz = clazzconstr.i_yapClass;
			if (clazz == null)
			{
				return null;
			}
			if (clazzconstr.HasChildren() || clazz.IsArray())
			{
				return null;
			}
			Db4objects.Db4o.Internal.Query.Result.IQueryResult queryResult = Stream().ClassOnlyQuery
				(i_trans, clazz);
			if (queryResult == null)
			{
				return null;
			}
			Sort(queryResult);
			return queryResult;
		}

		private Db4objects.Db4o.Query.IConstraint SingleConstraint()
		{
			return (Db4objects.Db4o.Query.IConstraint)i_constraints.SingleElement();
		}

		public class CreateCandidateCollectionResult
		{
			public readonly bool checkDuplicates;

			public readonly bool topLevel;

			public readonly Db4objects.Db4o.Foundation.List4 candidateCollection;

			public CreateCandidateCollectionResult(Db4objects.Db4o.Foundation.List4 candidateCollection_
				, bool checkDuplicates_, bool topLevel_)
			{
				candidateCollection = candidateCollection_;
				topLevel = topLevel_;
				checkDuplicates = checkDuplicates_;
			}
		}

		public virtual System.Collections.IEnumerator ExecuteSnapshot()
		{
			Db4objects.Db4o.Internal.Query.Processor.QQueryBase.CreateCandidateCollectionResult
				 r = CreateCandidateCollection();
			Db4objects.Db4o.Foundation.Collection4 executionPath = ExecutionPath(r);
			System.Collections.IEnumerator candidatesIterator = new Db4objects.Db4o.Foundation.Iterator4Impl
				(r.candidateCollection);
			Db4objects.Db4o.Foundation.Collection4 snapshots = new Db4objects.Db4o.Foundation.Collection4
				();
			while (candidatesIterator.MoveNext())
			{
				Db4objects.Db4o.Internal.Query.Processor.QCandidates candidates = (Db4objects.Db4o.Internal.Query.Processor.QCandidates
					)candidatesIterator.Current;
				snapshots.Add(candidates.ExecuteSnapshot(executionPath));
			}
			System.Collections.IEnumerator snapshotsIterator = snapshots.GetEnumerator();
			Db4objects.Db4o.Foundation.CompositeIterator4 resultingIDs = new Db4objects.Db4o.Foundation.CompositeIterator4
				(snapshotsIterator);
			if (!r.checkDuplicates)
			{
				return resultingIDs;
			}
			return CheckDuplicates(resultingIDs);
		}

		public virtual System.Collections.IEnumerator ExecuteLazy()
		{
			Db4objects.Db4o.Internal.Query.Processor.QQueryBase.CreateCandidateCollectionResult
				 r = CreateCandidateCollection();
			Db4objects.Db4o.Foundation.Collection4 executionPath = ExecutionPath(r);
			System.Collections.IEnumerator candidateCollection = new Db4objects.Db4o.Foundation.Iterator4Impl
				(r.candidateCollection);
			Db4objects.Db4o.Foundation.MappingIterator executeCandidates = new _AnonymousInnerClass391
				(this, executionPath, candidateCollection);
			Db4objects.Db4o.Foundation.CompositeIterator4 resultingIDs = new Db4objects.Db4o.Foundation.CompositeIterator4
				(executeCandidates);
			if (!r.checkDuplicates)
			{
				return resultingIDs;
			}
			return CheckDuplicates(resultingIDs);
		}

		private sealed class _AnonymousInnerClass391 : Db4objects.Db4o.Foundation.MappingIterator
		{
			public _AnonymousInnerClass391(QQueryBase _enclosing, Db4objects.Db4o.Foundation.Collection4
				 executionPath, System.Collections.IEnumerator baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
				this.executionPath = executionPath;
			}

			protected override object Map(object current)
			{
				return ((Db4objects.Db4o.Internal.Query.Processor.QCandidates)current).ExecuteLazy
					(executionPath);
			}

			private readonly QQueryBase _enclosing;

			private readonly Db4objects.Db4o.Foundation.Collection4 executionPath;
		}

		private Db4objects.Db4o.Foundation.MappingIterator CheckDuplicates(Db4objects.Db4o.Foundation.CompositeIterator4
			 executeAllCandidates)
		{
			return new _AnonymousInnerClass407(this, executeAllCandidates);
		}

		private sealed class _AnonymousInnerClass407 : Db4objects.Db4o.Foundation.MappingIterator
		{
			public _AnonymousInnerClass407(QQueryBase _enclosing, Db4objects.Db4o.Foundation.CompositeIterator4
				 baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			private Db4objects.Db4o.Internal.TreeInt ids = new Db4objects.Db4o.Internal.TreeInt
				(0);

			protected override object Map(object current)
			{
				int id = ((int)current);
				if (this.ids.Find(id) != null)
				{
					return Db4objects.Db4o.Foundation.MappingIterator.SKIP;
				}
				this.ids = (Db4objects.Db4o.Internal.TreeInt)this.ids.Add(new Db4objects.Db4o.Internal.TreeInt
					(id));
				return current;
			}

			private readonly QQueryBase _enclosing;
		}

		private Db4objects.Db4o.Foundation.Collection4 ExecutionPath(Db4objects.Db4o.Internal.Query.Processor.QQueryBase.CreateCandidateCollectionResult
			 r)
		{
			return r.topLevel ? null : FieldPathFromTop();
		}

		public virtual void ExecuteLocal(Db4objects.Db4o.Internal.Query.Result.IdListQueryResult
			 result)
		{
			Db4objects.Db4o.Internal.Query.Processor.QQueryBase.CreateCandidateCollectionResult
				 r = CreateCandidateCollection();
			bool checkDuplicates = r.checkDuplicates;
			bool topLevel = r.topLevel;
			Db4objects.Db4o.Foundation.List4 candidateCollection = r.candidateCollection;
			if (candidateCollection != null)
			{
				Db4objects.Db4o.Foundation.Collection4 executionPath = topLevel ? null : FieldPathFromTop
					();
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(candidateCollection
					);
				while (i.MoveNext())
				{
					((Db4objects.Db4o.Internal.Query.Processor.QCandidates)i.Current).Execute();
				}
				if (candidateCollection._next != null)
				{
					checkDuplicates = true;
				}
				if (checkDuplicates)
				{
					result.CheckDuplicates();
				}
				Db4objects.Db4o.Internal.ObjectContainerBase stream = Stream();
				i = new Db4objects.Db4o.Foundation.Iterator4Impl(candidateCollection);
				while (i.MoveNext())
				{
					Db4objects.Db4o.Internal.Query.Processor.QCandidates candidates = (Db4objects.Db4o.Internal.Query.Processor.QCandidates
						)i.Current;
					if (topLevel)
					{
						candidates.Traverse(result);
					}
					else
					{
						candidates.Traverse(new _AnonymousInnerClass461(this, executionPath, stream, result
							));
					}
				}
			}
			Sort(result);
		}

		private sealed class _AnonymousInnerClass461 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass461(QQueryBase _enclosing, Db4objects.Db4o.Foundation.Collection4
				 executionPath, Db4objects.Db4o.Internal.ObjectContainerBase stream, Db4objects.Db4o.Internal.Query.Result.IdListQueryResult
				 result)
			{
				this._enclosing = _enclosing;
				this.executionPath = executionPath;
				this.stream = stream;
				this.result = result;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.Internal.Query.Processor.QCandidate candidate = (Db4objects.Db4o.Internal.Query.Processor.QCandidate
					)a_object;
				if (candidate.Include())
				{
					Db4objects.Db4o.Internal.TreeInt ids = new Db4objects.Db4o.Internal.TreeInt(candidate
						._key);
					Db4objects.Db4o.Internal.TreeInt[] idsNew = new Db4objects.Db4o.Internal.TreeInt[
						1];
					System.Collections.IEnumerator itPath = executionPath.GetEnumerator();
					while (itPath.MoveNext())
					{
						idsNew[0] = null;
						string fieldName = (string)(itPath.Current);
						if (ids != null)
						{
							ids.Traverse(new _AnonymousInnerClass472(this, stream, idsNew, fieldName));
						}
						ids = idsNew[0];
					}
					if (ids != null)
					{
						ids.Traverse(new _AnonymousInnerClass492(this, result));
					}
				}
			}

			private sealed class _AnonymousInnerClass472 : Db4objects.Db4o.Foundation.IVisitor4
			{
				public _AnonymousInnerClass472(_AnonymousInnerClass461 _enclosing, Db4objects.Db4o.Internal.ObjectContainerBase
					 stream, Db4objects.Db4o.Internal.TreeInt[] idsNew, string fieldName)
				{
					this._enclosing = _enclosing;
					this.stream = stream;
					this.idsNew = idsNew;
					this.fieldName = fieldName;
				}

				public void Visit(object treeInt)
				{
					int id = ((Db4objects.Db4o.Internal.TreeInt)treeInt)._key;
					Db4objects.Db4o.Internal.StatefulBuffer reader = stream.ReadWriterByID(this._enclosing
						._enclosing.i_trans, id);
					if (reader != null)
					{
						Db4objects.Db4o.Internal.Marshall.ObjectHeader oh = new Db4objects.Db4o.Internal.Marshall.ObjectHeader
							(stream, reader);
						idsNew[0] = oh.YapClass().CollectFieldIDs(oh._marshallerFamily, oh._headerAttributes
							, idsNew[0], reader, fieldName);
					}
				}

				private readonly _AnonymousInnerClass461 _enclosing;

				private readonly Db4objects.Db4o.Internal.ObjectContainerBase stream;

				private readonly Db4objects.Db4o.Internal.TreeInt[] idsNew;

				private readonly string fieldName;
			}

			private sealed class _AnonymousInnerClass492 : Db4objects.Db4o.Foundation.IVisitor4
			{
				public _AnonymousInnerClass492(_AnonymousInnerClass461 _enclosing, Db4objects.Db4o.Internal.Query.Result.IdListQueryResult
					 result)
				{
					this._enclosing = _enclosing;
					this.result = result;
				}

				public void Visit(object treeInt)
				{
					result.AddKeyCheckDuplicates(((Db4objects.Db4o.Internal.TreeInt)treeInt)._key);
				}

				private readonly _AnonymousInnerClass461 _enclosing;

				private readonly Db4objects.Db4o.Internal.Query.Result.IdListQueryResult result;
			}

			private readonly QQueryBase _enclosing;

			private readonly Db4objects.Db4o.Foundation.Collection4 executionPath;

			private readonly Db4objects.Db4o.Internal.ObjectContainerBase stream;

			private readonly Db4objects.Db4o.Internal.Query.Result.IdListQueryResult result;
		}

		private Db4objects.Db4o.Foundation.Collection4 FieldPathFromTop()
		{
			Db4objects.Db4o.Internal.Query.Processor.QQueryBase q = this;
			Db4objects.Db4o.Foundation.Collection4 fieldPath = new Db4objects.Db4o.Foundation.Collection4
				();
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

		public virtual Db4objects.Db4o.Internal.Query.Processor.QQueryBase.CreateCandidateCollectionResult
			 CreateCandidateCollection()
		{
			bool checkDuplicates = false;
			bool topLevel = true;
			Db4objects.Db4o.Foundation.List4 candidateCollection = null;
			System.Collections.IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Internal.Query.Processor.QCon qcon = (Db4objects.Db4o.Internal.Query.Processor.QCon
					)i.Current;
				Db4objects.Db4o.Internal.Query.Processor.QCon old = qcon;
				qcon = qcon.GetRoot();
				if (qcon != old)
				{
					checkDuplicates = true;
					topLevel = false;
				}
				Db4objects.Db4o.Internal.ClassMetadata yc = qcon.GetYapClass();
				if (yc == null)
				{
					break;
				}
				candidateCollection = AddConstraintToCandidateCollection(candidateCollection, qcon
					);
			}
			return new Db4objects.Db4o.Internal.Query.Processor.QQueryBase.CreateCandidateCollectionResult
				(candidateCollection, checkDuplicates, topLevel);
		}

		private Db4objects.Db4o.Foundation.List4 AddConstraintToCandidateCollection(Db4objects.Db4o.Foundation.List4
			 candidateCollection, Db4objects.Db4o.Internal.Query.Processor.QCon qcon)
		{
			if (candidateCollection != null)
			{
				if (TryToAddToExistingCandidate(candidateCollection, qcon))
				{
					return candidateCollection;
				}
			}
			Db4objects.Db4o.Internal.Query.Processor.QCandidates candidates = new Db4objects.Db4o.Internal.Query.Processor.QCandidates
				(i_trans, qcon.GetYapClass(), null);
			candidates.AddConstraint(qcon);
			return new Db4objects.Db4o.Foundation.List4(candidateCollection, candidates);
		}

		private bool TryToAddToExistingCandidate(Db4objects.Db4o.Foundation.List4 candidateCollection
			, Db4objects.Db4o.Internal.Query.Processor.QCon qcon)
		{
			System.Collections.IEnumerator j = new Db4objects.Db4o.Foundation.Iterator4Impl(candidateCollection
				);
			while (j.MoveNext())
			{
				Db4objects.Db4o.Internal.Query.Processor.QCandidates candidates = (Db4objects.Db4o.Internal.Query.Processor.QCandidates
					)j.Current;
				if (candidates.TryAddConstraint(qcon))
				{
					return true;
				}
			}
			return false;
		}

		public Db4objects.Db4o.Internal.Transaction GetTransaction()
		{
			return i_trans;
		}

		internal virtual System.Collections.IEnumerator IterateConstraints()
		{
			return new Db4objects.Db4o.Foundation.Collection4(i_constraints).GetEnumerator();
		}

		public virtual Db4objects.Db4o.Query.IQuery OrderAscending()
		{
			lock (StreamLock())
			{
				SetOrdering(i_orderingGenerator.Next());
				return _this;
			}
		}

		public virtual Db4objects.Db4o.Query.IQuery OrderDescending()
		{
			lock (StreamLock())
			{
				SetOrdering(-i_orderingGenerator.Next());
				return _this;
			}
		}

		private void SetOrdering(int ordering)
		{
			System.Collections.IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).SetOrdering(ordering);
			}
		}

		public virtual void Marshall()
		{
			_evaluationModeAsInt = _evaluationMode.AsInt();
			System.Collections.IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).GetRoot().Marshall();
			}
		}

		public virtual void Unmarshall(Db4objects.Db4o.Internal.Transaction a_trans)
		{
			_evaluationMode = Db4objects.Db4o.Config.QueryEvaluationMode.FromInt(_evaluationModeAsInt
				);
			i_trans = a_trans;
			System.Collections.IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				((Db4objects.Db4o.Internal.Query.Processor.QCon)i.Current).Unmarshall(a_trans);
			}
		}

		internal virtual void RemoveConstraint(Db4objects.Db4o.Internal.Query.Processor.QCon
			 a_constraint)
		{
			i_constraints.Remove(a_constraint);
		}

		internal virtual Db4objects.Db4o.Query.IConstraint ToConstraint(Db4objects.Db4o.Foundation.Collection4
			 constraints)
		{
			if (constraints.Size() == 1)
			{
				return (Db4objects.Db4o.Query.IConstraint)constraints.SingleElement();
			}
			else
			{
				if (constraints.Size() > 0)
				{
					Db4objects.Db4o.Query.IConstraint[] constraintArray = new Db4objects.Db4o.Query.IConstraint
						[constraints.Size()];
					constraints.ToArray(constraintArray);
					return new Db4objects.Db4o.Internal.Query.Processor.QConstraints(i_trans, constraintArray
						);
				}
			}
			return null;
		}

		protected virtual object StreamLock()
		{
			return Stream().i_lock;
		}

		public virtual Db4objects.Db4o.Query.IQuery SortBy(Db4objects.Db4o.Query.IQueryComparator
			 comparator)
		{
			_comparator = comparator;
			return _this;
		}

		private void Sort(Db4objects.Db4o.Internal.Query.Result.IQueryResult result)
		{
			if (_comparator != null)
			{
				result.Sort(_comparator);
			}
		}

		private static Db4objects.Db4o.Internal.Query.Processor.QQuery Cast(Db4objects.Db4o.Internal.Query.Processor.QQueryBase
			 obj)
		{
			return (Db4objects.Db4o.Internal.Query.Processor.QQuery)obj;
		}

		public virtual bool RequiresSort()
		{
			if (_comparator != null)
			{
				return true;
			}
			System.Collections.IEnumerator i = IterateConstraints();
			while (i.MoveNext())
			{
				Db4objects.Db4o.Internal.Query.Processor.QCon qCon = (Db4objects.Db4o.Internal.Query.Processor.QCon
					)i.Current;
				if (qCon.RequiresSort())
				{
					return true;
				}
			}
			return false;
		}

		public virtual Db4objects.Db4o.Query.IQueryComparator Comparator()
		{
			return _comparator;
		}

		public virtual Db4objects.Db4o.Config.QueryEvaluationMode EvaluationMode()
		{
			return _evaluationMode;
		}

		public virtual void EvaluationMode(Db4objects.Db4o.Config.QueryEvaluationMode mode
			)
		{
			_evaluationMode = mode;
		}
	}
}
