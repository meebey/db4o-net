/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class ClassAspect
	{
		protected int _handle;

		private int _disabledFromAspectCountVersion = AspectVersionContextImpl.AlwaysEnabled
			.AspectCount();

		// used for identification when sending in C/S mode 
		public abstract Db4objects.Db4o.Internal.Marshall.AspectType AspectType();

		public abstract string GetName();

		public abstract void CascadeActivation(Transaction trans, object obj, IActivationDepth
			 depth);

		public abstract int LinkLength();

		public void IncrementOffset(IReadBuffer buffer)
		{
			buffer.Seek(buffer.Offset() + LinkLength());
		}

		public abstract void DefragAspect(IDefragmentContext context);

		public abstract void Marshall(MarshallingContext context, object child);

		public abstract void CollectIDs(CollectIdContext context);

		public virtual void SetHandle(int handle)
		{
			_handle = handle;
		}

		public abstract void Instantiate(UnmarshallingContext context);

		public abstract void Delete(DeleteContextImpl context, bool isUpdate);

		public abstract bool CanBeDisabled();

		protected virtual bool CheckEnabled(IAspectVersionContext context)
		{
			if (!Enabled(context))
			{
				IncrementOffset((IReadBuffer)context);
				return false;
			}
			return true;
		}

		public virtual void DisableFromAspectCountVersion(int aspectCount)
		{
			if (!CanBeDisabled())
			{
				return;
			}
			if (aspectCount < _disabledFromAspectCountVersion)
			{
				_disabledFromAspectCountVersion = aspectCount;
			}
		}

		public virtual bool Enabled(IAspectVersionContext context)
		{
			return _disabledFromAspectCountVersion > context.AspectCount();
		}

		public abstract void Deactivate(Transaction trans, object obj, IActivationDepth depth
			);
	}
}
