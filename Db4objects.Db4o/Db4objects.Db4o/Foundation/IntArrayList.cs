namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class IntArrayList : System.Collections.IEnumerable
	{
		internal const int INC = 20;

		protected int[] i_content;

		private int i_count;

		public IntArrayList() : this(INC)
		{
		}

		public IntArrayList(int initialSize)
		{
			i_content = new int[initialSize];
		}

		public virtual void Add(int a_value)
		{
			if (i_count >= i_content.Length)
			{
				int[] temp = new int[i_content.Length + INC];
				System.Array.Copy(i_content, 0, temp, 0, i_content.Length);
				i_content = temp;
			}
			i_content[i_count++] = a_value;
		}

		public virtual int IndexOf(int a_value)
		{
			for (int i = 0; i < i_count; i++)
			{
				if (i_content[i] == a_value)
				{
					return i;
				}
			}
			return -1;
		}

		public virtual int Size()
		{
			return i_count;
		}

		public virtual long[] AsLong()
		{
			long[] longs = new long[i_count];
			for (int i = 0; i < i_count; i++)
			{
				longs[i] = i_content[i];
			}
			return longs;
		}

		public virtual Db4objects.Db4o.Foundation.IIntIterator4 IntIterator()
		{
			return new Db4objects.Db4o.Foundation.ReverseIntIterator4Impl(i_content, i_count);
		}

		public virtual System.Collections.IEnumerator GetEnumerator()
		{
			return IntIterator();
		}

		public virtual int Get(int index)
		{
			return i_content[index];
		}

		public virtual void Swap(int left, int right)
		{
			if (left != right)
			{
				int swap = i_content[left];
				i_content[left] = i_content[right];
				i_content[right] = swap;
			}
		}
	}
}
