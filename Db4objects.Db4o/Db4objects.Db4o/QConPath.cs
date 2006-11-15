namespace Db4objects.Db4o
{
	/// <summary>
	/// Placeholder for a constraint, only necessary to attach children
	/// to the query graph.
	/// </summary>
	/// <remarks>
	/// Placeholder for a constraint, only necessary to attach children
	/// to the query graph.
	/// Added upon a call to Query#descend(), if there is no
	/// other place to hook up a new constraint.
	/// </remarks>
	/// <exclude></exclude>
	public class QConPath : Db4objects.Db4o.QConClass
	{
		public QConPath()
		{
		}

		internal QConPath(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.QCon a_parent
			, Db4objects.Db4o.QField a_field) : base(a_trans, a_parent, a_field, null)
		{
			if (a_field != null)
			{
				i_yapClass = a_field.GetYapClass();
			}
		}

		public override bool CanLoadByIndex()
		{
			return false;
		}

		internal override bool Evaluate(Db4objects.Db4o.QCandidate a_candidate)
		{
			if (a_candidate.ClassReflector() == null)
			{
				VisitOnNull(a_candidate.GetRoot());
			}
			return true;
		}

		internal override void EvaluateSelf()
		{
		}

		internal override bool IsNullConstraint()
		{
			return !HasChildren();
		}

		internal override Db4objects.Db4o.QConClass ShareParentForClass(Db4objects.Db4o.Reflect.IReflectClass
			 a_class, bool[] removeExisting)
		{
			if (i_parent == null)
			{
				return null;
			}
			if (!i_field.CanHold(a_class))
			{
				return null;
			}
			Db4objects.Db4o.QConClass newConstraint = new Db4objects.Db4o.QConClass(i_trans, 
				i_parent, i_field, a_class);
			Morph(removeExisting, newConstraint, a_class);
			return newConstraint;
		}

		internal override Db4objects.Db4o.QCon ShareParent(object a_object, bool[] removeExisting
			)
		{
			if (i_parent == null)
			{
				return null;
			}
			object obj = i_field.Coerce(a_object);
			if (obj == Db4objects.Db4o.Foundation.No4.INSTANCE)
			{
				Db4objects.Db4o.QConObject falseConstraint = new Db4objects.Db4o.QConFalse(i_trans
					, i_parent, i_field);
				Morph(removeExisting, falseConstraint, ReflectClassForObject(obj));
				return falseConstraint;
			}
			Db4objects.Db4o.QConObject newConstraint = new Db4objects.Db4o.QConObject(i_trans
				, i_parent, i_field, obj);
			Morph(removeExisting, newConstraint, ReflectClassForObject(obj));
			return newConstraint;
		}

		private Db4objects.Db4o.Reflect.IReflectClass ReflectClassForObject(object obj)
		{
			return i_trans.Reflector().ForObject(obj);
		}

		private void Morph(bool[] removeExisting, Db4objects.Db4o.QConObject newConstraint
			, Db4objects.Db4o.Reflect.IReflectClass claxx)
		{
			bool mayMorph = true;
			if (claxx != null)
			{
				Db4objects.Db4o.YapClass yc = i_trans.Stream().GetYapClass(claxx, true);
				if (yc != null)
				{
					System.Collections.IEnumerator i = IterateChildren();
					while (i.MoveNext())
					{
						Db4objects.Db4o.QField qf = ((Db4objects.Db4o.QCon)i.Current).GetField();
						if (!yc.HasField(i_trans.Stream(), qf.i_name))
						{
							mayMorph = false;
							break;
						}
					}
				}
			}
			if (mayMorph)
			{
				System.Collections.IEnumerator j = IterateChildren();
				while (j.MoveNext())
				{
					newConstraint.AddConstraint((Db4objects.Db4o.QCon)j.Current);
				}
				if (HasJoins())
				{
					System.Collections.IEnumerator k = IterateJoins();
					while (k.MoveNext())
					{
						Db4objects.Db4o.QConJoin qcj = (Db4objects.Db4o.QConJoin)k.Current;
						qcj.ExchangeConstraint(this, newConstraint);
						newConstraint.AddJoin(qcj);
					}
				}
				i_parent.ExchangeConstraint(this, newConstraint);
				removeExisting[0] = true;
			}
			else
			{
				i_parent.AddConstraint(newConstraint);
			}
		}

		internal sealed override bool VisitSelfOnNull()
		{
			return false;
		}

		public override string ToString()
		{
			return base.ToString();
			return "QConPath " + base.ToString();
		}
	}
}
