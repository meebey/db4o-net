/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public class ArrayHandler0 : ArrayHandler
	{
		public ArrayHandler0(ITypeHandler4 template) : base(template)
		{
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void Delete(IDeleteContext context)
		{
			context.ReadSlot();
			context.DefragmentRecommended();
		}

		/// <exception cref="Db4oIOException"></exception>
		public override void ReadCandidates(int handlerVersion, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates)
		{
			Transaction transaction = candidates.Transaction();
			Db4objects.Db4o.Internal.Buffer arrayBuffer = reader.ReadEmbeddedObject(transaction
				);
			int count = ElementCount(transaction, arrayBuffer);
			for (int i = 0; i < count; i++)
			{
				candidates.AddByIdentity(new QCandidate(candidates, null, arrayBuffer.ReadInt(), 
					true));
			}
		}

		public override object Read(IReadContext readContext)
		{
			IInternalReadContext context = (IInternalReadContext)readContext;
			Db4objects.Db4o.Internal.Buffer buffer = ReadIndirectedBuffer(context);
			if (buffer == null)
			{
				return null;
			}
			Db4objects.Db4o.Internal.Buffer contextBuffer = context.Buffer(buffer);
			object array = base.Read(context);
			context.Buffer(contextBuffer);
			return array;
		}
	}
}
