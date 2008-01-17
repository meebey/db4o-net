/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QConUnconditional : QCon
	{
		private bool _value;

		public QConUnconditional()
		{
		}

		public QConUnconditional(Transaction trans, bool value) : base(trans)
		{
			// cannot be final for C/S unmarshalling
			// C/S only
			_value = value;
		}

		internal override void EvaluateSimpleExec(QCandidates a_candidates)
		{
			a_candidates.Filter(this);
		}

		internal override bool Evaluate(QCandidate a_candidate)
		{
			return _value;
		}
	}
}
