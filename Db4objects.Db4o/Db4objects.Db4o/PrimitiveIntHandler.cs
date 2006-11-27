namespace Db4objects.Db4o
{
	/// <summary>
	/// improves YInt performance slightly by not doing checks
	/// and by doing the comparison with a substraction
	/// </summary>
	/// <exclude></exclude>
	public sealed class PrimitiveIntHandler : Db4objects.Db4o.YInt
	{
		public PrimitiveIntHandler(Db4objects.Db4o.YapStream stream) : base(stream)
		{
		}

		public override Db4objects.Db4o.IYapComparable PrepareComparison(object obj)
		{
			_currentInteger = ((int)obj);
			_currentInt = _currentInteger;
			return this;
		}

		private int _currentInteger;

		private int _currentInt;

		public override int CompareTo(object obj)
		{
			return Val(obj) - _currentInt;
		}

		public override object Current()
		{
			return _currentInteger;
		}

		public override bool IsEqual(object obj)
		{
			return Val(obj) == _currentInt;
		}

		public override bool IsGreater(object obj)
		{
			return Val(obj) > _currentInt;
		}

		public override bool IsSmaller(object obj)
		{
			return Val(obj) < _currentInt;
		}
	}
}
