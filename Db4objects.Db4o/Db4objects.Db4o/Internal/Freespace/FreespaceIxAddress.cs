using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.IX;

namespace Db4objects.Db4o.Internal.Freespace
{
	internal class FreespaceIxAddress : FreespaceIx
	{
		internal FreespaceIxAddress(LocalObjectContainer file, MetaIndex metaIndex) : base
			(file, metaIndex)
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
			MutableInt mint = new MutableInt();
			IIntObjectVisitor freespaceVisitor = new _AnonymousInnerClass37(this, mint);
			Traverse(new _AnonymousInnerClass42(this, freespaceVisitor));
			return mint.Value();
		}

		private sealed class _AnonymousInnerClass37 : IIntObjectVisitor
		{
			public _AnonymousInnerClass37(FreespaceIxAddress _enclosing, MutableInt mint)
			{
				this._enclosing = _enclosing;
				this.mint = mint;
			}

			public void Visit(int anInt, object anObject)
			{
				mint.Add(anInt);
			}

			private readonly FreespaceIxAddress _enclosing;

			private readonly MutableInt mint;
		}

		private sealed class _AnonymousInnerClass42 : IVisitor4
		{
			public _AnonymousInnerClass42(FreespaceIxAddress _enclosing, IIntObjectVisitor freespaceVisitor
				)
			{
				this._enclosing = _enclosing;
				this.freespaceVisitor = freespaceVisitor;
			}

			public void Visit(object obj)
			{
				((IxTree)obj).VisitAll(freespaceVisitor);
			}

			private readonly FreespaceIxAddress _enclosing;

			private readonly IIntObjectVisitor freespaceVisitor;
		}
	}
}
