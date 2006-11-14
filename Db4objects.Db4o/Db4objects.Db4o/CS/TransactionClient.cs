namespace Db4objects.Db4o.CS
{
	internal sealed class TransactionClient : Db4objects.Db4o.Transaction
	{
		private readonly Db4objects.Db4o.CS.YapClient i_client;

		private Db4objects.Db4o.Foundation.Tree i_yapObjectsToGc;

		internal TransactionClient(Db4objects.Db4o.CS.YapClient a_stream, Db4objects.Db4o.Transaction
			 a_parent) : base(a_stream, a_parent)
		{
			i_client = a_stream;
		}

		public override void BeginEndSet()
		{
			if (i_delete != null)
			{
				i_delete.Traverse(new _AnonymousInnerClass22(this));
			}
			i_delete = null;
			i_writtenUpdateDeletedMembers = null;
			i_client.WriteMsg(Db4objects.Db4o.CS.Messages.Msg.TA_BEGIN_END_SET);
		}

		private sealed class _AnonymousInnerClass22 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass22(TransactionClient _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.DeleteInfo info = (Db4objects.Db4o.DeleteInfo)a_object;
				if (info._delete && info._reference != null)
				{
					this._enclosing.i_yapObjectsToGc = Db4objects.Db4o.Foundation.Tree.Add(this._enclosing
						.i_yapObjectsToGc, new Db4objects.Db4o.TreeIntObject(info._key, info._reference)
						);
				}
			}

			private readonly TransactionClient _enclosing;
		}

		public override void Commit()
		{
			CommitTransactionListeners();
			if (i_yapObjectsToGc != null)
			{
				i_yapObjectsToGc.Traverse(new _AnonymousInnerClass39(this));
			}
			i_yapObjectsToGc = null;
			i_client.WriteMsg(Db4objects.Db4o.CS.Messages.Msg.COMMIT);
		}

		private sealed class _AnonymousInnerClass39 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass39(TransactionClient _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.YapObject yo = (Db4objects.Db4o.YapObject)((Db4objects.Db4o.TreeIntObject
					)a_object)._object;
				this._enclosing.Stream().RemoveReference(yo);
			}

			private readonly TransactionClient _enclosing;
		}

		public override void Delete(Db4objects.Db4o.YapObject a_yo, int a_cascade)
		{
			base.Delete(a_yo, a_cascade);
			i_client.WriteMsg(Db4objects.Db4o.CS.Messages.Msg.TA_DELETE.GetWriterForInts(this
				, new int[] { a_yo.GetID(), a_cascade }));
		}

		public override void DontDelete(int classID, int a_id)
		{
			base.DontDelete(classID, a_id);
			i_client.WriteMsg(Db4objects.Db4o.CS.Messages.Msg.TA_DONT_DELETE.GetWriterForInts
				(this, new int[] { classID, a_id }));
		}

		public override bool IsDeleted(int a_id)
		{
			i_client.WriteMsg(Db4objects.Db4o.CS.Messages.Msg.TA_IS_DELETED.GetWriterForInt(this
				, a_id));
			int res = i_client.ExpectedByteResponse(Db4objects.Db4o.CS.Messages.Msg.TA_IS_DELETED
				).ReadInt();
			return res == 1;
		}

		public override object[] ObjectAndYapObjectBySignature(long a_uuid, byte[] a_signature
			)
		{
			int messageLength = Db4objects.Db4o.YapConst.LONG_LENGTH + Db4objects.Db4o.YapConst
				.INT_LENGTH + a_signature.Length;
			Db4objects.Db4o.CS.Messages.MsgD message = Db4objects.Db4o.CS.Messages.Msg.OBJECT_BY_UUID
				.GetWriterForLength(this, messageLength);
			message.WriteLong(a_uuid);
			message.WriteBytes(a_signature);
			i_client.WriteMsg(message);
			message = (Db4objects.Db4o.CS.Messages.MsgD)i_client.ExpectedResponse(Db4objects.Db4o.CS.Messages.Msg
				.OBJECT_BY_UUID);
			int id = message.ReadInt();
			if (id > 0)
			{
				return Stream().GetObjectAndYapObjectByID(this, id);
			}
			return new object[2];
		}

		public override void Rollback()
		{
			i_yapObjectsToGc = null;
			RollBackTransactionListeners();
		}

		public override void WriteUpdateDeleteMembers(int a_id, Db4objects.Db4o.YapClass 
			a_yc, int a_type, int a_cascade)
		{
			i_client.WriteMsg(Db4objects.Db4o.CS.Messages.Msg.WRITE_UPDATE_DELETE_MEMBERS.GetWriterForInts
				(this, new int[] { a_id, a_yc.GetID(), a_type, a_cascade }));
		}
	}
}
