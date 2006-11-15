namespace Db4objects.Db4o.Inside.Callbacks
{
	public class NullCallbacks : Db4objects.Db4o.Inside.Callbacks.ICallbacks
	{
		public virtual void OnQueryFinished(Db4objects.Db4o.Query.IQuery query)
		{
		}

		public virtual void OnQueryStarted(Db4objects.Db4o.Query.IQuery query)
		{
		}

		public virtual bool ObjectCanNew(object obj)
		{
			return true;
		}

		public virtual bool ObjectCanActivate(object obj)
		{
			return true;
		}

		public virtual bool ObjectCanUpdate(object obj)
		{
			return true;
		}

		public virtual bool ObjectCanDelete(object obj)
		{
			return true;
		}

		public virtual bool ObjectCanDeactivate(object obj)
		{
			return true;
		}

		public virtual void ObjectOnNew(object obj)
		{
		}

		public virtual void ObjectOnActivate(object obj)
		{
		}

		public virtual void ObjectOnUpdate(object obj)
		{
		}

		public virtual void ObjectOnDelete(object obj)
		{
		}

		public virtual void ObjectOnDeactivate(object obj)
		{
		}
	}
}
