/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	public interface IDefragmentContext : IContext, IReadBuffer
	{
		ITypeHandler4 TypeHandlerForId(int id);

		int CopyID();

		int CopyIDReturnOriginalID();

		int CopySlotlessID();

		int CopyUnindexedID();

		void IncrementOffset(int length);

		bool IsLegacyHandlerVersion();

		int MappedID(int origID);

		BufferImpl SourceBuffer();

		BufferImpl TargetBuffer();

		Slot AllocateTargetSlot(int length);

		Slot AllocateMappedTargetSlot(int sourceAddress, int length);

		/// <exception cref="IOException"></exception>
		int CopySlotToNewMapped(int sourceAddress, int length);

		/// <exception cref="IOException"></exception>
		BufferImpl SourceBufferByAddress(int sourceAddress, int length);

		/// <exception cref="IOException"></exception>
		BufferImpl SourceBufferById(int sourceId);

		void TargetWriteBytes(int address, BufferImpl buffer);
	}
}
