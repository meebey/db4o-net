using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS
{
	internal sealed class ClientTransaction : Transaction
	{
		private readonly ClientObjectContainer i_client;

		private Tree i_yapObjectsToGc;

		internal ClientTransaction(ClientObjectContainer a_stream, Transaction a_parent) : 
			base(a_stream, a_parent)
		{
			i_client = a_stream;
		}

		public override void Commit()
		{
			CommitTransactionListeners();
			ClearAll();
			if (IsSystemTransaction())
			{
				i_client.WriteMsg(Msg.COMMIT_SYSTEMTRANS, true);
			}
			else
			{
				i_client.WriteMsg(Msg.COMMIT, true);
				i_client.ExpectedResponse(Msg.OK);
			}
		}

		protected override void Clear()
		{
			RemoveYapObjectReferences();
		}

		private void RemoveYapObjectReferences()
		{
			if (i_yapObjectsToGc != null)
			{
				i_yapObjectsToGc.Traverse(new _AnonymousInnerClass37(this));
			}
			i_yapObjectsToGc = null;
		}

		private sealed class _AnonymousInnerClass37 : IVisitor4
		{
			public _AnonymousInnerClass37(ClientTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				ObjectReference yo = (ObjectReference)((TreeIntObject)a_object)._object;
				this._enclosing.Stream().RemoveReference(yo);
			}

			private readonly ClientTransaction _enclosing;
		}

		public override bool Delete(ObjectReference @ref, int id, int cascade)
		{
			if (!base.Delete(@ref, id, cascade))
			{
				return false;
			}
			MsgD msg = Msg.TA_DELETE.GetWriterForInts(this, new int[] { id, cascade });
			i_client.WriteMsg(msg, false);
			return true;
		}

		public override bool IsDeleted(int a_id)
		{
			MsgD msg = Msg.TA_IS_DELETED.GetWriterForInt(this, a_id);
			i_client.WriteMsg(msg, true);
			int res = i_client.ExpectedByteResponse(Msg.TA_IS_DELETED).ReadInt();
			return res == 1;
		}

		public sealed override HardObjectReference GetHardReferenceBySignature(long a_uuid
			, byte[] a_signature)
		{
			int messageLength = Const4.LONG_LENGTH + Const4.INT_LENGTH + a_signature.Length;
			MsgD message = Msg.OBJECT_BY_UUID.GetWriterForLength(this, messageLength);
			message.WriteLong(a_uuid);
			message.WriteBytes(a_signature);
			i_client.Write(message);
			message = (MsgD)i_client.ExpectedResponse(Msg.OBJECT_BY_UUID);
			int id = message.ReadInt();
			if (id > 0)
			{
				return Stream().GetHardObjectReferenceById(this, id);
			}
			return HardObjectReference.INVALID;
		}

		public override void ProcessDeletes()
		{
			if (i_delete != null)
			{
				i_delete.Traverse(new _AnonymousInnerClass86(this));
			}
			i_delete = null;
			i_client.WriteMsg(Msg.PROCESS_DELETES, false);
		}

		private sealed class _AnonymousInnerClass86 : IVisitor4
		{
			public _AnonymousInnerClass86(ClientTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				DeleteInfo info = (DeleteInfo)a_object;
				if (info._reference != null)
				{
					this._enclosing.i_yapObjectsToGc = Tree.Add(this._enclosing.i_yapObjectsToGc, new 
						TreeIntObject(info._key, info._reference));
				}
			}

			private readonly ClientTransaction _enclosing;
		}

		public override void Rollback()
		{
			i_yapObjectsToGc = null;
			RollBackTransactionListeners();
			ClearAll();
		}

		public override void WriteUpdateDeleteMembers(int a_id, ClassMetadata a_yc, int a_type
			, int a_cascade)
		{
			MsgD msg = Msg.WRITE_UPDATE_DELETE_MEMBERS.GetWriterForInts(this, new int[] { a_id
				, a_yc.GetID(), a_type, a_cascade });
			i_client.WriteMsg(msg, false);
		}

		public override void SetPointer(int a_id, int a_address, int a_length)
		{
		}
	}
}
