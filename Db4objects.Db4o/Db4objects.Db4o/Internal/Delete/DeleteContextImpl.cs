/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Delete
{
	/// <exclude></exclude>
	public class DeleteContextImpl : BufferContext, IDeleteContext
	{
		private readonly IReflectClass _fieldClass;

		private readonly ITypeHandler4 _fieldHandler;

		private readonly int _handlerVersion;

		private readonly Config4Field _fieldConfig;

		private int _deleteDepth;

		public DeleteContextImpl(IReflectClass fieldClass, ITypeHandler4 fieldHandler, int
			 handlerVersion, Config4Field fieldConfig, StatefulBuffer buffer) : base(buffer.
			GetTransaction(), buffer)
		{
			_fieldHandler = fieldHandler;
			_fieldClass = fieldClass;
			_handlerVersion = handlerVersion;
			_fieldConfig = fieldConfig;
			_deleteDepth = ((StatefulBuffer)_buffer).CascadeDeletes();
		}

		public virtual void CascadeDeleteDepth(int depth)
		{
			_deleteDepth = depth;
		}

		public virtual int CascadeDeleteDepth()
		{
			return _deleteDepth;
		}

		public virtual bool CascadeDelete()
		{
			return CascadeDeleteDepth() > 0;
		}

		public virtual void DefragmentRecommended()
		{
			DiagnosticProcessor dp = Container()._handlers._diagnosticProcessor;
			if (dp.Enabled())
			{
				dp.DefragmentRecommended(DefragmentRecommendation.DefragmentRecommendationReason.
					DeleteEmbeded);
			}
		}

		public virtual Slot ReadSlot()
		{
			return new Slot(_buffer.ReadInt(), _buffer.ReadInt());
		}

		public override int HandlerVersion()
		{
			return _handlerVersion;
		}

		public virtual void Delete()
		{
			int preservedCascadeDepth = CascadeDeleteDepth();
			CascadeDeleteDepth(AdjustedDepth());
			// correctHandlerVersion(_fieldHandler).delete(DeleteContextImpl.this);
			SlotFormat.ForHandlerVersion(HandlerVersion()).DoWithSlotIndirection(this, _fieldHandler
				, new _IClosure4_72(this));
			CascadeDeleteDepth(preservedCascadeDepth);
		}

		private sealed class _IClosure4_72 : IClosure4
		{
			public _IClosure4_72(DeleteContextImpl _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				this._enclosing.CorrectHandlerVersion(this._enclosing._fieldHandler).Delete(this.
					_enclosing);
				return null;
			}

			private readonly DeleteContextImpl _enclosing;
		}

		private int AdjustedDepth()
		{
			if (Platform4.IsValueType(_fieldClass))
			{
				return 1;
			}
			if (_fieldConfig == null)
			{
				return CascadeDeleteDepth();
			}
			if (_fieldConfig.CascadeOnDelete().DefiniteYes())
			{
				return 1;
			}
			if (_fieldConfig.CascadeOnDelete().DefiniteNo())
			{
				return 0;
			}
			return CascadeDeleteDepth();
		}
	}
}
