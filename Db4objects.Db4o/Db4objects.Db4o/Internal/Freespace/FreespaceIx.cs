namespace Db4objects.Db4o.Internal.Freespace
{
	internal abstract class FreespaceIx
	{
		internal Db4objects.Db4o.Internal.IX.Index4 _index;

		internal Db4objects.Db4o.Internal.IX.IndexTransaction _indexTrans;

		internal Db4objects.Db4o.Internal.IX.IxTraverser _traverser;

		internal Db4objects.Db4o.Internal.Freespace.FreespaceVisitor _visitor;

		internal FreespaceIx(Db4objects.Db4o.Internal.LocalObjectContainer file, Db4objects.Db4o.MetaIndex
			 metaIndex)
		{
			_index = new Db4objects.Db4o.Internal.IX.Index4(file.GetSystemTransaction(), new 
				Db4objects.Db4o.Internal.Handlers.IntHandler(file), metaIndex, false);
			_indexTrans = _index.GlobalIndexTransaction();
		}

		internal abstract void Add(int address, int length);

		internal abstract int Address();

		public virtual void Debug()
		{
		}

		public virtual int EntryCount()
		{
			return Db4objects.Db4o.Foundation.Tree.Size(_indexTrans.GetRoot());
		}

		internal virtual void Find(int val)
		{
			_traverser = new Db4objects.Db4o.Internal.IX.IxTraverser();
			_traverser.FindBoundsExactMatch(val, (Db4objects.Db4o.Internal.IX.IxTree)_indexTrans
				.GetRoot());
		}

		internal abstract int Length();

		internal virtual bool Match()
		{
			_visitor = new Db4objects.Db4o.Internal.Freespace.FreespaceVisitor();
			_traverser.VisitMatch(_visitor);
			return _visitor.Visited();
		}

		internal virtual bool Preceding()
		{
			_visitor = new Db4objects.Db4o.Internal.Freespace.FreespaceVisitor();
			_traverser.VisitPreceding(_visitor);
			return _visitor.Visited();
		}

		internal abstract void Remove(int address, int length);

		internal virtual bool Subsequent()
		{
			_visitor = new Db4objects.Db4o.Internal.Freespace.FreespaceVisitor();
			_traverser.VisitSubsequent(_visitor);
			return _visitor.Visited();
		}

		public virtual void Traverse(Db4objects.Db4o.Foundation.IVisitor4 visitor)
		{
			Db4objects.Db4o.Foundation.Tree.Traverse(_indexTrans.GetRoot(), visitor);
		}
	}
}
