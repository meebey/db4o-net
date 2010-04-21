/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Internal.Activation
{
	public class TPUnspecifiedUpdateDepth : UnspecifiedUpdateDepth
	{
		private bool _tpCommit;

		internal TPUnspecifiedUpdateDepth(bool tpCommit)
		{
			_tpCommit = tpCommit;
		}

		public override bool CanSkip(ClassMetadata clazz)
		{
			if (_tpCommit)
			{
				return false;
			}
			return clazz.Reflector().ForClass(typeof(IActivatable)).IsAssignableFrom(clazz.ClassReflector
				());
		}

		protected override FixedUpdateDepth ForDepth(int depth)
		{
			return new TPFixedUpdateDepth(depth, _tpCommit);
		}
	}
}
