/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	public interface IDefragmentContext : IBufferContext
	{
		ITypeHandler4 TypeHandlerForId(int id);

		int CopyID();

		int CopyIDReturnOriginalID();

		int CopySlotlessID();

		int CopyUnindexedID();

		ITypeHandler4 CorrectHandlerVersion(ITypeHandler4 handler);

		int HandlerVersion();

		void IncrementOffset(int length);

		bool IsLegacyHandlerVersion();

		int MappedID(int origID);

		ByteArrayBuffer SourceBuffer();

		ByteArrayBuffer TargetBuffer();

		Slot AllocateTargetSlot(int length);

		Slot AllocateMappedTargetSlot(int sourceAddress, int length);

		/// <exception cref="IOException"></exception>
		int CopySlotToNewMapped(int sourceAddress, int length);

		/// <exception cref="IOException"></exception>
		ByteArrayBuffer SourceBufferByAddress(int sourceAddress, int length);

		/// <exception cref="IOException"></exception>
		ByteArrayBuffer SourceBufferById(int sourceId);

		void TargetWriteBytes(int address, ByteArrayBuffer buffer);
	}
}
