/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Internal.Activation
{
	/// <exclude></exclude>
	public class ActivationContext4
	{
		private readonly Transaction _transaction;

		private readonly object _targetObject;

		private readonly IActivationDepth _depth;

		public ActivationContext4(Transaction transaction, object obj, IActivationDepth depth
			)
		{
			_transaction = transaction;
			_targetObject = obj;
			_depth = depth;
		}

		public virtual void CascadeActivationToTarget(ClassMetadata classMetadata, bool doDescend
			)
		{
			IActivationDepth depth = doDescend ? _depth.Descend(classMetadata) : _depth;
			CascadeActivation(classMetadata, TargetObject(), depth);
		}

		public virtual void CascadeActivationToChild(object obj)
		{
			if (obj == null)
			{
				return;
			}
			ClassMetadata classMetadata = Container().ClassMetadataForObject(obj);
			if (classMetadata == null || classMetadata.IsPrimitive())
			{
				return;
			}
			IActivationDepth depth = _depth.Descend(classMetadata);
			CascadeActivation(classMetadata, obj, depth);
		}

		private void CascadeActivation(ClassMetadata classMetadata, object obj, IActivationDepth
			 depth)
		{
			if (!depth.RequiresActivation())
			{
				return;
			}
			if (depth.Mode().IsDeactivate())
			{
				Container().StillToDeactivate(_transaction, obj, depth, false);
			}
			else
			{
				// FIXME: [TA] do we need to check for isValueType here?
				if (classMetadata.IsValueType())
				{
					classMetadata.ActivateFields(_transaction, obj, depth);
				}
				else
				{
					Container().StillToActivate(_transaction, obj, depth);
				}
			}
		}

		public virtual ObjectContainerBase Container()
		{
			return _transaction.Container();
		}

		public virtual object TargetObject()
		{
			return _targetObject;
		}
	}
}
