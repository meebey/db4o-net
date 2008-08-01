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
	public class DeleteContextImpl : ObjectHeaderContext, IDeleteContext, IObjectIdContext
	{
		private readonly IReflectClass _fieldClass;

		private readonly Config4Field _fieldConfig;

		public DeleteContextImpl(Db4objects.Db4o.Internal.StatefulBuffer buffer, ObjectHeader
			 objectHeader, IReflectClass fieldClass, Config4Field fieldConfig) : base(buffer
			.Transaction(), buffer, objectHeader)
		{
			_fieldClass = fieldClass;
			_fieldConfig = fieldConfig;
		}

		public DeleteContextImpl(Db4objects.Db4o.Internal.Delete.DeleteContextImpl parentContext
			, IReflectClass fieldClass, Config4Field fieldConfig) : this(parentContext.StatefulBuffer
			(), parentContext._objectHeader, fieldClass, fieldConfig)
		{
		}

		public virtual void CascadeDeleteDepth(int depth)
		{
			StatefulBuffer().SetCascadeDeletes(depth);
		}

		private Db4objects.Db4o.Internal.StatefulBuffer StatefulBuffer()
		{
			return ((Db4objects.Db4o.Internal.StatefulBuffer)Buffer());
		}

		public virtual int CascadeDeleteDepth()
		{
			return StatefulBuffer().CascadeDeletes();
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

		public virtual void Delete(ITypeHandler4 handler)
		{
			ITypeHandler4 fieldHandler = Handlers4.CorrectHandlerVersion(this, handler);
			int preservedCascadeDepth = CascadeDeleteDepth();
			CascadeDeleteDepth(AdjustedDepth());
			if (SlotFormat().HandleAsObject(fieldHandler))
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

		public virtual int Id()
		{
			return StatefulBuffer().GetID();
		}
	}
}
