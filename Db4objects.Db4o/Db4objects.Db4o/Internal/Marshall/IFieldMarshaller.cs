/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public interface IFieldMarshaller
	{
		void Write(Transaction trans, ClassMetadata clazz, FieldMetadata field, BufferImpl
			 writer);

		RawFieldSpec ReadSpec(ObjectContainerBase stream, BufferImpl reader);

		FieldMetadata Read(ObjectContainerBase stream, FieldMetadata field, BufferImpl reader
			);

		int MarshalledLength(ObjectContainerBase stream, FieldMetadata field);

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		void Defrag(ClassMetadata yapClass, FieldMetadata yapField, LatinStringIO sio, DefragmentContextImpl
			 context);
	}
}
