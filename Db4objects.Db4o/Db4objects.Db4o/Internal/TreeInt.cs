/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal
{
	/// <summary>Base class for balanced trees.</summary>
	/// <remarks>Base class for balanced trees.</remarks>
	/// <exclude></exclude>
	public class TreeInt : Tree, IReadWriteable
	{
		public static Db4objects.Db4o.Internal.TreeInt Add(Db4objects.Db4o.Internal.TreeInt
			 tree, int value)
		{
			return (Db4objects.Db4o.Internal.TreeInt)Tree.Add(tree, new Db4objects.Db4o.Internal.TreeInt
				(value));
		}

		public static Db4objects.Db4o.Internal.TreeInt RemoveLike(Db4objects.Db4o.Internal.TreeInt
			 tree, int value)
		{
			return (Db4objects.Db4o.Internal.TreeInt)Tree.RemoveLike(tree, new Db4objects.Db4o.Internal.TreeInt
				(value));
		}

		public static Tree AddAll(Tree tree, IIntIterator4 iter)
		{
			if (!iter.MoveNext())
			{
				return tree;
			}
			Db4objects.Db4o.Internal.TreeInt firstAdded = new Db4objects.Db4o.Internal.TreeInt
				(iter.CurrentInt());
			tree = Tree.Add(tree, firstAdded);
			while (iter.MoveNext())
			{
				tree = tree.Add(new Db4objects.Db4o.Internal.TreeInt(iter.CurrentInt()));
			}
			return tree;
		}

		public int _key;

		public TreeInt(int a_key)
		{
			this._key = a_key;
		}

		public override int Compare(Tree a_to)
		{
			return _key - ((Db4objects.Db4o.Internal.TreeInt)a_to)._key;
		}

		internal virtual Tree DeepClone()
		{
			return new Db4objects.Db4o.Internal.TreeInt(_key);
		}

		public override bool Duplicates()
		{
			return false;
		}

		public static Db4objects.Db4o.Internal.TreeInt Find(Tree a_in, int a_key)
		{
			if (a_in == null)
			{
				return null;
			}
			return ((Db4objects.Db4o.Internal.TreeInt)a_in).Find(a_key);
		}

		public Db4objects.Db4o.Internal.TreeInt Find(int a_key)
		{
			int cmp = _key - a_key;
			if (cmp < 0)
			{
				if (_subsequent != null)
				{
					return ((Db4objects.Db4o.Internal.TreeInt)_subsequent).Find(a_key);
				}
			}
			else
			{
				if (cmp > 0)
				{
					if (_preceding != null)
					{
						return ((Db4objects.Db4o.Internal.TreeInt)_preceding).Find(a_key);
					}
				}
				else
				{
					return this;
				}
			}
			return null;
		}

		public virtual object Read(ByteArrayBuffer a_bytes)
		{
			return new Db4objects.Db4o.Internal.TreeInt(a_bytes.ReadInt());
		}

		public virtual void Write(ByteArrayBuffer a_writer)
		{
			a_writer.WriteInt(_key);
		}

		public static void Write(ByteArrayBuffer a_writer, Db4objects.Db4o.Internal.TreeInt
			 a_tree)
		{
			Write(a_writer, a_tree, a_tree == null ? 0 : a_tree.Size());
		}

		public static void Write(ByteArrayBuffer a_writer, Db4objects.Db4o.Internal.TreeInt
			 a_tree, int size)
		{
			if (a_tree == null)
			{
				a_writer.WriteInt(0);
				return;
			}
			a_writer.WriteInt(size);
			a_tree.Traverse(new _IVisitor4_97(a_writer));
		}

		private sealed class _IVisitor4_97 : IVisitor4
		{
			public _IVisitor4_97(ByteArrayBuffer a_writer)
			{
				this.a_writer = a_writer;
			}

			public void Visit(object a_object)
			{
				((Db4objects.Db4o.Internal.TreeInt)a_object).Write(a_writer);
			}

			private readonly ByteArrayBuffer a_writer;
		}

		public virtual int OwnLength()
		{
			return Const4.IntLength;
		}

		internal virtual bool VariableLength()
		{
			return false;
		}

		internal virtual QCandidate ToQCandidate(QCandidates candidates)
		{
			QCandidate qc = new QCandidate(candidates, null, _key, true);
			qc._preceding = ToQCandidate((Db4objects.Db4o.Internal.TreeInt)_preceding, candidates
				);
			qc._subsequent = ToQCandidate((Db4objects.Db4o.Internal.TreeInt)_subsequent, candidates
				);
			qc._size = _size;
			return qc;
		}

		public static QCandidate ToQCandidate(Db4objects.Db4o.Internal.TreeInt tree, QCandidates
			 candidates)
		{
			if (tree == null)
			{
				return null;
			}
			return tree.ToQCandidate(candidates);
		}

		public override string ToString()
		{
			return string.Empty + _key;
		}

		protected override Tree ShallowCloneInternal(Tree tree)
		{
			Db4objects.Db4o.Internal.TreeInt treeint = (Db4objects.Db4o.Internal.TreeInt)base
				.ShallowCloneInternal(tree);
			treeint._key = _key;
			return treeint;
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Internal.TreeInt treeint = new Db4objects.Db4o.Internal.TreeInt(_key
				);
			return ShallowCloneInternal(treeint);
		}

		public static int MarshalledLength(Db4objects.Db4o.Internal.TreeInt a_tree)
		{
			if (a_tree == null)
			{
				return Const4.IntLength;
			}
			return a_tree.MarshalledLength();
		}

		public int MarshalledLength()
		{
			if (VariableLength())
			{
				IntByRef mint = new IntByRef(Const4.IntLength);
				Traverse(new _IVisitor4_152(mint));
				return mint.value;
			}
			return Const4.IntLength + (Size() * OwnLength());
		}

		private sealed class _IVisitor4_152 : IVisitor4
		{
			public _IVisitor4_152(IntByRef mint)
			{
				this.mint = mint;
			}

			public void Visit(object obj)
			{
				mint.value += ((Db4objects.Db4o.Internal.TreeInt)obj).OwnLength();
			}

			private readonly IntByRef mint;
		}

		public override object Key()
		{
			return _key;
		}
	}
}
