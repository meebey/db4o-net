using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.IX;

namespace Db4objects.Db4o.Internal.Freespace
{
	internal abstract class FreespaceIx
	{
		internal Index4 _index;

		internal IndexTransaction _indexTrans;

		internal IxTraverser _traverser;

		internal FreespaceVisitor _visitor;

		internal FreespaceIx(LocalObjectContainer file, MetaIndex metaIndex)
		{
			_index = new Index4(file.GetLocalSystemTransaction(), new IntHandler(file), metaIndex
				, false);
			_indexTrans = _index.GlobalIndexTransaction();
		}

		internal abstract void Add(int address, int length);

		internal abstract int Address();

		public virtual void Debug()
		{
		}

		public virtual int EntryCount()
		{
			return Tree.Size(_indexTrans.GetRoot());
		}

		internal virtual void Find(int val)
		{
			_traverser = new IxTraverser();
			_traverser.FindBoundsExactMatch(val, (IxTree)_indexTrans.GetRoot());
		}

		internal abstract int Length();

		internal virtual bool Match()
		{
			_visitor = new FreespaceVisitor();
			_traverser.VisitMatch(_visitor);
			return _visitor.Visited();
		}

		internal virtual bool Preceding()
		{
			_visitor = new FreespaceVisitor();
			_traverser.VisitPreceding(_visitor);
			return _visitor.Visited();
		}

		internal abstract void Remove(int address, int length);

		internal virtual bool Subsequent()
		{
			_visitor = new FreespaceVisitor();
			_traverser.VisitSubsequent(_visitor);
			return _visitor.Visited();
		}

		public virtual void Traverse(IVisitor4 visitor)
		{
			Tree.Traverse(_indexTrans.GetRoot(), visitor);
		}
	}
}
