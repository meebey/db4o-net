/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <summary>Class constraint on queries</summary>
	/// <exclude></exclude>
	public class QConClass : QConObject
	{
		[System.NonSerialized]
		private IReflectClass _claxx;

		public string _className;

		public bool i_equal;

		public QConClass()
		{
		}

		internal QConClass(Transaction a_trans, QCon a_parent, QField a_field, IReflectClass
			 claxx) : base(a_trans, a_parent, a_field, null)
		{
			// C/S
			if (claxx != null)
			{
				i_yapClass = a_trans.Container().ProduceClassMetadata(claxx);
				if (claxx.Equals(a_trans.Container()._handlers.IclassObject))
				{
					i_yapClass = (ClassMetadata)i_yapClass.TypeHandler();
				}
			}
			_claxx = claxx;
		}

		internal QConClass(Transaction trans, IReflectClass claxx) : this(trans, null, null
			, claxx)
		{
		}

		public virtual string GetClassName()
		{
			return _claxx.GetName();
		}

		public override bool CanBeIndexLeaf()
		{
			return false;
		}

		internal override bool Evaluate(QCandidate a_candidate)
		{
			bool res = true;
			IReflectClass claxx = a_candidate.ClassReflector();
			if (claxx == null)
			{
				res = false;
			}
			else
			{
				res = i_equal ? _claxx.Equals(claxx) : _claxx.IsAssignableFrom(claxx);
			}
			return i_evaluator.Not(res);
		}

		internal override void EvaluateSelf()
		{
			// optimization for simple class queries: 
			// No instantiation of objects, if not necessary.
			// Does not handle the special comparison of the
			// Compare interface.
			//
			if (i_candidates.WasLoadedFromClassIndex())
			{
				if (i_evaluator.IsDefault())
				{
					if (!HasOrdering() && !HasJoins())
					{
						if (i_yapClass != null && i_candidates.i_yapClass != null)
						{
							if (i_yapClass.GetHigherHierarchy(i_candidates.i_yapClass) == i_yapClass)
							{
								return;
							}
						}
					}
				}
			}
			i_candidates.Filter(this);
		}

		public override IConstraint Equal()
		{
			lock (StreamLock())
			{
				i_equal = true;
				return this;
			}
		}

		internal override bool IsNullConstraint()
		{
			return false;
		}

		internal override string LogObject()
		{
			return string.Empty;
		}

		internal override void Marshall()
		{
			base.Marshall();
			if (_claxx != null)
			{
				_className = _claxx.GetName();
			}
		}

		public override string ToString()
		{
			string str = "QConClass ";
			if (_claxx != null)
			{
				str += _claxx.ToString() + " ";
			}
			return str + base.ToString();
		}

		internal override void Unmarshall(Transaction a_trans)
		{
			if (i_trans == null)
			{
				base.Unmarshall(a_trans);
				if (_className != null)
				{
					_claxx = a_trans.Reflector().ForName(_className);
				}
			}
		}

		internal override void SetEvaluationMode()
		{
			IEnumerator children = IterateChildren();
			while (children.MoveNext())
			{
				object child = children.Current;
				if (child is QConObject)
				{
					((QConObject)child).SetEvaluationMode();
				}
			}
		}
	}
}
