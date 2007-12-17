/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	public interface IDefragmentContext : IContext, IReadBuffer
	{
		ClassMetadata ClassMetadataForId(int id);

		int CopyID();

		int CopyIDReturnOriginalID();

		int CopyUnindexedID();

		void IncrementOffset(int length);

		bool IsLegacyHandlerVersion();

		int MappedID(int origID);

		BufferImpl SourceBuffer();

		BufferImpl TargetBuffer();
	}
}
