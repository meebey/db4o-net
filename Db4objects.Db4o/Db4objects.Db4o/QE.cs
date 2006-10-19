namespace Db4objects.Db4o
{
	/// <summary>Query Evaluator - Represents such things as &gt;, &gt;=, &lt;, &lt;=, EQUAL, LIKE, etc.
	/// 	</summary>
	/// <remarks>Query Evaluator - Represents such things as &gt;, &gt;=, &lt;, &lt;=, EQUAL, LIKE, etc.
	/// 	</remarks>
	/// <exclude></exclude>
	public class QE : Db4objects.Db4o.Types.IUnversioned
	{
		internal static readonly Db4objects.Db4o.QE DEFAULT = new Db4objects.Db4o.QE();

		public const int NULLS = 0;

		public const int SMALLER = 1;

		public const int EQUAL = 2;

		public const int GREATER = 3;

		internal virtual Db4objects.Db4o.QE Add(Db4objects.Db4o.QE evaluator)
		{
			return evaluator;
		}

		public virtual bool Identity()
		{
			return false;
		}

		internal virtual bool IsDefault()
		{
			return true;
		}

		internal virtual bool Evaluate(Db4objects.Db4o.QConObject a_constraint, Db4objects.Db4o.QCandidate
			 a_candidate, object a_value)
		{
			if (a_value == null)
			{
				return a_constraint.GetComparator(a_candidate) is Db4objects.Db4o.Null;
			}
			return a_constraint.GetComparator(a_candidate).IsEqual(a_value);
		}

		public override bool Equals(object obj)
		{
			return obj.GetType() == this.GetType();
		}

		internal virtual bool Not(bool res)
		{
			return res;
		}

		/// <summary>Specifies which part of the index to take.</summary>
		/// <remarks>
		/// Specifies which part of the index to take.
		/// Array elements:
		/// [0] - smaller
		/// [1] - equal
		/// [2] - greater
		/// [3] - nulls
		/// </remarks>
		/// <param name="bits"></param>
		public virtual void IndexBitMap(bool[] bits)
		{
			bits[Db4objects.Db4o.QE.EQUAL] = true;
		}

		public virtual bool SupportsIndex()
		{
			return true;
		}
	}
}
