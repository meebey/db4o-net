/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Handlers.Array;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <summary>Represents an actual object in the database.</summary>
	/// <remarks>
	/// Represents an actual object in the database. Forms a tree structure, indexed
	/// by id. Can have dependents that are doNotInclude'd in the query result when
	/// this is doNotInclude'd.
	/// </remarks>
	/// <exclude></exclude>
	public class QCandidate : TreeInt, ICandidate, IOrderable
	{
		internal ByteArrayBuffer _bytes;

		internal readonly QCandidates _candidates;

		private List4 _dependants;

		internal bool _include = true;

		private object _member;

		internal IOrderable _order;

		internal Tree _pendingJoins;

		private Db4objects.Db4o.Internal.Query.Processor.QCandidate _root;

		internal ClassMetadata _yapClass;

		internal FieldMetadata _yapField;

		private int _handlerVersion;

		private QCandidate(QCandidates qcandidates) : base(0)
		{
			// db4o ID is stored in _key;
			// db4o byte stream storing the object
			// Dependant candidates
			// whether to include in the result set
			// may use id for optimisation ???
			// Comparable
			// Possible pending joins on children
			// The evaluation root to compare all ORs
			// the YapClass of this object
			// temporary yapField and member for one field during evaluation
			// null denotes null object
			_candidates = qcandidates;
		}

		public QCandidate(QCandidates candidates, object obj, int id) : base(id)
		{
			if (DTrace.enabled)
			{
				DTrace.CreateCandidate.Log(id);
			}
			_candidates = candidates;
			_order = this;
			_member = obj;
			_include = true;
			if (id == 0)
			{
				_key = candidates.GenerateCandidateId();
			}
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Internal.Query.Processor.QCandidate qcan = new Db4objects.Db4o.Internal.Query.Processor.QCandidate
				(_candidates);
			qcan.SetBytes(_bytes);
			qcan._dependants = _dependants;
			qcan._include = _include;
			qcan._member = _member;
			qcan._order = _order;
			qcan._pendingJoins = _pendingJoins;
			qcan._root = _root;
			qcan._yapClass = _yapClass;
			qcan._yapField = _yapField;
			return base.ShallowCloneInternal(qcan);
		}

		internal virtual void AddDependant(Db4objects.Db4o.Internal.Query.Processor.QCandidate
			 a_candidate)
		{
			_dependants = new List4(_dependants, a_candidate);
		}

		private void CheckInstanceOfCompare()
		{
			if (_member is ICompare)
			{
				_member = ((ICompare)_member).Compare();
				LocalObjectContainer stream = Container();
				_yapClass = stream.ClassMetadataForReflectClass(stream.Reflector().ForObject(_member
					));
				_key = stream.GetID(Transaction(), _member);
				if (_key == 0)
				{
					SetBytes(null);
				}
				else
				{
					SetBytes(stream.ReadReaderByID(Transaction(), _key));
				}
			}
		}

		public override int Compare(Tree a_to)
		{
			return _order.CompareTo(((Db4objects.Db4o.Internal.Query.Processor.QCandidate)a_to
				)._order);
		}

		public virtual int CompareTo(object a_object)
		{
			if (a_object is Order)
			{
				return -((Order)a_object).CompareTo(this);
			}
			return _key - ((TreeInt)a_object)._key;
		}

		internal virtual bool CreateChild(QCandidates a_candidates)
		{
			if (!_include)
			{
				return false;
			}
			if (_yapField != null)
			{
				ITypeHandler4 handler = _yapField.GetHandler();
				if (handler != null)
				{
					QueryingReadContext queryingReadContext = new QueryingReadContext(Transaction(), 
						MarshallerFamily().HandlerVersion(), _bytes);
					ITypeHandler4 tempHandler = null;
					if (handler is IFirstClassHandler)
					{
						IFirstClassHandler firstClassHandler = (IFirstClassHandler)Handlers4.CorrectHandlerVersion
							(queryingReadContext, handler);
						tempHandler = Handlers4.CorrectHandlerVersion(queryingReadContext, firstClassHandler
							.ReadCandidateHandler(queryingReadContext));
					}
					if (tempHandler != null)
					{
						ITypeHandler4 arrayElementHandler = tempHandler;
						int offset = queryingReadContext.Offset();
						bool outerRes = true;
						// The following construct is worse than not ideal.
						// For each constraint it completely reads the
						// underlying structure again. The structure could b
						// kept fairly easy. TODO: Optimize!
						IEnumerator i = a_candidates.IterateConstraints();
						while (i.MoveNext())
						{
							QCon qcon = (QCon)i.Current;
							QField qf = qcon.GetField();
							if (qf == null || qf.i_name.Equals(_yapField.GetName()))
							{
								QCon tempParent = qcon.i_parent;
								qcon.SetParent(null);
								QCandidates candidates = new QCandidates(a_candidates.i_trans, null, qf);
								candidates.AddConstraint(qcon);
								qcon.SetCandidates(candidates);
								ReadArrayCandidates(handler, queryingReadContext.Buffer(), arrayElementHandler, candidates
									);
								queryingReadContext.Seek(offset);
								bool isNot = qcon.IsNot();
								if (isNot)
								{
									qcon.RemoveNot();
								}
								candidates.Evaluate();
								Tree.ByRef pending = new Tree.ByRef();
								bool[] innerRes = new bool[] { isNot };
								candidates.Traverse(new _IVisitor4_187(innerRes, isNot, pending));
								// Collect all pending subresults.
								// We need to change
								// the
								// constraint here, so
								// our
								// pending collector
								// uses
								// the right
								// comparator.
								// We only keep one
								// pending result
								// for
								// all array
								// elements.
								// and memorize,
								// whether we had a
								// true or a false
								// result.
								// or both.
								if (isNot)
								{
									qcon.Not();
								}
								// In case we had pending subresults, we
								// need to communicate
								// them up to our root.
								if (pending.value != null)
								{
									pending.value.Traverse(new _IVisitor4_256(this));
								}
								if (!innerRes[0])
								{
									// Again this could be double triggering.
									// 
									// We want to clean up the "No route"
									// at some stage.
									qcon.Visit(GetRoot(), qcon.Evaluator().Not(false));
									outerRes = false;
								}
								qcon.SetParent(tempParent);
							}
						}
						return outerRes;
					}
					// We may get simple types here too, if the YapField was null
					// in the higher level simple evaluation. Evaluate these
					// immediately.
					if (Handlers4.HandlesSimple(handler))
					{
						a_candidates.i_currentConstraint.Visit(this);
						return true;
					}
				}
			}
			if (_yapField == null || _yapField is NullFieldMetadata)
			{
				return false;
			}
			_yapClass.SeekToField(Transaction(), _bytes, _yapField);
			Db4objects.Db4o.Internal.Query.Processor.QCandidate candidate = ReadSubCandidate(
				a_candidates);
			if (candidate == null)
			{
				return false;
			}
			// fast early check for YapClass
			if (a_candidates.i_yapClass != null && a_candidates.i_yapClass.IsStrongTyped())
			{
				if (_yapField != null)
				{
					ITypeHandler4 handler = _yapField.GetHandler();
					if (handler is ClassMetadata)
					{
						ClassMetadata classMetadata = (ClassMetadata)handler;
						if (classMetadata is UntypedFieldHandler)
						{
							classMetadata = candidate.ReadYapClass();
						}
						if (classMetadata == null)
						{
							return false;
						}
						if (!Handlers4.HandlerCanHold(classMetadata, Container().Reflector(), a_candidates
							.i_yapClass.ClassReflector()))
						{
							return false;
						}
					}
				}
			}
			AddDependant(a_candidates.Add(candidate));
			return true;
		}

		private sealed class _IVisitor4_187 : IVisitor4
		{
			public _IVisitor4_187(bool[] innerRes, bool isNot, Tree.ByRef pending)
			{
				this.innerRes = innerRes;
				this.isNot = isNot;
				this.pending = pending;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Internal.Query.Processor.QCandidate cand = (Db4objects.Db4o.Internal.Query.Processor.QCandidate
					)obj;
				if (cand.Include())
				{
					innerRes[0] = !isNot;
				}
				if (cand._pendingJoins != null)
				{
					cand._pendingJoins.Traverse(new _IVisitor4_200(pending));
				}
			}

			private sealed class _IVisitor4_200 : IVisitor4
			{
				public _IVisitor4_200(Tree.ByRef pending)
				{
					this.pending = pending;
				}

				public void Visit(object a_object)
				{
					QPending newPending = ((QPending)a_object).InternalClonePayload();
					newPending.ChangeConstraint();
					QPending oldPending = (QPending)Tree.Find(pending.value, newPending);
					if (oldPending != null)
					{
						if (oldPending._result != newPending._result)
						{
							oldPending._result = QPending.Both;
						}
					}
					else
					{
						pending.value = Tree.Add(pending.value, newPending);
					}
				}

				private readonly Tree.ByRef pending;
			}

			private readonly bool[] innerRes;

			private readonly bool isNot;

			private readonly Tree.ByRef pending;
		}

		private sealed class _IVisitor4_256 : IVisitor4
		{
			public _IVisitor4_256(QCandidate _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				this._enclosing.GetRoot().Evaluate((QPending)a_object);
			}

			private readonly QCandidate _enclosing;
		}

		private void ReadArrayCandidates(ITypeHandler4 fieldHandler, IReadBuffer buffer, 
			ITypeHandler4 arrayElementHandler, QCandidates candidates)
		{
			if (!(arrayElementHandler is IFirstClassHandler))
			{
				return;
			}
			SlotFormat slotFormat = SlotFormat.ForHandlerVersion(_handlerVersion);
			slotFormat.DoWithSlotIndirection(buffer, fieldHandler, new _IClosure4_340(this, slotFormat
				, arrayElementHandler, buffer, candidates));
		}

		private sealed class _IClosure4_340 : IClosure4
		{
			public _IClosure4_340(QCandidate _enclosing, SlotFormat slotFormat, ITypeHandler4
				 arrayElementHandler, IReadBuffer buffer, QCandidates candidates)
			{
				this._enclosing = _enclosing;
				this.slotFormat = slotFormat;
				this.arrayElementHandler = arrayElementHandler;
				this.buffer = buffer;
				this.candidates = candidates;
			}

			public object Run()
			{
				QueryingReadContext context = null;
				if (slotFormat.HandleAsObject(arrayElementHandler))
				{
					// TODO: Code is similar to FieldMetadata.collectIDs. Try to refactor to one place.
					int collectionID = buffer.ReadInt();
					ByteArrayBuffer arrayElementBuffer = this._enclosing.Container().ReadReaderByID(this
						._enclosing.Transaction(), collectionID);
					ObjectHeader objectHeader = ObjectHeader.ScrollBufferToContent(this._enclosing.Container
						(), arrayElementBuffer);
					context = new QueryingReadContext(this._enclosing.Transaction(), candidates, this
						._enclosing._handlerVersion, arrayElementBuffer, collectionID);
					objectHeader.ClassMetadata().CollectIDs(context);
				}
				else
				{
					context = new QueryingReadContext(this._enclosing.Transaction(), candidates, this
						._enclosing._handlerVersion, buffer, 0);
					((IFirstClassHandler)arrayElementHandler).CollectIDs(context);
				}
				Tree.Traverse(context.Ids(), new _IVisitor4_358(candidates));
				IEnumerator i = context.ObjectsWithoutId();
				while (i.MoveNext())
				{
					object obj = i.Current;
					candidates.Add(new Db4objects.Db4o.Internal.Query.Processor.QCandidate(candidates
						, obj, 0));
				}
				return null;
			}

			private sealed class _IVisitor4_358 : IVisitor4
			{
				public _IVisitor4_358(QCandidates candidates)
				{
					this.candidates = candidates;
				}

				public void Visit(object obj)
				{
					TreeInt idNode = (TreeInt)obj;
					candidates.Add(new Db4objects.Db4o.Internal.Query.Processor.QCandidate(candidates
						, null, idNode._key));
				}

				private readonly QCandidates candidates;
			}

			private readonly QCandidate _enclosing;

			private readonly SlotFormat slotFormat;

			private readonly ITypeHandler4 arrayElementHandler;

			private readonly IReadBuffer buffer;

			private readonly QCandidates candidates;
		}

		internal virtual void DoNotInclude()
		{
			Include(false);
			if (_dependants != null)
			{
				IEnumerator i = new Iterator4Impl(_dependants);
				_dependants = null;
				while (i.MoveNext())
				{
					((Db4objects.Db4o.Internal.Query.Processor.QCandidate)i.Current).DoNotInclude();
				}
			}
		}

		public override bool Duplicates()
		{
			return _order.HasDuplicates();
		}

		internal virtual bool Evaluate(QConObject a_constraint, QE a_evaluator)
		{
			if (a_evaluator.Identity())
			{
				return a_evaluator.Evaluate(a_constraint, this, null);
			}
			if (_member == null)
			{
				_member = Value();
			}
			return a_evaluator.Evaluate(a_constraint, this, a_constraint.Translate(_member));
		}

		internal virtual bool Evaluate(QPending a_pending)
		{
			QPending oldPending = (QPending)Tree.Find(_pendingJoins, a_pending);
			if (oldPending == null)
			{
				a_pending.ChangeConstraint();
				_pendingJoins = Tree.Add(_pendingJoins, a_pending.InternalClonePayload());
				return true;
			}
			_pendingJoins = _pendingJoins.RemoveNode(oldPending);
			oldPending._join.EvaluatePending(this, oldPending, a_pending._result);
			return false;
		}

		internal virtual IReflectClass ClassReflector()
		{
			ReadYapClass();
			if (_yapClass == null)
			{
				return null;
			}
			return _yapClass.ClassReflector();
		}

		internal virtual bool FieldIsAvailable()
		{
			return ClassReflector() != null;
		}

		// / ***<Candidate interface code>***
		public virtual IObjectContainer ObjectContainer()
		{
			return Container();
		}

		public virtual object GetObject()
		{
			object obj = Value(true);
			if (obj is ByteArrayBuffer)
			{
				ByteArrayBuffer reader = (ByteArrayBuffer)obj;
				int offset = reader._offset;
				obj = ReadString(reader);
				reader._offset = offset;
			}
			return obj;
		}

		public virtual string ReadString(ByteArrayBuffer buffer)
		{
			return StringHandler.ReadString(Transaction().Context(), buffer);
		}

		internal virtual Db4objects.Db4o.Internal.Query.Processor.QCandidate GetRoot()
		{
			return _root == null ? this : _root;
		}

		internal LocalObjectContainer Container()
		{
			return Transaction().File();
		}

		internal LocalTransaction Transaction()
		{
			return _candidates.i_trans;
		}

		public virtual bool HasDuplicates()
		{
			// Subcandidates are evaluated along with their constraints
			// in one big QCandidates object. The tree can have duplicates
			// so evaluation can be cascaded up to different roots.
			return _root != null;
		}

		public virtual void HintOrder(int a_order, bool a_major)
		{
			if (_order == this)
			{
				_order = new Order();
			}
			_order.HintOrder(a_order, a_major);
		}

		public virtual bool Include()
		{
			return _include;
		}

		/// <summary>For external interface use only.</summary>
		/// <remarks>
		/// For external interface use only. Call doNotInclude() internally so
		/// dependancies can be checked.
		/// </remarks>
		public virtual void Include(bool flag)
		{
			// TODO:
			// Internal and external flag may need to be handled seperately.
			_include = flag;
		}

		public override void OnAttemptToAddDuplicate(Tree a_tree)
		{
			_size = 0;
			_root = (Db4objects.Db4o.Internal.Query.Processor.QCandidate)a_tree;
		}

		private IReflectClass MemberClass()
		{
			return Transaction().Reflector().ForObject(_member);
		}

		internal virtual IPreparedComparison PrepareComparison(ObjectContainerBase container
			, object constraint)
		{
			IContext context = container.Transaction().Context();
			if (_yapField != null)
			{
				return _yapField.PrepareComparison(context, constraint);
			}
			if (_yapClass != null)
			{
				return _yapClass.PrepareComparison(context, constraint);
			}
			IReflector reflector = container.Reflector();
			ClassMetadata classMetadata = null;
			if (_bytes != null)
			{
				classMetadata = container.ProduceClassMetadata(reflector.ForObject(constraint));
			}
			else
			{
				if (_member != null)
				{
					classMetadata = container.ClassMetadataForReflectClass(reflector.ForObject(_member
						));
				}
			}
			if (classMetadata != null)
			{
				if (_member != null && _member.GetType().IsArray)
				{
					ITypeHandler4 arrayElementTypehandler = classMetadata.TypeHandler();
					if (reflector.Array().IsNDimensional(MemberClass()))
					{
						MultidimensionalArrayHandler mah = new MultidimensionalArrayHandler(arrayElementTypehandler
							, false);
						return mah.PrepareComparison(context, _member);
					}
					ArrayHandler ya = new ArrayHandler(arrayElementTypehandler, false);
					return ya.PrepareComparison(context, _member);
				}
				return classMetadata.PrepareComparison(context, constraint);
			}
			return null;
		}

		private void Read()
		{
			if (_include)
			{
				if (_bytes == null)
				{
					if (_key > 0)
					{
						if (DTrace.enabled)
						{
							DTrace.CandidateRead.Log(_key);
						}
						SetBytes(Container().ReadReaderByID(Transaction(), _key));
						if (_bytes == null)
						{
							Include(false);
						}
					}
					else
					{
						Include(false);
					}
				}
			}
		}

		private int CurrentOffSet()
		{
			return _bytes._offset;
		}

		private Db4objects.Db4o.Internal.Query.Processor.QCandidate ReadSubCandidate(QCandidates
			 candidateCollection)
		{
			Read();
			if (_bytes == null || _yapField == null)
			{
				return null;
			}
			int offset = CurrentOffSet();
			QueryingReadContext context = NewQueryingReadContext();
			ITypeHandler4 handler = Handlers4.CorrectHandlerVersion(context, _yapField.GetHandler
				());
			Db4objects.Db4o.Internal.Query.Processor.QCandidate subCandidate = candidateCollection
				.ReadSubCandidate(context, handler);
			Seek(offset);
			if (subCandidate != null)
			{
				subCandidate._root = GetRoot();
				return subCandidate;
			}
			return null;
		}

		private void Seek(int offset)
		{
			_bytes._offset = offset;
		}

		private QueryingReadContext NewQueryingReadContext()
		{
			return new QueryingReadContext(Transaction(), _handlerVersion, _bytes);
		}

		private void ReadThis(bool a_activate)
		{
			Read();
			ObjectContainerBase container = Transaction().Container();
			_member = container.TryGetByID(Transaction(), _key);
			if (_member != null && (a_activate || _member is ICompare))
			{
				container.Activate(Transaction(), _member);
				CheckInstanceOfCompare();
			}
		}

		internal virtual ClassMetadata ReadYapClass()
		{
			if (_yapClass == null)
			{
				Read();
				if (_bytes != null)
				{
					Seek(0);
					ObjectContainerBase stream = Container();
					ObjectHeader objectHeader = new ObjectHeader(stream, _bytes);
					_yapClass = objectHeader.ClassMetadata();
					if (_yapClass != null)
					{
						if (stream._handlers.IclassCompare.IsAssignableFrom(_yapClass.ClassReflector()))
						{
							ReadThis(false);
						}
					}
				}
			}
			return _yapClass;
		}

		public override string ToString()
		{
			string str = "QCandidate ";
			if (_yapClass != null)
			{
				str += "\n   YapClass " + _yapClass.GetName();
			}
			if (_yapField != null)
			{
				str += "\n   YapField " + _yapField.GetName();
			}
			if (_member != null)
			{
				str += "\n   Member " + _member.ToString();
			}
			if (_root != null)
			{
				str += "\n  rooted by:\n";
				str += _root.ToString();
			}
			else
			{
				str += "\n  ROOT";
			}
			return str;
		}

		internal virtual void UseField(QField a_field)
		{
			Read();
			if (_bytes == null)
			{
				_yapField = null;
				return;
			}
			ReadYapClass();
			_member = null;
			if (a_field == null)
			{
				_yapField = null;
				return;
			}
			if (_yapClass == null)
			{
				_yapField = null;
				return;
			}
			_yapField = a_field.GetYapField(_yapClass);
			if (_yapField == null)
			{
				FieldNotFound();
				return;
			}
			HandlerVersion handlerVersion = _yapClass.SeekToField(Transaction(), _bytes, _yapField
				);
			if (handlerVersion == HandlerVersion.Invalid)
			{
				FieldNotFound();
				return;
			}
			_handlerVersion = handlerVersion._number;
		}

		private void FieldNotFound()
		{
			if (_yapClass.HoldsAnyClass())
			{
				// retry finding the field on reading the value 
				_yapField = null;
			}
			else
			{
				// we can't get a value for the field, comparisons should definitely run against null
				_yapField = new NullFieldMetadata();
			}
			_handlerVersion = HandlerRegistry.HandlerVersion;
		}

		internal virtual object Value()
		{
			return Value(false);
		}

		// TODO: This is only used for Evaluations. Handling may need
		// to be different for collections also.
		internal virtual object Value(bool a_activate)
		{
			if (_member == null)
			{
				if (_yapField == null)
				{
					ReadThis(a_activate);
				}
				else
				{
					int offset = CurrentOffSet();
					_member = _yapField.Read(NewQueryingReadContext());
					Seek(offset);
					CheckInstanceOfCompare();
				}
			}
			return _member;
		}

		internal virtual void SetBytes(ByteArrayBuffer bytes)
		{
			_bytes = bytes;
		}

		private Db4objects.Db4o.Internal.Marshall.MarshallerFamily MarshallerFamily()
		{
			return Db4objects.Db4o.Internal.Marshall.MarshallerFamily.Version(_handlerVersion
				);
		}
	}
}
