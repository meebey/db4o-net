namespace Db4objects.Db4o.Inside.Fieldindex
{
	public class FieldIndexProcessor
	{
		private readonly Db4objects.Db4o.QCandidates _candidates;

		public FieldIndexProcessor(Db4objects.Db4o.QCandidates candidates)
		{
			_candidates = candidates;
		}

		public virtual Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult Run()
		{
			Db4objects.Db4o.Inside.Fieldindex.IIndexedNode bestIndex = SelectBestIndex();
			if (null == bestIndex)
			{
				return Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult.NO_INDEX_FOUND;
			}
			if (bestIndex.ResultSize() > 0)
			{
				Db4objects.Db4o.Inside.Fieldindex.IIndexedNode resolved = ResolveFully(bestIndex);
				if (null == resolved)
				{
					return Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult.NO_INDEX_FOUND;
				}
				return new Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult(resolved);
			}
			return Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult.FOUND_INDEX_BUT_NO_MATCH;
		}

		private Db4objects.Db4o.Inside.Fieldindex.IIndexedNode ResolveFully(Db4objects.Db4o.Inside.Fieldindex.IIndexedNode
			 bestIndex)
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

		public virtual Db4objects.Db4o.Inside.Fieldindex.IIndexedNode SelectBestIndex()
		{
			System.Collections.IEnumerator i = CollectIndexedNodes();
			if (!i.MoveNext())
			{
				return null;
			}
			Db4objects.Db4o.Inside.Fieldindex.IIndexedNode best = (Db4objects.Db4o.Inside.Fieldindex.IIndexedNode
				)i.Current;
			while (i.MoveNext())
			{
				Db4objects.Db4o.Inside.Fieldindex.IIndexedNode leaf = (Db4objects.Db4o.Inside.Fieldindex.IIndexedNode
					)i.Current;
				if (leaf.ResultSize() < best.ResultSize())
				{
					best = leaf;
				}
			}
			return best;
		}

		public virtual System.Collections.IEnumerator CollectIndexedNodes()
		{
			return new Db4objects.Db4o.Inside.Fieldindex.IndexedNodeCollector(_candidates).GetNodes
				();
		}
	}
}
