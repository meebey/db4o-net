/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MWriteUpdate : MsgObject, IServerSideMessage
	{
		public void ProcessAtServer()
		{
			int classMetadataID = _payLoad.ReadInt();
			int arrayTypeValue = _payLoad.ReadInt();
			ArrayType arrayType = ArrayType.ForValue(arrayTypeValue);
			LocalObjectContainer container = (LocalObjectContainer)Stream();
			Unmarshall(_payLoad._offset);
			lock (StreamLock())
			{
				ClassMetadata classMetadata = container.ClassMetadataForID(classMetadataID);
				int id = _payLoad.GetID();
				Transaction().WriteUpdateAdjustIndexes(id, classMetadata, arrayType, 0);
				Transaction().DontDelete(id);
				Slot oldSlot = ((LocalTransaction)Transaction()).GetCommittedSlotOfID(id);
				container.GetSlotForUpdate(_payLoad);
				classMetadata.AddFieldIndices(_payLoad, oldSlot);
				_payLoad.WriteEncrypt();
				DeactivateCacheFor(id);
			}
		}

		private void DeactivateCacheFor(int id)
		{
			Transaction().Deactivate(id, new FixedActivationDepth(1));
		}
	}
}
