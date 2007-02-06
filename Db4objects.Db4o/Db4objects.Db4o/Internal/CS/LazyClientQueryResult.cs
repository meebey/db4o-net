namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public class LazyClientQueryResult : Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult
	{
		private const int SIZE_NOT_SET = -1;

		private readonly Db4objects.Db4o.Internal.CS.ClientObjectContainer _client;

		private readonly int _queryResultID;

		private int _size = SIZE_NOT_SET;

		private readonly Db4objects.Db4o.Internal.CS.LazyClientIdIterator _iterator;

		public LazyClientQueryResult(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.CS.ClientObjectContainer
			 client, int queryResultID) : base(trans)
		{
			_client = client;
			_queryResultID = queryResultID;
			_iterator = new Db4objects.Db4o.Internal.CS.LazyClientIdIterator(this);
		}

		public override object Get(int index)
		{
			lock (StreamLock())
			{
				return ActivatedObject(GetId(index));
			}
		}

		public override int GetId(int index)
		{
			return AskServer(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECTSET_GET_ID, index
				);
		}

		public override int IndexOf(int id)
		{
			return AskServer(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECTSET_INDEXOF, id);
		}

		private int AskServer(Db4objects.Db4o.Internal.CS.Messages.MsgD message, int param
			)
		{
			_client.WriteMsg(message.GetWriterForInts(_transaction, new int[] { _queryResultID
				, param }));
			return ((Db4objects.Db4o.Internal.CS.Messages.MsgD)_client.ExpectedResponse(message
				)).ReadInt();
		}

		public override Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			return _iterator;
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return new Db4objects.Db4o.Internal.CS.ClientQueryResultIterator(this);
		}

		public override int Size()
		{
			if (_size == SIZE_NOT_SET)
			{
				_client.WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECTSET_SIZE.GetWriterForInt
					(_transaction, _queryResultID));
				_size = ((Db4objects.Db4o.Internal.CS.Messages.MsgD)_client.ExpectedResponse(Db4objects.Db4o.Internal.CS.Messages.Msg
					.OBJECTSET_SIZE)).ReadInt();
			}
			return _size;
		}

		~LazyClientQueryResult()
		{
			_client.WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECTSET_FINALIZED.GetWriterForInt
				(_transaction, _queryResultID));
		}

		public override void LoadFromIdReader(Db4objects.Db4o.Internal.Buffer reader)
		{
			_iterator.LoadFromIdReader(reader, reader.ReadInt());
		}

		public virtual void Reset()
		{
			_client.WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECTSET_RESET.GetWriterForInt
				(_transaction, _queryResultID));
		}

		public virtual void FetchIDs(int batchSize)
		{
			_client.WriteMsg(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECTSET_FETCH.GetWriterForInts
				(_transaction, new int[] { _queryResultID, batchSize }));
			Db4objects.Db4o.Internal.Buffer reader = _client.ExpectedByteResponse(Db4objects.Db4o.Internal.CS.Messages.Msg
				.ID_LIST);
			LoadFromIdReader(reader);
		}
	}
}
