/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Internal.Activation
{
	public class TPFixedUpdateDepth : FixedUpdateDepth
	{
		private bool _tpCommit;

		public TPFixedUpdateDepth(int depth, bool tpCommit) : base(depth)
		{
			_tpCommit = tpCommit;
		}

		internal virtual void TpCommit(bool tpCommit)
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
			return new Db4objects.Db4o.Internal.Activation.TPFixedUpdateDepth(depth, _tpCommit
				);
		}
	}
}
