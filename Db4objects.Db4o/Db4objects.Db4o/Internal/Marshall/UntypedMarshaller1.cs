/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class UntypedMarshaller1 : UntypedMarshaller
	{
		public override bool UseNormalClassRead()
		{
			return false;
		}

		public override ITypeHandler4 ReadArrayHandler(Transaction trans, BufferImpl[] reader
			)
		{
			int payLoadOffSet = reader[0].ReadInt();
			if (payLoadOffSet == 0)
			{
				return null;
			}
			ITypeHandler4 ret = null;
			reader[0]._offset = payLoadOffSet;
			int yapClassID = reader[0].ReadInt();
			ClassMetadata yc = trans.Container().ClassMetadataForId(yapClassID);
			if (yc != null)
			{
				ret = yc.ReadArrayHandler(trans, _family, reader);
			}
			return ret;
		}

		public override void Defrag(IDefragmentContext context)
		{
			int payLoadOffSet = context.ReadInt();
			if (payLoadOffSet == 0)
			{
				return;
			}
			int linkOffSet = context.Offset();
			context.Seek(payLoadOffSet);
			int classMetadataID = context.CopyIDReturnOriginalID();
			ClassMetadata classMetadata = context.ClassMetadataForId(classMetadataID);
			if (classMetadata != null)
			{
				classMetadata.Defragment(context);
			}
			context.Seek(linkOffSet);
		}
	}
}
