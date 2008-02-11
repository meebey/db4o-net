/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Internal.Fieldindex;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Fieldindex
{
	public class FieldIndexProcessor
	{
		private readonly QCandidates _candidates;

		public FieldIndexProcessor(QCandidates candidates)
		{
			_candidates = candidates;
		}

		public virtual FieldIndexProcessorResult Run()
		{
			IIndexedNode bestIndex = SelectBestIndex();
			if (null == bestIndex)
			{
				return FieldIndexProcessorResult.NoIndexFound;
			}
			if (bestIndex.ResultSize() > 0)
			{
				IIndexedNode resolved = ResolveFully(bestIndex);
				if (null == resolved)
				{
					return FieldIndexProcessorResult.NoIndexFound;
				}
				return new FieldIndexProcessorResult(resolved);
			}
			return FieldIndexProcessorResult.FoundIndexButNoMatch;
		}

		private IIndexedNode ResolveFully(IIndexedNode bestIndex)
		{
			if (null == bestIndex)
			{
				return null;
			}
			if (bestIndex.IsResolved())
			{
				return bestIndex;
			}
			return ResolveFully(bestIndex.Resolve());
		}

		public virtual IIndexedNode SelectBestIndex()
		{
			IEnumerator i = CollectIndexedNodes();
			if (!i.MoveNext())
			{
				return null;
			}
			IIndexedNode best = (IIndexedNode)i.Current;
			while (i.MoveNext())
			{
				IIndexedNode leaf = (IIndexedNode)i.Current;
				if (leaf.ResultSize() < best.ResultSize())
				{
					best = leaf;
				}
			}
			return best;
		}

		public virtual IEnumerator CollectIndexedNodes()
		{
			return new IndexedNodeCollector(_candidates).GetNodes();
		}
	}
}
