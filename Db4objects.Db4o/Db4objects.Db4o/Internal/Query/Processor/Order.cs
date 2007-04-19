using Db4objects.Db4o.Internal.Query.Processor;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	internal class Order : IOrderable
	{
		private int i_major;

		private int i_minor;

		public virtual int CompareTo(object obj)
		{
			if (obj is Order)
			{
				Order other = (Order)obj;
				int res = i_major - other.i_major;
				if (res != 0)
				{
					return res;
				}
				return i_minor - other.i_minor;
			}
			return -1;
		}

		public virtual void HintOrder(int a_order, bool a_major)
		{
			if (a_major)
			{
				i_major = a_order;
			}
			else
			{
				i_minor = a_order;
			}
		}

		public virtual bool HasDuplicates()
		{
			return true;
		}

		public override string ToString()
		{
			return "Order " + i_major + " " + i_minor;
		}

		public virtual void SwapMajorToMinor()
		{
			i_minor = i_major;
			i_major = 0;
		}
	}
}
