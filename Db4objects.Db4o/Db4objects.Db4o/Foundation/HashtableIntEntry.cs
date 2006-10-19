namespace Db4objects.Db4o.Foundation
{
	internal class HashtableIntEntry : Db4objects.Db4o.Foundation.IDeepClone
	{
		internal int i_key;

		internal object i_object;

		internal Db4objects.Db4o.Foundation.HashtableIntEntry i_next;

		internal HashtableIntEntry(int a_hash, object a_object)
		{
			i_key = a_hash;
			i_object = a_object;
		}

		protected HashtableIntEntry()
		{
		}

		public virtual void AcceptKeyVisitor(Db4objects.Db4o.Foundation.IVisitor4 visitor
			)
		{
			visitor.Visit(i_key);
		}

		public virtual object DeepClone(object obj)
		{
			return DeepCloneInternal(new Db4objects.Db4o.Foundation.HashtableIntEntry(), obj);
		}

		public virtual bool SameKeyAs(Db4objects.Db4o.Foundation.HashtableIntEntry other)
		{
			return i_key == other.i_key;
		}

		protected virtual Db4objects.Db4o.Foundation.HashtableIntEntry DeepCloneInternal(
			Db4objects.Db4o.Foundation.HashtableIntEntry entry, object obj)
		{
			entry.i_key = i_key;
			entry.i_next = i_next;
			if (i_object is Db4objects.Db4o.Foundation.IDeepClone)
			{
				entry.i_object = ((Db4objects.Db4o.Foundation.IDeepClone)i_object).DeepClone(obj);
			}
			else
			{
				entry.i_object = i_object;
			}
			if (i_next != null)
			{
				entry.i_next = (Db4objects.Db4o.Foundation.HashtableIntEntry)i_next.DeepClone(obj
					);
			}
			return entry;
		}
	}
}
