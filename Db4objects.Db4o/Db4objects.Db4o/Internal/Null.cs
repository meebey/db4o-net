namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Null : Db4objects.Db4o.Internal.IX.IIndexable4
	{
		public static readonly Db4objects.Db4o.Internal.IX.IIndexable4 INSTANCE = new Db4objects.Db4o.Internal.Null
			();

		public virtual object ComparableObject(Db4objects.Db4o.Internal.Transaction trans
			, object indexEntry)
		{
			return null;
		}

		public virtual int CompareTo(object a_obj)
		{
			if (a_obj == null)
			{
				return 0;
			}
			return -1;
		}

		public virtual object Current()
		{
			return null;
		}

		public virtual bool IsEqual(object obj)
		{
			return obj == null;
		}

		public virtual bool IsGreater(object obj)
		{
			return false;
		}

		public virtual bool IsSmaller(object obj)
		{
			return false;
		}

		public virtual int LinkLength()
		{
			return 0;
		}

		public virtual Db4objects.Db4o.Internal.IComparable4 PrepareComparison(object obj
			)
		{
			return this;
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer a_reader)
		{
			return null;
		}

		public virtual void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer a_writer, object
			 a_object)
		{
		}

		public virtual void DefragIndexEntry(Db4objects.Db4o.Internal.ReaderPair readers)
		{
		}
	}
}
