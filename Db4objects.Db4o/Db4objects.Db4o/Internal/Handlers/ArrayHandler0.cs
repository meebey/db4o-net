/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class ArrayHandler0 : ArrayHandler2
	{
		protected override void WithContent(BufferContext context, IRunnable runnable)
		{
			int address = context.ReadInt();
			int length = context.ReadInt();
			if (address == 0)
			{
				return;
			}
			IReadBuffer temp = context.Buffer();
			ByteArrayBuffer indirectedBuffer = context.Container().BufferByAddress(address, length
				);
			context.Buffer(indirectedBuffer);
			runnable.Run();
			context.Buffer(temp);
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void Delete(IDeleteContext context)
		{
			context.ReadSlot();
			context.DefragmentRecommended();
		}

		/// <summary>TODO: Consider to remove, Parent should take care.</summary>
		/// <remarks>TODO: Consider to remove, Parent should take care.</remarks>
		/// <exception cref="Db4oIOException"></exception>
		public override void ReadCandidates(QueryingReadContext context)
		{
			Transaction transaction = context.Transaction();
			QCandidates candidates = context.Candidates();
			ByteArrayBuffer arrayBuffer = ((ByteArrayBuffer)context.Buffer()).ReadEmbeddedObject
				(transaction);
			ArrayInfo info = NewArrayInfo();
			ReadInfo(transaction, arrayBuffer, info);
			for (int i = 0; i < info.ElementCount(); i++)
			{
				candidates.AddByIdentity(new QCandidate(candidates, null, arrayBuffer.ReadInt(), 
					true));
			}
		}

		public override object Read(IReadContext readContext)
		{
			IInternalReadContext context = (IInternalReadContext)readContext;
			ByteArrayBuffer buffer = (ByteArrayBuffer)context.ReadIndirectedBuffer();
			if (buffer == null)
			{
				return null;
			}
			// With the following line we ask the context to work with 
			// a different buffer. Should this logic ever be needed by
			// a user handler, it should be implemented by using a Queue
			// in the UnmarshallingContext.
			// The buffer has to be set back from the outside!  See below
			IReadBuffer contextBuffer = context.Buffer(buffer);
			object array = base.Read(context);
			// The context buffer has to be set back.
			context.Buffer(contextBuffer);
			return array;
		}

		public static void Defragment(IDefragmentContext context, ArrayHandler handler)
		{
			int sourceAddress = context.SourceBuffer().ReadInt();
			int length = context.SourceBuffer().ReadInt();
			if (sourceAddress == 0 && length == 0)
			{
				context.TargetBuffer().WriteInt(0);
				context.TargetBuffer().WriteInt(0);
				return;
			}
			Slot slot = context.AllocateMappedTargetSlot(sourceAddress, length);
			ByteArrayBuffer sourceBuffer = null;
			try
			{
				sourceBuffer = context.SourceBufferByAddress(sourceAddress, length);
			}
			catch (IOException exc)
			{
				throw new Db4oIOException(exc);
			}
			DefragmentContextImpl payloadContext = new DefragmentContextImpl(sourceBuffer, (DefragmentContextImpl
				)context);
			handler.Defrag1(payloadContext);
			payloadContext.WriteToTarget(slot.Address());
			context.TargetBuffer().WriteInt(slot.Address());
			context.TargetBuffer().WriteInt(length);
		}

		public override void Defragment(IDefragmentContext context)
		{
			Defragment(context, this);
		}

		public override void Defrag2(IDefragmentContext context)
		{
			int elements = ReadElementCountDefrag(context);
			for (int i = 0; i < elements; i++)
			{
				DelegateTypeHandler().Defragment(context);
			}
		}
	}
}
