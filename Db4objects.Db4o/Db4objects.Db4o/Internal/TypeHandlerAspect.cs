/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class TypeHandlerAspect : ClassAspect
	{
		public readonly ITypeHandler4 _typeHandler;

		public TypeHandlerAspect(ITypeHandler4 typeHandler)
		{
			_typeHandler = typeHandler;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj == null || obj.GetType() != GetType())
			{
				return false;
			}
			Db4objects.Db4o.Internal.TypeHandlerAspect other = (Db4objects.Db4o.Internal.TypeHandlerAspect
				)obj;
			return _typeHandler.Equals(other._typeHandler);
		}

		public override int GetHashCode()
		{
			return _typeHandler.GetHashCode();
		}

		public override string GetName()
		{
			return _typeHandler.GetType().FullName;
		}

		public override void CascadeActivation(Transaction trans, object obj, IActivationDepth
			 depth)
		{
			if (_typeHandler is IFirstClassHandler)
			{
				ActivationContext4 context = new ActivationContext4(trans, obj, depth);
				((IFirstClassHandler)_typeHandler).CascadeActivation(context);
			}
		}

		public override void CollectIDs(CollectIdContext context)
		{
			throw new NotImplementedException();
		}

		public override void DefragAspect(IDefragmentContext context)
		{
			context.SlotFormat().DoWithSlotIndirection(context, new _IClosure4_54(this, context
				));
		}

		private sealed class _IClosure4_54 : IClosure4
		{
			public _IClosure4_54(TypeHandlerAspect _enclosing, IDefragmentContext context)
			{
				this._enclosing = _enclosing;
				this.context = context;
			}

			public object Run()
			{
				this._enclosing._typeHandler.Defragment(context);
				return null;
			}

			private readonly TypeHandlerAspect _enclosing;

			private readonly IDefragmentContext context;
		}

		public override int LinkLength()
		{
			return Const4.IndirectionLength;
		}

		public override void Marshall(MarshallingContext context, object obj)
		{
			context.CreateIndirectionWithinSlot();
			_typeHandler.Write(context, obj);
		}

		public override Db4objects.Db4o.Internal.Marshall.AspectType AspectType()
		{
			return Db4objects.Db4o.Internal.Marshall.AspectType.Typehandler;
		}

		public override void Instantiate(UnmarshallingContext context)
		{
			if (!CheckEnabled(context))
			{
				return;
			}
			object oldObject = context.PersistentObject();
			context.SlotFormat().DoWithSlotIndirection(context, new _IClosure4_81(this, context
				, oldObject));
		}

		private sealed class _IClosure4_81 : IClosure4
		{
			public _IClosure4_81(TypeHandlerAspect _enclosing, UnmarshallingContext context, 
				object oldObject)
			{
				this._enclosing = _enclosing;
				this.context = context;
				this.oldObject = oldObject;
			}

			public object Run()
			{
				object readObject = this._enclosing._typeHandler.Read(context);
				if (readObject != null && oldObject != readObject)
				{
					context.PersistentObject(readObject);
				}
				return null;
			}

			private readonly TypeHandlerAspect _enclosing;

			private readonly UnmarshallingContext context;

			private readonly object oldObject;
		}

		public override void Delete(DeleteContextImpl context, bool isUpdate)
		{
			context.SlotFormat().DoWithSlotIndirection(context, new _IClosure4_93(this, context
				));
		}

		private sealed class _IClosure4_93 : IClosure4
		{
			public _IClosure4_93(TypeHandlerAspect _enclosing, DeleteContextImpl context)
			{
				this._enclosing = _enclosing;
				this.context = context;
			}

			public object Run()
			{
				this._enclosing._typeHandler.Delete(context);
				return null;
			}

			private readonly TypeHandlerAspect _enclosing;

			private readonly DeleteContextImpl context;
		}

		public override void Deactivate(Transaction trans, object obj, IActivationDepth depth
			)
		{
			CascadeActivation(trans, obj, depth);
		}

		public override bool CanBeDisabled()
		{
			return true;
		}
	}
}
