namespace Db4objects.Db4o.Internal.CS
{
	internal sealed class ClientTransaction : Db4objects.Db4o.Internal.Transaction
	{
		private readonly Db4objects.Db4o.Internal.CS.ClientObjectContainer i_client;

		private Db4objects.Db4o.Foundation.Tree i_yapObjectsToGc;

		internal ClientTransaction(Db4objects.Db4o.Internal.CS.ClientObjectContainer a_stream
			, Db4objects.Db4o.Internal.Transaction a_parent) : base(a_stream, a_parent)
		{
			i_client = a_stream;
		}

		public override void Commit()
		{
			CommitTransactionListeners();
			ClearAll();
			i_client.WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.COMMIT, true);
		}

		protected override void ClearAll()
		{
			RemoveYapObjectReferences();
			base.ClearAll();
		}

		private void RemoveYapObjectReferences()
		{
			if (i_yapObjectsToGc != null)
			{
				i_yapObjectsToGc.Traverse(new _AnonymousInnerClass33(this));
			}
			i_yapObjectsToGc = null;
		}

		private sealed class _AnonymousInnerClass33 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass33(ClientTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.Internal.ObjectReference yo = (Db4objects.Db4o.Internal.ObjectReference
					)((Db4objects.Db4o.Internal.TreeIntObject)a_object)._object;
				this._enclosing.Stream().RemoveReference(yo);
			}

			private readonly ClientTransaction _enclosing;
		}

		public override bool Delete(Db4objects.Db4o.Internal.ObjectReference @ref, int id
			, int cascade)
		{
			if (!base.Delete(@ref, id, cascade))
			{
				return false;
			}
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
				.TA_DELETE.GetWriterForInts(this, new int[] { id, cascade });
			i_client.WriteMsg(msg, false);
			return true;
		}

		public override bool IsDeleted(int a_id)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
				.TA_IS_DELETED.GetWriterForInt(this, a_id);
			i_client.WriteMsg(msg, true);
			int res = i_client.ExpectedByteResponse(Db4objects.Db4o.Internal.CS.Messages.Msg.
				TA_IS_DELETED).ReadInt();
			return res == 1;
		}

		public sealed override Db4objects.Db4o.Internal.HardObjectReference GetHardReferenceBySignature
			(long a_uuid, byte[] a_signature)
		{
			int messageLength = Db4objects.Db4o.Internal.Const4.LONG_LENGTH + Db4objects.Db4o.Internal.Const4
				.INT_LENGTH + a_signature.Length;
			Db4objects.Db4o.Internal.CS.Messages.MsgD message = Db4objects.Db4o.Internal.CS.Messages.Msg
				.OBJECT_BY_UUID.GetWriterForLength(this, messageLength);
			message.WriteLong(a_uuid);
			message.WriteBytes(a_signature);
			i_client.WriteMsg(message);
			message = (Db4objects.Db4o.Internal.CS.Messages.MsgD)i_client.ExpectedResponse(Db4objects.Db4o.Internal.CS.Messages.Msg
				.OBJECT_BY_UUID);
			int id = message.ReadInt();
			if (id > 0)
			{
				return Stream().GetHardObjectReferenceById(this, id);
			}
			return Db4objects.Db4o.Internal.HardObjectReference.INVALID;
		}

		public override void ProcessDeletes()
		{
			if (i_delete != null)
			{
				i_delete.Traverse(new _AnonymousInnerClass82(this));
			}
			i_delete = null;
			i_writtenUpdateDeletedMembers = null;
			i_client.WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.PROCESS_DELETES, false
				);
		}

		private sealed class _AnonymousInnerClass82 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass82(ClientTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.Internal.DeleteInfo info = (Db4objects.Db4o.Internal.DeleteInfo)a_object;
				if (info._reference != null)
				{
					this._enclosing.i_yapObjectsToGc = Db4objects.Db4o.Foundation.Tree.Add(this._enclosing
						.i_yapObjectsToGc, new Db4objects.Db4o.Internal.TreeIntObject(info._key, info._reference
						));
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

		public override void WriteUpdateDeleteMembers(int a_id, Db4objects.Db4o.Internal.ClassMetadata
			 a_yc, int a_type, int a_cascade)
		{
			Db4objects.Db4o.Internal.CS.Messages.MsgD msg = Db4objects.Db4o.Internal.CS.Messages.Msg
				.WRITE_UPDATE_DELETE_MEMBERS.GetWriterForInts(this, new int[] { a_id, a_yc.GetID
				(), a_type, a_cascade });
			i_client.WriteMsg(msg, false);
		}

		public override void SetPointer(int a_id, int a_address, int a_length)
		{
		}
	}
}
