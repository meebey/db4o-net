/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MWriteUpdate : MsgObject, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int yapClassId = _payLoad.ReadInt();
			int arrayTypeValue = _payLoad.ReadInt();
			ArrayType arrayType = ArrayType.ForValue(arrayTypeValue);
			LocalObjectContainer stream = (LocalObjectContainer)Stream();
			Unmarshall(_payLoad._offset);
			lock (StreamLock())
			{
				ClassMetadata classMetadata = stream.ClassMetadataForId(yapClassId);
				int id = _payLoad.GetID();
				Transaction().WriteUpdateAdjustIndexes(id, classMetadata, arrayType, 0);
				Transaction().DontDelete(id);
				Slot oldSlot = ((LocalTransaction)Transaction()).GetCommittedSlotOfID(id);
				stream.GetSlotForUpdate(_payLoad);
				classMetadata.AddFieldIndices(_payLoad, oldSlot);
				_payLoad.WriteEncrypt();
				DeactivateCacheFor(id);
			}
			return true;
		}

		private void DeactivateCacheFor(int id)
		{
			Transaction().Deactivate(id, new FixedActivationDepth(1));
		}
	}
}
