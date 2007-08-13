/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.IX;
using Sharpen;

namespace Db4objects.Db4o.Internal.IX
{
	/// <exclude></exclude>
	public class Index4
	{
		public readonly IIndexable4 _handler;

		private static int _version;

		public readonly MetaIndex _metaIndex;

		private IndexTransaction _globalIndexTransaction;

		private Collection4 _indexTransactions;

		private IxFileRangeReader _fileRangeReader;

		internal readonly bool _nullHandling;

		public Index4(LocalTransaction systemTrans, IIndexable4 handler, MetaIndex metaIndex
			, bool nullHandling)
		{
			_metaIndex = metaIndex;
			_handler = handler;
			_globalIndexTransaction = new IndexTransaction(systemTrans, this);
			_nullHandling = nullHandling;
			CreateGlobalFileRange();
		}

		public virtual IndexTransaction DirtyIndexTransaction(LocalTransaction a_trans)
		{
			IndexTransaction ift = new IndexTransaction(a_trans, this);
			if (_indexTransactions == null)
			{
				_indexTransactions = new Collection4();
			}
			else
			{
				IndexTransaction iftExisting = (IndexTransaction)_indexTransactions.Get(ift);
				if (iftExisting != null)
				{
					return iftExisting;
				}
			}
			a_trans.AddDirtyFieldIndex(ift);
			ift.SetRoot(Tree.DeepClone(_globalIndexTransaction.GetRoot(), ift));
			ift.i_version = ++_version;
			_indexTransactions.Add(ift);
			return ift;
		}

		public virtual IndexTransaction GlobalIndexTransaction()
		{
			return _globalIndexTransaction;
		}

		public virtual IndexTransaction IndexTransactionFor(LocalTransaction a_trans)
		{
			if (_indexTransactions != null)
			{
				IndexTransaction ift = new IndexTransaction(a_trans, this);
				ift = (IndexTransaction)_indexTransactions.Get(ift);
				if (ift != null)
				{
					return ift;
				}
			}
			return _globalIndexTransaction;
		}

		private int[] FreeForMetaIndex()
		{
			return new int[] { _metaIndex.indexAddress, _metaIndex.indexLength };
		}

		private void DoFree(int[] addressLength)
		{
			LocalObjectContainer yf = File();
			for (int i = 0; i < addressLength.Length; i += 2)
			{
				yf.Free(addressLength[i], addressLength[i + 1]);
			}
		}

		/// <summary>
		/// solving a hen-egg problem: commit itself works with freespace
		/// so we have to do this all sequentially in the right way, working
		/// with with both indexes at the same time.
		/// </summary>
		/// <remarks>
		/// solving a hen-egg problem: commit itself works with freespace
		/// so we have to do this all sequentially in the right way, working
		/// with with both indexes at the same time.
		/// </remarks>
		public virtual void CommitFreeSpace(Db4objects.Db4o.Internal.IX.Index4 other)
		{
			int entries = CountEntries();
			int length = (entries + 4) * LengthPerEntry();
			int mySlot = GetSlot(length);
			int otherSlot = GetSlot(length);
			DoFree(FreeForMetaIndex());
			DoFree(other.FreeForMetaIndex());
			entries = WriteToNewSlot(mySlot);
			MetaIndexSetMembers(entries, length, mySlot);
			CreateGlobalFileRange();
			int otherEntries = other.WriteToNewSlot(otherSlot);
			other.MetaIndexSetMembers(entries, length, otherSlot);
			other.CreateGlobalFileRange();
		}

		private int LengthPerEntry()
		{
			return _handler.LinkLength() + Const4.INT_LENGTH;
		}

		private void MetaIndexStore(int entries, int length, int address)
		{
			Transaction transact = Trans();
			MetaIndexSetMembers(entries, length, address);
			transact.Container().SetInternal(transact, _metaIndex, 1, false);
		}

		private void MetaIndexSetMembers(int entries, int length, int address)
		{
			_metaIndex.indexEntries = entries;
			_metaIndex.indexLength = length;
			_metaIndex.indexAddress = address;
		}

		private int WriteToNewSlot(int slot)
		{
			Tree root = GetRoot();
			StatefulBuffer writer = new StatefulBuffer(Trans(), slot, LengthPerEntry());
			int[] entries = new int[] { 0 };
			if (root != null)
			{
				root.Traverse(new _IVisitor4_149(this, entries, writer));
			}
			return entries[0];
		}

		private sealed class _IVisitor4_149 : IVisitor4
		{
			public _IVisitor4_149(Index4 _enclosing, int[] entries, StatefulBuffer writer)
			{
				this._enclosing = _enclosing;
				this.entries = entries;
				this.writer = writer;
			}

			public void Visit(object a_object)
			{
				entries[0] += ((IxTree)a_object).Write(this._enclosing._handler, writer);
			}

			private readonly Index4 _enclosing;

			private readonly int[] entries;

			private readonly StatefulBuffer writer;
		}

		internal virtual void Commit(IndexTransaction ixTrans)
		{
			_indexTransactions.Remove(ixTrans);
			_globalIndexTransaction.Merge(ixTrans);
			bool createNewFileRange = true;
			if (createNewFileRange)
			{
				int entries = CountEntries();
				int length = CountEntries() * LengthPerEntry();
				int slot = GetSlot(length);
				int[] free = FreeForMetaIndex();
				MetaIndexStore(entries, length, slot);
				WriteToNewSlot(slot);
				IxFileRange newFileRange = CreateGlobalFileRange();
				if (_indexTransactions != null)
				{
					IEnumerator i = _indexTransactions.GetEnumerator();
					while (i.MoveNext())
					{
						IndexTransaction ft = (IndexTransaction)i.Current;
						Tree clonedTree = newFileRange;
						if (clonedTree != null)
						{
							clonedTree = (Tree)clonedTree.DeepClone(ft);
						}
						Tree.ByRef tree = new Tree.ByRef(clonedTree);
						ft.GetRoot().TraverseFromLeaves((new _IVisitor4_197(this, ft, tree)));
						ft.SetRoot(tree.value);
					}
				}
				DoFree(free);
			}
			else
			{
				IEnumerator i = _indexTransactions.GetEnumerator();
				while (i.MoveNext())
				{
					((IndexTransaction)i.Current).Merge(ixTrans);
				}
			}
		}

		private sealed class _IVisitor4_197 : IVisitor4
		{
			public _IVisitor4_197(Index4 _enclosing, IndexTransaction ft, Tree.ByRef tree)
			{
				this._enclosing = _enclosing;
				this.ft = ft;
				this.tree = tree;
			}

			public void Visit(object a_object)
			{
				IxTree ixTree = (IxTree)a_object;
				if (ixTree._version == ft.i_version)
				{
					if (!(ixTree is IxFileRange))
					{
						ixTree.BeginMerge();
						tree.value = Tree.Add(tree.value, ixTree);
					}
				}
			}

			private readonly Index4 _enclosing;

			private readonly IndexTransaction ft;

			private readonly Tree.ByRef tree;
		}

		private IxFileRange CreateGlobalFileRange()
		{
			IxFileRange fr = null;
			if (_metaIndex.indexEntries > 0)
			{
				fr = new IxFileRange(_globalIndexTransaction, _metaIndex.indexAddress, 0, _metaIndex
					.indexEntries);
			}
			_globalIndexTransaction.SetRoot(fr);
			return fr;
		}

		internal virtual void Rollback(IndexTransaction a_ft)
		{
			_indexTransactions.Remove(a_ft);
		}

		internal virtual IxFileRangeReader FileRangeReader()
		{
			if (_fileRangeReader == null)
			{
				_fileRangeReader = new IxFileRangeReader(_handler);
			}
			return _fileRangeReader;
		}

		public override string ToString()
		{
			return base.ToString();
			StringBuilder sb = new StringBuilder();
			sb.Append("IxField  " + Runtime.IdentityHashCode(this));
			if (_globalIndexTransaction != null)
			{
				sb.Append("\n  Global \n   ");
				sb.Append(_globalIndexTransaction.ToString());
			}
			else
			{
				sb.Append("\n  no global index \n   ");
			}
			if (_indexTransactions != null)
			{
				IEnumerator i = _indexTransactions.GetEnumerator();
				while (i.MoveNext())
				{
					sb.Append("\n");
					sb.Append(i.Current.ToString());
				}
			}
			return sb.ToString();
		}

		private LocalTransaction Trans()
		{
			return _globalIndexTransaction.i_trans;
		}

		private LocalObjectContainer File()
		{
			return Trans().File();
		}

		private int GetSlot(int length)
		{
			return File().GetSlot(length).Address();
		}

		private Tree GetRoot()
		{
			return _globalIndexTransaction.GetRoot();
		}

		private int CountEntries()
		{
			Tree root = GetRoot();
			return root == null ? 0 : root.Size();
		}
	}
}
