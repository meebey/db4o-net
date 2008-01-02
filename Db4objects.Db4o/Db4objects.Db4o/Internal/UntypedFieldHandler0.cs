/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Mapping;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class UntypedFieldHandler0 : UntypedFieldHandler
	{
		public UntypedFieldHandler0(ObjectContainerBase container) : base(container)
		{
		}

		public override object Read(IReadContext context)
		{
			return context.ReadObject();
		}

		public override ObjectID ReadObjectID(IInternalReadContext context)
		{
			int id = context.ReadInt();
			return id == 0 ? ObjectID.IS_NULL : new ObjectID(id);
		}

		public override void Defragment(IDefragmentContext context)
		{
			int sourceId = context.SourceBuffer().ReadInt();
			if (sourceId == 0)
			{
				context.TargetBuffer().WriteInt(0);
				return;
			}
			int targetId = 0;
			try
			{
				targetId = context.MappedID(sourceId);
			}
			catch (MappingNotFoundException)
			{
				targetId = CopyDependentSlot(context, sourceId);
			}
			context.TargetBuffer().WriteInt(targetId);
		}

		private int CopyDependentSlot(IDefragmentContext context, int sourceId)
		{
			try
			{
				BufferImpl sourceBuffer = context.SourceBufferById(sourceId);
				Slot targetPointerSlot = context.AllocateMappedTargetSlot(sourceId, Const4.POINTER_LENGTH
					);
				Slot targetPayloadSlot = context.AllocateTargetSlot(sourceBuffer.Length());
				BufferImpl pointerBuffer = new BufferImpl(Const4.POINTER_LENGTH);
				pointerBuffer.WriteInt(targetPayloadSlot.Address());
				pointerBuffer.WriteInt(targetPayloadSlot.Length());
				context.TargetWriteBytes(targetPointerSlot.Address(), pointerBuffer);
				DefragmentContextImpl payloadContext = new DefragmentContextImpl(sourceBuffer, (DefragmentContextImpl
					)context);
				int clazzId = payloadContext.CopyIDReturnOriginalID();
				ClassMetadata payloadClazz = payloadContext.ClassMetadataForId(clazzId);
				ITypeHandler4 payloadHandler = payloadContext.CorrectHandlerVersion(payloadClazz);
				payloadHandler.Defragment(payloadContext);
				payloadContext.WriteToTarget(targetPayloadSlot.Address());
				return targetPointerSlot.Address();
			}
			catch (IOException ioexc)
			{
				throw new Db4oIOException(ioexc);
			}
		}
	}
}
