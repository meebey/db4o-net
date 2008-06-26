/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
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
	public class UntypedFieldHandler0 : UntypedFieldHandler2
	{
		public UntypedFieldHandler0(ObjectContainerBase container) : base(container)
		{
		}

		public override object Read(IReadContext context)
		{
			return context.ReadObject();
		}

		public override ITypeHandler4 ReadCandidateHandler(QueryingReadContext context)
		{
			int id = 0;
			int offset = context.Offset();
			try
			{
				id = context.ReadInt();
			}
			catch (Exception)
			{
			}
			context.Seek(offset);
			if (id != 0)
			{
				StatefulBuffer reader = context.Container().ReadWriterByID(context.Transaction(), 
					id);
				if (reader != null)
				{
					ObjectHeader oh = new ObjectHeader(reader);
					try
					{
						if (oh.ClassMetadata() != null)
						{
							context.Buffer(reader);
							return oh.ClassMetadata().SeekCandidateHandler(context);
						}
					}
					catch (Exception e)
					{
					}
				}
			}
			// TODO: Check Exception Types
			// Errors typically occur, if classes don't match
			return null;
		}

		public override ObjectID ReadObjectID(IInternalReadContext context)
		{
			int id = context.ReadInt();
			return id == 0 ? ObjectID.IsNull : new ObjectID(id);
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
				ByteArrayBuffer sourceBuffer = context.SourceBufferById(sourceId);
				Slot targetPointerSlot = context.AllocateMappedTargetSlot(sourceId, Const4.PointerLength
					);
				Slot targetPayloadSlot = context.AllocateTargetSlot(sourceBuffer.Length());
				ByteArrayBuffer pointerBuffer = new ByteArrayBuffer(Const4.PointerLength);
				pointerBuffer.WriteInt(targetPayloadSlot.Address());
				pointerBuffer.WriteInt(targetPayloadSlot.Length());
				context.TargetWriteBytes(targetPointerSlot.Address(), pointerBuffer);
				DefragmentContextImpl payloadContext = new DefragmentContextImpl(sourceBuffer, (DefragmentContextImpl
					)context);
				int clazzId = payloadContext.CopyIDReturnOriginalID();
				ITypeHandler4 payloadHandler = payloadContext.TypeHandlerForId(clazzId);
				ITypeHandler4 versionedPayloadHandler = payloadContext.CorrectHandlerVersion(payloadHandler
					);
				versionedPayloadHandler.Defragment(payloadContext);
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
