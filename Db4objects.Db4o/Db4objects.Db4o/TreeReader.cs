namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public sealed class TreeReader
	{
		private readonly Db4objects.Db4o.IReadable i_template;

		private readonly Db4objects.Db4o.YapReader i_bytes;

		private int i_current = 0;

		private int i_levels = 0;

		private int i_size;

		private bool i_orderOnRead;

		public TreeReader(Db4objects.Db4o.YapReader a_bytes, Db4objects.Db4o.IReadable a_template
			) : this(a_bytes, a_template, false)
		{
		}

		public TreeReader(Db4objects.Db4o.YapReader a_bytes, Db4objects.Db4o.IReadable a_template
			, bool a_orderOnRead)
		{
			i_template = a_template;
			i_bytes = a_bytes;
			i_orderOnRead = a_orderOnRead;
		}

		public Db4objects.Db4o.Foundation.Tree Read()
		{
			return Read(i_bytes.ReadInt());
		}

		public Db4objects.Db4o.Foundation.Tree Read(int a_size)
		{
			i_size = a_size;
			if (i_size > 0)
			{
				if (i_orderOnRead)
				{
					Db4objects.Db4o.Foundation.Tree tree = null;
					for (int i = 0; i < i_size; i++)
					{
						tree = Db4objects.Db4o.Foundation.Tree.Add(tree, (Db4objects.Db4o.Foundation.Tree
							)i_template.Read(i_bytes));
					}
					return tree;
				}
				while ((1 << i_levels) < (i_size + 1))
				{
					i_levels++;
				}
				return LinkUp(null, i_levels);
			}
			return null;
		}

		private Db4objects.Db4o.Foundation.Tree LinkUp(Db4objects.Db4o.Foundation.Tree a_preceding
			, int a_level)
		{
			Db4objects.Db4o.Foundation.Tree node = (Db4objects.Db4o.Foundation.Tree)i_template
				.Read(i_bytes);
			i_current++;
			node._preceding = a_preceding;
			node._subsequent = LinkDown(a_level + 1);
			node.CalculateSize();
			if (i_current < i_size)
			{
				return LinkUp(node, a_level - 1);
			}
			return node;
		}

		private Db4objects.Db4o.Foundation.Tree LinkDown(int a_level)
		{
			if (i_current < i_size)
			{
				i_current++;
				if (a_level < i_levels)
				{
					Db4objects.Db4o.Foundation.Tree preceding = LinkDown(a_level + 1);
					Db4objects.Db4o.Foundation.Tree node = (Db4objects.Db4o.Foundation.Tree)i_template
						.Read(i_bytes);
					node._preceding = preceding;
					node._subsequent = LinkDown(a_level + 1);
					node.CalculateSize();
					return node;
				}
				return (Db4objects.Db4o.Foundation.Tree)i_template.Read(i_bytes);
			}
			return null;
		}
	}
}
