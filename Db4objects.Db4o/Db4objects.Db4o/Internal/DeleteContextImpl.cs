/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class DeleteContextImpl : BufferContext, IDeleteContext
	{
		private readonly int _handlerVersion;

		public DeleteContextImpl(StatefulBuffer buffer, int handlerVersion) : base(buffer
			.GetTransaction(), buffer)
		{
			_handlerVersion = handlerVersion;
		}

		public virtual void CascadeDeleteDepth(int depth)
		{
			((StatefulBuffer)_buffer).SetCascadeDeletes(depth);
		}

		public virtual int CascadeDeleteDepth()
		{
			return ((StatefulBuffer)_buffer).CascadeDeletes();
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
	}
}
