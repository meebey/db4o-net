using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MWriteUpdate : MsgObject, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int yapClassId = _payLoad.ReadInt();
			LocalObjectContainer stream = (LocalObjectContainer)Stream();
			Unmarshall(_payLoad._offset);
			lock (StreamLock())
			{
				ClassMetadata yc = stream.ClassMetadataForId(yapClassId);
				_payLoad.WriteEmbedded();
				int id = _payLoad.GetID();
				Transaction().DontDelete(id);
				Slot oldSlot = ((LocalTransaction)Transaction()).GetCommittedSlotOfID(id);
				stream.GetSlotForUpdate(_payLoad);
				yc.AddFieldIndices(_payLoad, oldSlot);
				_payLoad.WriteEncrypt();
			}
			return true;
		}
	}
}
