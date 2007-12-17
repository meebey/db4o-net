/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Query.Result;

namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public class LazyClientQueryResult : AbstractQueryResult
	{
		private const int SIZE_NOT_SET = -1;

		private readonly ClientObjectContainer _client;

		private readonly int _queryResultID;

		private int _size = SIZE_NOT_SET;

		private readonly LazyClientIdIterator _iterator;

		public LazyClientQueryResult(Transaction trans, ClientObjectContainer client, int
			 queryResultID) : base(trans)
		{
			_client = client;
			_queryResultID = queryResultID;
			_iterator = new LazyClientIdIterator(this);
		}

		public override object Get(int index)
		{
			lock (Lock())
			{
				return ActivatedObject(GetId(index));
			}
		}

		public override int GetId(int index)
		{
			return AskServer(Msg.OBJECTSET_GET_ID, index);
		}

		public override int IndexOf(int id)
		{
			return AskServer(Msg.OBJECTSET_INDEXOF, id);
		}

		private int AskServer(MsgD message, int param)
		{
			_client.Write(message.GetWriterForInts(_transaction, new int[] { _queryResultID, 
				param }));
			return ((MsgD)_client.ExpectedResponse(message)).ReadInt();
		}

		public override IIntIterator4 IterateIDs()
		{
			return _iterator;
		}

		public override IEnumerator GetEnumerator()
		{
			return ClientServerPlatform.CreateClientQueryResultIterator(this);
		}

		public override int Size()
		{
			if (_size == SIZE_NOT_SET)
			{
				_client.Write(Msg.OBJECTSET_SIZE.GetWriterForInt(_transaction, _queryResultID));
				_size = ((MsgD)_client.ExpectedResponse(Msg.OBJECTSET_SIZE)).ReadInt();
			}
			return _size;
		}

		~LazyClientQueryResult()
		{
			_client.Write(Msg.OBJECTSET_FINALIZED.GetWriterForInt(_transaction, _queryResultID
				));
		}

		public override void LoadFromIdReader(BufferImpl reader)
		{
			_iterator.LoadFromIdReader(reader, reader.ReadInt());
		}

		public virtual void Reset()
		{
			_client.Write(Msg.OBJECTSET_RESET.GetWriterForInt(_transaction, _queryResultID));
		}

		public virtual void FetchIDs(int batchSize)
		{
			_client.Write(Msg.OBJECTSET_FETCH.GetWriterForInts(_transaction, new int[] { _queryResultID
				, batchSize }));
			BufferImpl reader = _client.ExpectedByteResponse(Msg.ID_LIST);
			LoadFromIdReader(reader);
		}
	}
}
