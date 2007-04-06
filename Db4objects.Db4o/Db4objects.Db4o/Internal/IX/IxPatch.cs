using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.IX;

namespace Db4objects.Db4o.Internal.IX
{
	/// <summary>Node for index tree, can be addition or removal node</summary>
	public abstract class IxPatch : IxTree
	{
		internal int _parentID;

		internal object _value;

		private IQueue4 _queue;

		internal IxPatch(IndexTransaction a_ft, int a_parentID, object a_value) : base(a_ft
			)
		{
			_parentID = a_parentID;
			_value = a_value;
		}

		public override Tree Add(Tree a_new)
		{
			int cmp = Compare(a_new);
			if (cmp == 0)
			{
				Db4objects.Db4o.Internal.IX.IxPatch patch = (Db4objects.Db4o.Internal.IX.IxPatch)
					a_new;
				cmp = _parentID - patch._parentID;
				if (cmp == 0)
				{
					IQueue4 queue = _queue;
					if (queue == null)
					{
						queue = new NonblockingQueue();
						queue.Add(this);
					}
					queue.Add(patch);
					patch._queue = queue;
					patch._subsequent = _subsequent;
					patch._preceding = _preceding;
					patch.CalculateSize();
					return patch;
				}
			}
			return Add(a_new, cmp);
		}

		public override int Compare(Tree a_to)
		{
			IIndexable4 handler = _fieldTransaction.i_index._handler;
			return handler.CompareTo(handler.ComparableObject(Trans(), _value));
		}

		public virtual bool HasQueue()
		{
			return _queue != null;
		}

		public virtual IQueue4 DetachQueue()
		{
			IQueue4 queue = _queue;
			this._queue = null;
			return queue;
		}

		protected override Tree ShallowCloneInternal(Tree tree)
		{
			Db4objects.Db4o.Internal.IX.IxPatch patch = (Db4objects.Db4o.Internal.IX.IxPatch)
				base.ShallowCloneInternal(tree);
			patch._parentID = _parentID;
			patch._value = _value;
			patch._queue = _queue;
			return patch;
		}
	}
}
