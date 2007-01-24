namespace Db4objects.Db4o
{
	/// <summary>Join constraint on queries</summary>
	/// <exclude></exclude>
	public class QConJoin : Db4objects.Db4o.QCon
	{
		public bool i_and;

		public Db4objects.Db4o.QCon i_constraint1;

		public Db4objects.Db4o.QCon i_constraint2;

		public QConJoin()
		{
		}

		internal QConJoin(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.QCon a_c1, 
			Db4objects.Db4o.QCon a_c2, bool a_and) : base(a_trans)
		{
			i_constraint1 = a_c1;
			i_constraint2 = a_c2;
			i_and = a_and;
		}

		internal override void DoNotInclude(Db4objects.Db4o.QCandidate a_root)
		{
			i_constraint1.DoNotInclude(a_root);
			i_constraint2.DoNotInclude(a_root);
		}

		internal override void ExchangeConstraint(Db4objects.Db4o.QCon a_exchange, Db4objects.Db4o.QCon
			 a_with)
		{
			base.ExchangeConstraint(a_exchange, a_with);
			if (a_exchange == i_constraint1)
			{
				i_constraint1 = a_with;
			}
			if (a_exchange == i_constraint2)
			{
				i_constraint2 = a_with;
			}
		}

		internal virtual void EvaluatePending(Db4objects.Db4o.QCandidate a_root, Db4objects.Db4o.QPending
			 a_pending, int a_secondResult)
		{
			bool res = i_evaluator.Not(i_and ? ((a_pending._result + a_secondResult) > 0) : (
				a_pending._result + a_secondResult) > -4);
			if (HasJoins())
			{
				System.Collections.IEnumerator i = IterateJoins();
				while (i.MoveNext())
				{
					Db4objects.Db4o.QConJoin qcj = (Db4objects.Db4o.QConJoin)i.Current;
					a_root.Evaluate(new Db4objects.Db4o.QPending(qcj, this, res));
				}
			}
			else
			{
				if (!res)
				{
					i_constraint1.DoNotInclude(a_root);
					i_constraint2.DoNotInclude(a_root);
				}
			}
		}

		public virtual Db4objects.Db4o.QCon GetOtherConstraint(Db4objects.Db4o.QCon a_constraint
			)
		{
			if (a_constraint == i_constraint1)
			{
				return i_constraint2;
			}
			else
			{
				if (a_constraint == i_constraint2)
				{
					return i_constraint1;
				}
			}
			throw new System.ArgumentException();
		}

		internal override string LogObject()
		{
			return string.Empty;
		}

		internal virtual bool RemoveForParent(Db4objects.Db4o.QCon a_constraint)
		{
			if (i_and)
			{
				Db4objects.Db4o.QCon other = GetOtherConstraint(a_constraint);
				other.RemoveJoin(this);
				other.Remove();
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return base.ToString();
			string str = "QConJoin " + (i_and ? "AND " : "OR");
			if (i_constraint1 != null)
			{
				str += "\n   " + i_constraint1;
			}
			if (i_constraint2 != null)
			{
				str += "\n   " + i_constraint2;
			}
			return str;
		}

		public virtual bool IsOr()
		{
			return !i_and;
		}
	}
}
