namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <summary>Array of constraints for queries.</summary>
	/// <remarks>
	/// Array of constraints for queries.
	/// Necessary to be returned to Query#constraints()
	/// </remarks>
	/// <exclude></exclude>
	public class QConstraints : Db4objects.Db4o.Internal.Query.Processor.QCon, Db4objects.Db4o.Query.IConstraints
	{
		private Db4objects.Db4o.Query.IConstraint[] i_constraints;

		internal QConstraints(Db4objects.Db4o.Internal.Transaction a_trans, Db4objects.Db4o.Query.IConstraint[]
			 constraints) : base(a_trans)
		{
			i_constraints = constraints;
		}

		internal override Db4objects.Db4o.Query.IConstraint Join(Db4objects.Db4o.Query.IConstraint
			 a_with, bool a_and)
		{
			lock (StreamLock())
			{
				if (!(a_with is Db4objects.Db4o.Internal.Query.Processor.QCon))
				{
					return null;
				}
				return ((Db4objects.Db4o.Internal.Query.Processor.QCon)a_with).Join1(this, a_and);
			}
		}

		public virtual Db4objects.Db4o.Query.IConstraint[] ToArray()
		{
			lock (StreamLock())
			{
				return i_constraints;
			}
		}

		public override Db4objects.Db4o.Query.IConstraint Contains()
		{
			lock (StreamLock())
			{
				for (int i = 0; i < i_constraints.Length; i++)
				{
					i_constraints[i].Contains();
				}
				return this;
			}
		}

		public override Db4objects.Db4o.Query.IConstraint Equal()
		{
			lock (StreamLock())
			{
				for (int i = 0; i < i_constraints.Length; i++)
				{
					i_constraints[i].Equal();
				}
				return this;
			}
		}

		public override Db4objects.Db4o.Query.IConstraint Greater()
		{
			lock (StreamLock())
			{
				for (int i = 0; i < i_constraints.Length; i++)
				{
					i_constraints[i].Greater();
				}
				return this;
			}
		}

		public override Db4objects.Db4o.Query.IConstraint Identity()
		{
			lock (StreamLock())
			{
				for (int i = 0; i < i_constraints.Length; i++)
				{
					i_constraints[i].Identity();
				}
				return this;
			}
		}

		public override Db4objects.Db4o.Query.IConstraint Not()
		{
			lock (StreamLock())
			{
				for (int i = 0; i < i_constraints.Length; i++)
				{
					i_constraints[i].Not();
				}
				return this;
			}
		}

		public override Db4objects.Db4o.Query.IConstraint Like()
		{
			lock (StreamLock())
			{
				for (int i = 0; i < i_constraints.Length; i++)
				{
					i_constraints[i].Like();
				}
				return this;
			}
		}

		public override Db4objects.Db4o.Query.IConstraint StartsWith(bool caseSensitive)
		{
			lock (StreamLock())
			{
				for (int i = 0; i < i_constraints.Length; i++)
				{
					i_constraints[i].StartsWith(caseSensitive);
				}
				return this;
			}
		}

		public override Db4objects.Db4o.Query.IConstraint EndsWith(bool caseSensitive)
		{
			lock (StreamLock())
			{
				for (int i = 0; i < i_constraints.Length; i++)
				{
					i_constraints[i].EndsWith(caseSensitive);
				}
				return this;
			}
		}

		public override Db4objects.Db4o.Query.IConstraint Smaller()
		{
			lock (StreamLock())
			{
				for (int i = 0; i < i_constraints.Length; i++)
				{
					i_constraints[i].Smaller();
				}
				return this;
			}
		}

		public override object GetObject()
		{
			lock (StreamLock())
			{
				object[] objects = new object[i_constraints.Length];
				for (int i = 0; i < i_constraints.Length; i++)
				{
					objects[i] = i_constraints[i].GetObject();
				}
				return objects;
			}
		}
	}
}
