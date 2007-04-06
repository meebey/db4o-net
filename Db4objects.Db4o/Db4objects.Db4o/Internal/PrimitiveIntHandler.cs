using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;

namespace Db4objects.Db4o.Internal
{
	/// <summary>
	/// improves YInt performance slightly by not doing checks
	/// and by doing the comparison with a substraction
	/// </summary>
	/// <exclude></exclude>
	public class PrimitiveIntHandler : IntHandler
	{
		public PrimitiveIntHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override IComparable4 PrepareComparison(object obj)
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
