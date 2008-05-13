/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Diagnostic;
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

		private readonly int _handlerVersion;

		private readonly Config4Field _fieldConfig;

		public DeleteContextImpl(IReflectClass fieldClass, int handlerVersion, Config4Field
			 fieldConfig, StatefulBuffer buffer) : base(buffer.GetTransaction(), buffer)
		{
			_fieldClass = fieldClass;
			_handlerVersion = handlerVersion;
			_fieldConfig = fieldConfig;
		}

		public virtual void CascadeDeleteDepth(int depth)
		{
			((StatefulBuffer)Buffer()).SetCascadeDeletes(depth);
		}

		public virtual int CascadeDeleteDepth()
		{
			return ((StatefulBuffer)Buffer()).CascadeDeletes();
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
			return new Slot(Buffer().ReadInt(), Buffer().ReadInt());
		}

		public override int HandlerVersion()
		{
			return _handlerVersion;
		}

		public virtual void Delete(ITypeHandler4 handler)
		{
			ITypeHandler4 fieldHandler = CorrectHandlerVersion(handler);
			int preservedCascadeDepth = CascadeDeleteDepth();
			CascadeDeleteDepth(AdjustedDepth());
			if (SlotFormat.ForHandlerVersion(HandlerVersion()).HandleAsObject(fieldHandler))
			{
				DeleteObject();
			}
			else
			{
				fieldHandler.Delete(this);
			}
			CascadeDeleteDepth(preservedCascadeDepth);
		}

		public virtual void DeleteObject()
		{
			int id = Buffer().ReadInt();
			if (CascadeDelete())
			{
				Container().DeleteByID(Transaction(), id, CascadeDeleteDepth());
			}
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
