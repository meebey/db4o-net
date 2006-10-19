namespace Db4objects.Db4o
{
	/// <summary>Class constraint on queries</summary>
	/// <exclude></exclude>
	public class QConClass : Db4objects.Db4o.QConObject
	{
		[Db4objects.Db4o.Transient]
		private Db4objects.Db4o.Reflect.IReflectClass _claxx;

		public string _className;

		public bool i_equal;

		public QConClass()
		{
		}

		internal QConClass(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.QCon a_parent
			, Db4objects.Db4o.QField a_field, Db4objects.Db4o.Reflect.IReflectClass claxx) : 
			base(a_trans, a_parent, a_field, null)
		{
			if (claxx != null)
			{
				i_yapClass = a_trans.Stream().GetYapClass(claxx, true);
				if (claxx.Equals(a_trans.Stream().i_handlers.ICLASS_OBJECT))
				{
					i_yapClass = (Db4objects.Db4o.YapClass)((Db4objects.Db4o.YapClassPrimitive)i_yapClass
						).i_handler;
				}
			}
			_claxx = claxx;
		}

		public override bool CanBeIndexLeaf()
		{
			return false;
		}

		internal override bool Evaluate(Db4objects.Db4o.QCandidate a_candidate)
		{
			bool res = true;
			Db4objects.Db4o.Reflect.IReflectClass claxx = a_candidate.ClassReflector();
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
			if (i_evaluator.IsDefault())
			{
				if (i_orderID == 0 && !HasJoins())
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
			i_candidates.Filter(this);
		}

		public override Db4objects.Db4o.Query.IConstraint Equal()
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
			return "";
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
			return base.ToString();
			string str = "QConClass ";
			if (_claxx != null)
			{
				str += _claxx.ToString() + " ";
			}
			return str + base.ToString();
		}

		internal override void Unmarshall(Db4objects.Db4o.Transaction a_trans)
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
	}
}
