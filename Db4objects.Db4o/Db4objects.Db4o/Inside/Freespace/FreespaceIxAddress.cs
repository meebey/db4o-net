namespace Db4objects.Db4o.Inside.Freespace
{
	internal class FreespaceIxAddress : Db4objects.Db4o.Inside.Freespace.FreespaceIx
	{
		internal FreespaceIxAddress(Db4objects.Db4o.YapFile file, Db4objects.Db4o.MetaIndex
			 metaIndex) : base(file, metaIndex)
		{
		}

		internal override void Add(int address, int length)
		{
			_index._handler.PrepareComparison(address);
			_indexTrans.Add(length, address);
		}

		internal override int Address()
		{
			return _visitor._value;
		}

		internal override int Length()
		{
			return _visitor._key;
		}

		internal override void Remove(int address, int length)
		{
			_index._handler.PrepareComparison(address);
			_indexTrans.Remove(length, address);
		}

		internal virtual int FreeSize()
		{
			Db4objects.Db4o.Foundation.MutableInt mint = new Db4objects.Db4o.Foundation.MutableInt
				();
			Db4objects.Db4o.Foundation.IIntObjectVisitor freespaceVisitor = new _AnonymousInnerClass36
				(this, mint);
			Traverse(new _AnonymousInnerClass41(this, freespaceVisitor));
			return mint.Value();
		}

		private sealed class _AnonymousInnerClass36 : Db4objects.Db4o.Foundation.IIntObjectVisitor
		{
			public _AnonymousInnerClass36(FreespaceIxAddress _enclosing, Db4objects.Db4o.Foundation.MutableInt
				 mint)
			{
				this._enclosing = _enclosing;
				this.mint = mint;
			}

			public void Visit(int anInt, object anObject)
			{
				mint.Add(anInt);
			}

			private readonly FreespaceIxAddress _enclosing;

			private readonly Db4objects.Db4o.Foundation.MutableInt mint;
		}

		private sealed class _AnonymousInnerClass41 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass41(FreespaceIxAddress _enclosing, Db4objects.Db4o.Foundation.IIntObjectVisitor
				 freespaceVisitor)
			{
				this._enclosing = _enclosing;
				this.freespaceVisitor = freespaceVisitor;
			}

			public void Visit(object obj)
			{
				((Db4objects.Db4o.Inside.IX.IxTree)obj).VisitAll(freespaceVisitor);
			}

			private readonly FreespaceIxAddress _enclosing;

			private readonly Db4objects.Db4o.Foundation.IIntObjectVisitor freespaceVisitor;
		}
	}
}
