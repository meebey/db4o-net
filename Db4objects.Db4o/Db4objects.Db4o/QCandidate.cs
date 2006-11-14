namespace Db4objects.Db4o
{
	/// <summary>Represents an actual object in the database.</summary>
	/// <remarks>
	/// Represents an actual object in the database. Forms a tree structure, indexed
	/// by id. Can have dependents that are doNotInclude'd in the query result when
	/// this is doNotInclude'd.
	/// </remarks>
	/// <exclude></exclude>
	public class QCandidate : Db4objects.Db4o.TreeInt, Db4objects.Db4o.Query.ICandidate
		, Db4objects.Db4o.IOrderable
	{
		internal Db4objects.Db4o.YapReader _bytes;

		internal readonly Db4objects.Db4o.QCandidates _candidates;

		private Db4objects.Db4o.Foundation.List4 _dependants;

		internal bool _include = true;

		private object _member;

		internal Db4objects.Db4o.IOrderable _order;

		internal Db4objects.Db4o.Foundation.Tree _pendingJoins;

		private Db4objects.Db4o.QCandidate _root;

		internal Db4objects.Db4o.YapClass _yapClass;

		internal Db4objects.Db4o.YapField _yapField;

		internal Db4objects.Db4o.Inside.Marshall.MarshallerFamily _marshallerFamily;

		private QCandidate(Db4objects.Db4o.QCandidates qcandidates) : base(0)
		{
			_candidates = qcandidates;
		}

		private QCandidate() : this(null)
		{
		}

		public QCandidate(Db4objects.Db4o.QCandidates candidates, object obj, int id, bool
			 include) : base(id)
		{
			_candidates = candidates;
			_order = this;
			_member = obj;
			_include = include;
			if (id == 0)
			{
				_key = candidates.GenerateCandidateId();
			}
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.QCandidate qcan = new Db4objects.Db4o.QCandidate(_candidates);
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

		internal virtual void AddDependant(Db4objects.Db4o.QCandidate a_candidate)
		{
			_dependants = new Db4objects.Db4o.Foundation.List4(_dependants, a_candidate);
		}

		private void CheckInstanceOfCompare()
		{
			if (_member is Db4objects.Db4o.Config.ICompare)
			{
				_member = ((Db4objects.Db4o.Config.ICompare)_member).Compare();
				Db4objects.Db4o.YapFile stream = GetStream();
				_yapClass = stream.GetYapClass(stream.Reflector().ForObject(_member), false);
				_key = (int)stream.GetID(_member);
				SetBytes(stream.ReadReaderByID(GetTransaction(), _key));
			}
		}

		public override int Compare(Db4objects.Db4o.Foundation.Tree a_to)
		{
			return _order.CompareTo(((Db4objects.Db4o.QCandidate)a_to)._order);
		}

		public virtual int CompareTo(object a_object)
		{
			return _key - ((Db4objects.Db4o.TreeInt)a_object)._key;
		}

		internal virtual bool CreateChild(Db4objects.Db4o.QCandidates a_candidates)
		{
			if (!_include)
			{
				return false;
			}
			if (_yapField != null)
			{
				Db4objects.Db4o.ITypeHandler4 handler = _yapField.GetHandler();
				if (handler != null)
				{
					Db4objects.Db4o.YapReader[] arrayBytes = { _bytes };
					Db4objects.Db4o.ITypeHandler4 arrayHandler = handler.ReadArrayHandler(GetTransaction
						(), _marshallerFamily, arrayBytes);
					if (arrayHandler != null)
					{
						int offset = arrayBytes[0]._offset;
						bool outerRes = true;
						System.Collections.IEnumerator i = a_candidates.IterateConstraints();
						while (i.MoveNext())
						{
							Db4objects.Db4o.QCon qcon = (Db4objects.Db4o.QCon)i.Current;
							Db4objects.Db4o.QField qf = qcon.GetField();
							if (qf == null || qf.i_name.Equals(_yapField.GetName()))
							{
								Db4objects.Db4o.QCon tempParent = qcon.i_parent;
								qcon.SetParent(null);
								Db4objects.Db4o.QCandidates candidates = new Db4objects.Db4o.QCandidates(a_candidates
									.i_trans, null, qf);
								candidates.AddConstraint(qcon);
								qcon.SetCandidates(candidates);
								arrayHandler.ReadCandidates(_marshallerFamily, arrayBytes[0], candidates);
								arrayBytes[0]._offset = offset;
								bool isNot = qcon.IsNot();
								if (isNot)
								{
									qcon.RemoveNot();
								}
								candidates.Evaluate();
								Db4objects.Db4o.Foundation.Tree[] pending = new Db4objects.Db4o.Foundation.Tree[1
									];
								bool[] innerRes = { isNot };
								candidates.Traverse(new _AnonymousInnerClass167(this, innerRes, isNot, pending));
								if (isNot)
								{
									qcon.Not();
								}
								if (pending[0] != null)
								{
									pending[0].Traverse(new _AnonymousInnerClass236(this));
								}
								if (!innerRes[0])
								{
									qcon.Visit(GetRoot(), qcon.Evaluator().Not(false));
									outerRes = false;
								}
								qcon.SetParent(tempParent);
							}
						}
						return outerRes;
					}
					if (handler.GetTypeID() == Db4objects.Db4o.YapConst.TYPE_SIMPLE)
					{
						a_candidates.i_currentConstraint.Visit(this);
						return true;
					}
				}
			}
			if (_yapField == null || _yapField is Db4objects.Db4o.YapFieldNull)
			{
				return false;
			}
			_yapClass.FindOffset(_bytes, _yapField);
			Db4objects.Db4o.QCandidate candidate = ReadSubCandidate(a_candidates);
			if (candidate == null)
			{
				return false;
			}
			if (a_candidates.i_yapClass != null && a_candidates.i_yapClass.IsStrongTyped())
			{
				if (_yapField != null)
				{
					Db4objects.Db4o.ITypeHandler4 handler = _yapField.GetHandler();
					if (handler != null && (handler.GetTypeID() == Db4objects.Db4o.YapConst.TYPE_CLASS
						))
					{
						Db4objects.Db4o.YapClass yc = (Db4objects.Db4o.YapClass)handler;
						if (yc is Db4objects.Db4o.YapClassAny)
						{
							yc = candidate.ReadYapClass();
						}
						if (yc == null)
						{
							return false;
						}
						if (!yc.CanHold(a_candidates.i_yapClass.ClassReflector()))
						{
							return false;
						}
					}
				}
			}
			AddDependant(a_candidates.AddByIdentity(candidate));
			return true;
		}

		private sealed class _AnonymousInnerClass167 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass167(QCandidate _enclosing, bool[] innerRes, bool isNot
				, Db4objects.Db4o.Foundation.Tree[] pending)
			{
				this._enclosing = _enclosing;
				this.innerRes = innerRes;
				this.isNot = isNot;
				this.pending = pending;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.QCandidate cand = (Db4objects.Db4o.QCandidate)obj;
				if (cand.Include())
				{
					innerRes[0] = !isNot;
				}
				if (cand._pendingJoins != null)
				{
					cand._pendingJoins.Traverse(new _AnonymousInnerClass180(this, pending));
				}
			}

			private sealed class _AnonymousInnerClass180 : Db4objects.Db4o.Foundation.IVisitor4
			{
				public _AnonymousInnerClass180(_AnonymousInnerClass167 _enclosing, Db4objects.Db4o.Foundation.Tree[]
					 pending)
				{
					this._enclosing = _enclosing;
					this.pending = pending;
				}

				public void Visit(object a_object)
				{
					Db4objects.Db4o.QPending newPending = (Db4objects.Db4o.QPending)a_object;
					newPending.ChangeConstraint();
					Db4objects.Db4o.QPending oldPending = (Db4objects.Db4o.QPending)Db4objects.Db4o.Foundation.Tree
						.Find(pending[0], newPending);
					if (oldPending != null)
					{
						if (oldPending._result != newPending._result)
						{
							oldPending._result = Db4objects.Db4o.QPending.BOTH;
						}
					}
					else
					{
						pending[0] = Db4objects.Db4o.Foundation.Tree.Add(pending[0], newPending);
					}
				}

				private readonly _AnonymousInnerClass167 _enclosing;

				private readonly Db4objects.Db4o.Foundation.Tree[] pending;
			}

			private readonly QCandidate _enclosing;

			private readonly bool[] innerRes;

			private readonly bool isNot;

			private readonly Db4objects.Db4o.Foundation.Tree[] pending;
		}

		private sealed class _AnonymousInnerClass236 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass236(QCandidate _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				this._enclosing.GetRoot().Evaluate((Db4objects.Db4o.QPending)a_object);
			}

			private readonly QCandidate _enclosing;
		}

		internal virtual void DoNotInclude()
		{
			_include = false;
			if (_dependants != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(_dependants
					);
				_dependants = null;
				while (i.MoveNext())
				{
					((Db4objects.Db4o.QCandidate)i.Current).DoNotInclude();
				}
			}
		}

		public override bool Duplicates()
		{
			return _order.HasDuplicates();
		}

		internal virtual bool Evaluate(Db4objects.Db4o.QConObject a_constraint, Db4objects.Db4o.QE
			 a_evaluator)
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

		internal virtual bool Evaluate(Db4objects.Db4o.QPending a_pending)
		{
			Db4objects.Db4o.QPending oldPending = (Db4objects.Db4o.QPending)Db4objects.Db4o.Foundation.Tree
				.Find(_pendingJoins, a_pending);
			if (oldPending == null)
			{
				a_pending.ChangeConstraint();
				_pendingJoins = Db4objects.Db4o.Foundation.Tree.Add(_pendingJoins, a_pending);
				return true;
			}
			_pendingJoins = _pendingJoins.RemoveNode(oldPending);
			oldPending._join.EvaluatePending(this, oldPending, a_pending._result);
			return false;
		}

		internal virtual Db4objects.Db4o.Reflect.IReflectClass ClassReflector()
		{
			ReadYapClass();
			if (_yapClass == null)
			{
				return null;
			}
			return _yapClass.ClassReflector();
		}

		public virtual Db4objects.Db4o.IObjectContainer ObjectContainer()
		{
			return GetStream();
		}

		public virtual object GetObject()
		{
			object obj = Value(true);
			if (obj is Db4objects.Db4o.YapReader)
			{
				Db4objects.Db4o.YapReader reader = (Db4objects.Db4o.YapReader)obj;
				int offset = reader._offset;
				obj = _marshallerFamily._string.ReadFromOwnSlot(GetStream(), reader);
				reader._offset = offset;
			}
			return obj;
		}

		internal virtual Db4objects.Db4o.QCandidate GetRoot()
		{
			return _root == null ? this : _root;
		}

		private Db4objects.Db4o.YapFile GetStream()
		{
			return GetTransaction().i_file;
		}

		private Db4objects.Db4o.Transaction GetTransaction()
		{
			return _candidates.i_trans;
		}

		public virtual bool HasDuplicates()
		{
			return _root != null;
		}

		public virtual void HintOrder(int a_order, bool a_major)
		{
			_order = new Db4objects.Db4o.Order();
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
			_include = flag;
		}

		public override void IsDuplicateOf(Db4objects.Db4o.Foundation.Tree a_tree)
		{
			_size = 0;
			_root = (Db4objects.Db4o.QCandidate)a_tree;
		}

		private Db4objects.Db4o.Reflect.IReflectClass MemberClass()
		{
			return GetTransaction().Reflector().ForObject(_member);
		}

		internal virtual Db4objects.Db4o.IYapComparable PrepareComparison(Db4objects.Db4o.YapStream
			 a_stream, object a_constraint)
		{
			if (_yapField != null)
			{
				return _yapField.PrepareComparison(a_constraint);
			}
			if (_yapClass == null)
			{
				Db4objects.Db4o.YapClass yc = null;
				if (_bytes != null)
				{
					yc = a_stream.GetYapClass(a_stream.Reflector().ForObject(a_constraint), true);
				}
				else
				{
					if (_member != null)
					{
						yc = a_stream.GetYapClass(a_stream.Reflector().ForObject(_member), false);
					}
				}
				if (yc != null)
				{
					if (_member != null && _member.GetType().IsArray)
					{
						Db4objects.Db4o.ITypeHandler4 ydt = (Db4objects.Db4o.ITypeHandler4)yc.PrepareComparison
							(a_constraint);
						if (a_stream.Reflector().Array().IsNDimensional(MemberClass()))
						{
							Db4objects.Db4o.YapArrayN yan = new Db4objects.Db4o.YapArrayN(a_stream, ydt, false
								);
							return yan;
						}
						Db4objects.Db4o.YapArray ya = new Db4objects.Db4o.YapArray(a_stream, ydt, false);
						return ya;
					}
					return yc.PrepareComparison(a_constraint);
				}
				return null;
			}
			return _yapClass.PrepareComparison(a_constraint);
		}

		private void Read()
		{
			if (_include)
			{
				if (_bytes == null)
				{
					if (_key > 0)
					{
						SetBytes(GetStream().ReadReaderByID(GetTransaction(), _key));
						if (_bytes == null)
						{
							_include = false;
						}
					}
					else
					{
						_include = false;
					}
				}
			}
		}

		private Db4objects.Db4o.QCandidate ReadSubCandidate(Db4objects.Db4o.QCandidates candidateCollection
			)
		{
			Read();
			if (_bytes != null)
			{
				Db4objects.Db4o.QCandidate subCandidate = null;
				int offset = _bytes._offset;
				try
				{
					subCandidate = _yapField.i_handler.ReadSubCandidate(_marshallerFamily, _bytes, candidateCollection
						, false);
				}
				catch
				{
					return null;
				}
				_bytes._offset = offset;
				if (subCandidate != null)
				{
					subCandidate._root = GetRoot();
					return subCandidate;
				}
			}
			return null;
		}

		private void ReadThis(bool a_activate)
		{
			Read();
			Db4objects.Db4o.Transaction trans = GetTransaction();
			if (trans != null)
			{
				_member = trans.Stream().GetByID1(trans, _key);
				if (_member != null && (a_activate || _member is Db4objects.Db4o.Config.ICompare)
					)
				{
					trans.Stream().Activate1(trans, _member);
					CheckInstanceOfCompare();
				}
			}
		}

		internal virtual Db4objects.Db4o.YapClass ReadYapClass()
		{
			if (_yapClass == null)
			{
				Read();
				if (_bytes != null)
				{
					_bytes._offset = 0;
					Db4objects.Db4o.YapStream stream = GetStream();
					Db4objects.Db4o.Inside.Marshall.ObjectHeader objectHeader = new Db4objects.Db4o.Inside.Marshall.ObjectHeader
						(stream, _bytes);
					_yapClass = objectHeader.YapClass();
					if (_yapClass != null)
					{
						if (stream.i_handlers.ICLASS_COMPARE.IsAssignableFrom(_yapClass.ClassReflector())
							)
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
			return base.ToString();
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

		internal virtual void UseField(Db4objects.Db4o.QField a_field)
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
			_marshallerFamily = _yapClass.FindOffset(_bytes, _yapField);
			if (_yapField == null || _marshallerFamily == null)
			{
				if (_yapClass.HoldsAnyClass())
				{
					_yapField = null;
				}
				else
				{
					_yapField = new Db4objects.Db4o.YapFieldNull();
				}
			}
		}

		internal virtual object Value()
		{
			return Value(false);
		}

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
					int offset = _bytes._offset;
					try
					{
						_member = _yapField.ReadQuery(GetTransaction(), _marshallerFamily, _bytes);
					}
					catch (Db4objects.Db4o.CorruptionException)
					{
						_member = null;
					}
					_bytes._offset = offset;
					CheckInstanceOfCompare();
				}
			}
			return _member;
		}

		internal virtual void SetBytes(Db4objects.Db4o.YapReader bytes)
		{
			_bytes = bytes;
		}
	}
}
