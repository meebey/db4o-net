using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	public class HashtableIntEntry : IEntry4, IDeepClone
	{
		internal int _key;

		internal object _object;

		internal Db4objects.Db4o.Foundation.HashtableIntEntry _next;

		internal HashtableIntEntry(int a_hash, object a_object)
		{
			_key = a_hash;
			_object = a_object;
		}

		protected HashtableIntEntry()
		{
		}

		public virtual object Key()
		{
			return _key;
		}

		public virtual object Value()
		{
			return _object;
		}

		public virtual object DeepClone(object obj)
		{
			return DeepCloneInternal(new Db4objects.Db4o.Foundation.HashtableIntEntry(), obj);
		}

		public virtual bool SameKeyAs(Db4objects.Db4o.Foundation.HashtableIntEntry other)
		{
			return _key == other._key;
		}

		protected virtual Db4objects.Db4o.Foundation.HashtableIntEntry DeepCloneInternal(
			Db4objects.Db4o.Foundation.HashtableIntEntry entry, object obj)
		{
			entry._key = _key;
			entry._next = _next;
			if (_object is IDeepClone)
			{
				entry._object = ((IDeepClone)_object).DeepClone(obj);
			}
			else
			{
				entry._object = _object;
			}
			if (_next != null)
			{
				entry._next = (Db4objects.Db4o.Foundation.HashtableIntEntry)_next.DeepClone(obj);
			}
			return entry;
		}
	}
}
