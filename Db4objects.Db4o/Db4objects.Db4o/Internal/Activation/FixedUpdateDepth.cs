/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Internal.Activation
{
	public sealed class FixedUpdateDepth : IUpdateDepth
	{
		private int _depth;

		public FixedUpdateDepth(int depth)
		{
			_depth = depth;
		}

		public bool SufficientDepth()
		{
			return _depth > 0;
		}

		public bool Negative()
		{
			// should never happen?
			return _depth < 0;
		}

		public override string ToString()
		{
			return GetType().FullName + ": " + _depth;
		}

		public IUpdateDepth Adjust(ClassMetadata clazz)
		{
			if (clazz.CascadesOnDeleteOrUpdate())
			{
				return AdjustDepthToBorders().Descend();
			}
			return Descend();
		}

		public bool IsBroaderThan(Db4objects.Db4o.Internal.Activation.FixedUpdateDepth other
			)
		{
			return _depth > other._depth;
		}

		// TODO code duplication in fixed activation/update depth
		public Db4objects.Db4o.Internal.Activation.FixedUpdateDepth AdjustDepthToBorders(
			)
		{
			return new Db4objects.Db4o.Internal.Activation.FixedUpdateDepth(DepthUtil.AdjustDepthToBorders
				(_depth));
		}

		public IUpdateDepth AdjustUpdateDepthForCascade(bool isCollection)
		{
			int minimumUpdateDepth = isCollection ? 2 : 1;
			if (_depth < minimumUpdateDepth)
			{
				return new Db4objects.Db4o.Internal.Activation.FixedUpdateDepth(minimumUpdateDepth
					);
			}
			return this;
		}

		public IUpdateDepth Descend()
		{
			return new Db4objects.Db4o.Internal.Activation.FixedUpdateDepth(_depth - 1);
		}

		public override bool Equals(object other)
		{
			if (this == other)
			{
				return true;
			}
			if (other == null || GetType() != other.GetType())
			{
				return false;
			}
			return _depth == ((Db4objects.Db4o.Internal.Activation.FixedUpdateDepth)other)._depth;
		}

		public override int GetHashCode()
		{
			return _depth;
		}
	}
}
