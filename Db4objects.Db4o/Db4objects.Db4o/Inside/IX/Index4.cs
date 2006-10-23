namespace Db4objects.Db4o.Inside.IX
{
	/// <exclude></exclude>
	public class Index4
	{
		public readonly Db4objects.Db4o.Inside.IX.IIndexable4 _handler;

		private static int _version;

		public readonly Db4objects.Db4o.MetaIndex _metaIndex;

		private Db4objects.Db4o.Inside.IX.IndexTransaction _globalIndexTransaction;

		private Db4objects.Db4o.Foundation.Collection4 _indexTransactions;

		private Db4objects.Db4o.Inside.IX.IxFileRangeReader _fileRangeReader;

		internal readonly bool _nullHandling;

		public Index4(Db4objects.Db4o.Transaction systemTrans, Db4objects.Db4o.Inside.IX.IIndexable4
			 handler, Db4objects.Db4o.MetaIndex metaIndex, bool nullHandling)
		{
			_metaIndex = metaIndex;
			_handler = handler;
			_globalIndexTransaction = new Db4objects.Db4o.Inside.IX.IndexTransaction(systemTrans
				, this);
			_nullHandling = nullHandling;
			CreateGlobalFileRange();
		}

		public virtual Db4objects.Db4o.Inside.IX.IndexTransaction DirtyIndexTransaction(Db4objects.Db4o.Transaction
			 a_trans)
		{
			Db4objects.Db4o.Inside.IX.IndexTransaction ift = new Db4objects.Db4o.Inside.IX.IndexTransaction
				(a_trans, this);
			if (_indexTransactions == null)
			{
				_indexTransactions = new Db4objects.Db4o.Foundation.Collection4();
			}
			else
			{
				Db4objects.Db4o.Inside.IX.IndexTransaction iftExisting = (Db4objects.Db4o.Inside.IX.IndexTransaction
					)_indexTransactions.Get(ift);
				if (iftExisting != null)
				{
					return iftExisting;
				}
			}
			a_trans.AddDirtyFieldIndex(ift);
			ift.SetRoot(Db4objects.Db4o.Foundation.Tree.DeepClone(_globalIndexTransaction.GetRoot
				(), ift));
			ift.i_version = ++_version;
			_indexTransactions.Add(ift);
			return ift;
		}

		public virtual Db4objects.Db4o.Inside.IX.IndexTransaction GlobalIndexTransaction(
			)
		{
			return _globalIndexTransaction;
		}

		public virtual Db4objects.Db4o.Inside.IX.IndexTransaction IndexTransactionFor(Db4objects.Db4o.Transaction
			 a_trans)
		{
			if (_indexTransactions != null)
			{
				Db4objects.Db4o.Inside.IX.IndexTransaction ift = new Db4objects.Db4o.Inside.IX.IndexTransaction
					(a_trans, this);
				ift = (Db4objects.Db4o.Inside.IX.IndexTransaction)_indexTransactions.Get(ift);
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
			Db4objects.Db4o.YapFile yf = File();
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
		public virtual void CommitFreeSpace(Db4objects.Db4o.Inside.IX.Index4 other)
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
			return _handler.LinkLength() + Db4objects.Db4o.YapConst.INT_LENGTH;
		}

		private void MetaIndexStore(int entries, int length, int address)
		{
			Db4objects.Db4o.Transaction transact = Trans();
			MetaIndexSetMembers(entries, length, address);
			transact.Stream().SetInternal(transact, _metaIndex, 1, false);
		}

		private void MetaIndexSetMembers(int entries, int length, int address)
		{
			_metaIndex.indexEntries = entries;
			_metaIndex.indexLength = length;
			_metaIndex.indexAddress = address;
		}

		private int WriteToNewSlot(int slot)
		{
			Db4objects.Db4o.Foundation.Tree root = GetRoot();
			Db4objects.Db4o.YapWriter writer = new Db4objects.Db4o.YapWriter(Trans(), slot, LengthPerEntry
				());
			int[] entries = new int[] { 0 };
			if (root != null)
			{
				root.Traverse(new _AnonymousInnerClass148(this, entries, writer));
			}
			return entries[0];
		}

		private sealed class _AnonymousInnerClass148 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass148(Index4 _enclosing, int[] entries, Db4objects.Db4o.YapWriter
				 writer)
			{
				this._enclosing = _enclosing;
				this.entries = entries;
				this.writer = writer;
			}

			public void Visit(object a_object)
			{
				entries[0] += ((Db4objects.Db4o.Inside.IX.IxTree)a_object).Write(this._enclosing.
					_handler, writer);
			}

			private readonly Index4 _enclosing;

			private readonly int[] entries;

			private readonly Db4objects.Db4o.YapWriter writer;
		}

		internal virtual void Commit(Db4objects.Db4o.Inside.IX.IndexTransaction ixTrans)
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
				Db4objects.Db4o.Inside.IX.IxFileRange newFileRange = CreateGlobalFileRange();
				if (_indexTransactions != null)
				{
					System.Collections.IEnumerator i = _indexTransactions.GetEnumerator();
					while (i.MoveNext())
					{
						Db4objects.Db4o.Inside.IX.IndexTransaction ft = (Db4objects.Db4o.Inside.IX.IndexTransaction
							)i.Current;
						Db4objects.Db4o.Foundation.Tree clonedTree = newFileRange;
						if (clonedTree != null)
						{
							clonedTree = clonedTree.DeepClone(ft);
						}
						Db4objects.Db4o.Foundation.Tree[] tree = { clonedTree };
						ft.GetRoot().TraverseFromLeaves((new _AnonymousInnerClass196(this, ft, tree)));
						ft.SetRoot(tree[0]);
					}
				}
				DoFree(free);
			}
			else
			{
				System.Collections.IEnumerator i = _indexTransactions.GetEnumerator();
				while (i.MoveNext())
				{
					((Db4objects.Db4o.Inside.IX.IndexTransaction)i.Current).Merge(ixTrans);
				}
			}
		}

		private sealed class _AnonymousInnerClass196 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass196(Index4 _enclosing, Db4objects.Db4o.Inside.IX.IndexTransaction
				 ft, Db4objects.Db4o.Foundation.Tree[] tree)
			{
				this._enclosing = _enclosing;
				this.ft = ft;
				this.tree = tree;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.Inside.IX.IxTree ixTree = (Db4objects.Db4o.Inside.IX.IxTree)a_object;
				if (ixTree._version == ft.i_version)
				{
					if (!(ixTree is Db4objects.Db4o.Inside.IX.IxFileRange))
					{
						ixTree.BeginMerge();
						tree[0] = Db4objects.Db4o.Foundation.Tree.Add(tree[0], ixTree);
					}
				}
			}

			private readonly Index4 _enclosing;

			private readonly Db4objects.Db4o.Inside.IX.IndexTransaction ft;

			private readonly Db4objects.Db4o.Foundation.Tree[] tree;
		}

		private Db4objects.Db4o.Inside.IX.IxFileRange CreateGlobalFileRange()
		{
			Db4objects.Db4o.Inside.IX.IxFileRange fr = null;
			if (_metaIndex.indexEntries > 0)
			{
				fr = new Db4objects.Db4o.Inside.IX.IxFileRange(_globalIndexTransaction, _metaIndex
					.indexAddress, 0, _metaIndex.indexEntries);
			}
			_globalIndexTransaction.SetRoot(fr);
			return fr;
		}

		internal virtual void Rollback(Db4objects.Db4o.Inside.IX.IndexTransaction a_ft)
		{
			_indexTransactions.Remove(a_ft);
		}

		internal virtual Db4objects.Db4o.Inside.IX.IxFileRangeReader FileRangeReader()
		{
			if (_fileRangeReader == null)
			{
				_fileRangeReader = new Db4objects.Db4o.Inside.IX.IxFileRangeReader(_handler);
			}
			return _fileRangeReader;
		}

		public override string ToString()
		{
			return base.ToString();
			Sharpen.Lang.StringBuffer sb = new Sharpen.Lang.StringBuffer();
			sb.Append("IxField  " + Sharpen.Runtime.IdentityHashCode(this));
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
				System.Collections.IEnumerator i = _indexTransactions.GetEnumerator();
				while (i.MoveNext())
				{
					sb.Append("\n");
					sb.Append(i.Current.ToString());
				}
			}
			return sb.ToString();
		}

		private Db4objects.Db4o.Transaction Trans()
		{
			return _globalIndexTransaction.i_trans;
		}

		private Db4objects.Db4o.YapFile File()
		{
			return Trans().i_file;
		}

		private int GetSlot(int length)
		{
			return File().GetSlot(length);
		}

		private Db4objects.Db4o.Foundation.Tree GetRoot()
		{
			return _globalIndexTransaction.GetRoot();
		}

		private int CountEntries()
		{
			Db4objects.Db4o.Foundation.Tree root = GetRoot();
			return root == null ? 0 : root.Size();
		}
	}
}
