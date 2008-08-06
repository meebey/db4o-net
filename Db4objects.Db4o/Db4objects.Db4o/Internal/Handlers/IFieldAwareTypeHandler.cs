/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public interface IFieldAwareTypeHandler : ITypeHandler4, IVersionedTypeHandler, IFirstClassHandler
		, IVirtualAttributeHandler
	{
		void AddFieldIndices(ObjectIdContextImpl context, Slot oldSlot);

		void CollectIDs(CollectIdContext context, string fieldName);

		void DeleteMembers(DeleteContextImpl deleteContext, bool isUpdate);

		void ReadVirtualAttributes(ObjectReferenceContext context);

		void ClassMetadata(Db4objects.Db4o.Internal.ClassMetadata classMetadata);

		bool SeekToField(ObjectHeaderContext context, FieldMetadata field);
	}
}
