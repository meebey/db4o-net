namespace Db4objects.Db4o
{
	/// <summary>
	/// public for .NET conversion reasons
	/// TODO: Split this class for individual usecases.
	/// </summary>
	/// <remarks>
	/// public for .NET conversion reasons
	/// TODO: Split this class for individual usecases. Only use the member
	/// variables needed for the respective usecase.
	/// </remarks>
	/// <exclude></exclude>
	public sealed class YapWriter : Db4objects.Db4o.YapReader
	{
		private int i_address;

		private int _addressOffset;

		private int i_cascadeDelete;

		private Db4objects.Db4o.Foundation.Tree i_embedded;

		private int i_id;

		private int i_instantionDepth;

		private int i_length;

		internal Db4objects.Db4o.Transaction i_trans;

		private int i_updateDepth = 1;

		public int _payloadOffset;

		public YapWriter(Db4objects.Db4o.Transaction a_trans, int a_initialBufferSize)
		{
			i_trans = a_trans;
			i_length = a_initialBufferSize;
			_buffer = new byte[i_length];
		}

		public YapWriter(Db4objects.Db4o.Transaction a_trans, int a_address, int a_initialBufferSize
			) : this(a_trans, a_initialBufferSize)
		{
			i_address = a_address;
		}

		public YapWriter(Db4objects.Db4o.YapWriter parent, Db4objects.Db4o.YapWriter[] previousRead
			, int previousCount)
		{
			previousRead[previousCount++] = this;
			int parentID = parent.ReadInt();
			i_length = parent.ReadInt();
			i_id = parent.ReadInt();
			previousRead[parentID].AddEmbedded(this);
			i_address = parent.ReadInt();
			i_trans = parent.GetTransaction();
			_buffer = new byte[i_length];
			System.Array.Copy(parent._buffer, parent._offset, _buffer, 0, i_length);
			parent._offset += i_length;
			if (previousCount < previousRead.Length)
			{
				new Db4objects.Db4o.YapWriter(parent, previousRead, previousCount);
			}
		}

		public void AddEmbedded(Db4objects.Db4o.YapWriter a_bytes)
		{
			i_embedded = Db4objects.Db4o.Foundation.Tree.Add(i_embedded, new Db4objects.Db4o.TreeIntObject
				(a_bytes.GetID(), a_bytes));
		}

		public int AppendTo(Db4objects.Db4o.YapReader a_bytes, int a_id)
		{
			a_id++;
			a_bytes.WriteInt(i_length);
			a_bytes.WriteInt(i_id);
			a_bytes.WriteInt(i_address);
			a_bytes.Append(_buffer);
			int[] newID = { a_id };
			int myID = a_id;
			ForEachEmbedded(new _AnonymousInnerClass95(this, a_bytes, myID, newID));
			return newID[0];
		}

		private sealed class _AnonymousInnerClass95 : Db4objects.Db4o.IVisitorYapBytes
		{
			public _AnonymousInnerClass95(YapWriter _enclosing, Db4objects.Db4o.YapReader a_bytes
				, int myID, int[] newID)
			{
				this._enclosing = _enclosing;
				this.a_bytes = a_bytes;
				this.myID = myID;
				this.newID = newID;
			}

			public void Visit(Db4objects.Db4o.YapWriter a_embedded)
			{
				a_bytes.WriteInt(myID);
				newID[0] = a_embedded.AppendTo(a_bytes, newID[0]);
			}

			private readonly YapWriter _enclosing;

			private readonly Db4objects.Db4o.YapReader a_bytes;

			private readonly int myID;

			private readonly int[] newID;
		}

		public int CascadeDeletes()
		{
			return i_cascadeDelete;
		}

		public void DebugCheckBytes()
		{
		}

		public int EmbeddedCount()
		{
			int[] count = { 0 };
			ForEachEmbedded(new _AnonymousInnerClass120(this, count));
			return count[0];
		}

		private sealed class _AnonymousInnerClass120 : Db4objects.Db4o.IVisitorYapBytes
		{
			public _AnonymousInnerClass120(YapWriter _enclosing, int[] count)
			{
				this._enclosing = _enclosing;
				this.count = count;
			}

			public void Visit(Db4objects.Db4o.YapWriter a_bytes)
			{
				count[0] += 1 + a_bytes.EmbeddedCount();
			}

			private readonly YapWriter _enclosing;

			private readonly int[] count;
		}

		public int EmbeddedLength()
		{
			int[] length = { 0 };
			ForEachEmbedded(new _AnonymousInnerClass130(this, length));
			return length[0];
		}

		private sealed class _AnonymousInnerClass130 : Db4objects.Db4o.IVisitorYapBytes
		{
			public _AnonymousInnerClass130(YapWriter _enclosing, int[] length)
			{
				this._enclosing = _enclosing;
				this.length = length;
			}

			public void Visit(Db4objects.Db4o.YapWriter a_bytes)
			{
				length[0] += a_bytes.GetLength() + a_bytes.EmbeddedLength();
			}

			private readonly YapWriter _enclosing;

			private readonly int[] length;
		}

		internal void ForEachEmbedded(Db4objects.Db4o.IVisitorYapBytes a_visitor)
		{
			if (i_embedded != null)
			{
				i_embedded.Traverse(new _AnonymousInnerClass140(this, a_visitor));
			}
		}

		private sealed class _AnonymousInnerClass140 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass140(YapWriter _enclosing, Db4objects.Db4o.IVisitorYapBytes
				 a_visitor)
			{
				this._enclosing = _enclosing;
				this.a_visitor = a_visitor;
			}

			public void Visit(object a_object)
			{
				a_visitor.Visit((Db4objects.Db4o.YapWriter)((Db4objects.Db4o.TreeIntObject)a_object
					)._object);
			}

			private readonly YapWriter _enclosing;

			private readonly Db4objects.Db4o.IVisitorYapBytes a_visitor;
		}

		public int GetAddress()
		{
			return i_address;
		}

		public int AddressOffset()
		{
			return _addressOffset;
		}

		public int GetID()
		{
			return i_id;
		}

		public int GetInstantiationDepth()
		{
			return i_instantionDepth;
		}

		public override int GetLength()
		{
			return i_length;
		}

		public Db4objects.Db4o.YapStream GetStream()
		{
			return i_trans.Stream();
		}

		public Db4objects.Db4o.YapStream Stream()
		{
			return i_trans.Stream();
		}

		public Db4objects.Db4o.YapFile File()
		{
			return i_trans.i_file;
		}

		public Db4objects.Db4o.Transaction GetTransaction()
		{
			return i_trans;
		}

		public int GetUpdateDepth()
		{
			return i_updateDepth;
		}

		internal byte[] GetWrittenBytes()
		{
			byte[] bytes = new byte[_offset];
			System.Array.Copy(_buffer, 0, bytes, 0, _offset);
			return bytes;
		}

		public int PreparePayloadRead()
		{
			int newPayLoadOffset = ReadInt();
			int length = ReadInt();
			int linkOffSet = _offset;
			_offset = newPayLoadOffset;
			_payloadOffset += length;
			return linkOffSet;
		}

		public void Read()
		{
			Stream().ReadBytes(_buffer, i_address, _addressOffset, i_length);
		}

		public bool Read(Db4objects.Db4o.Foundation.Network.IYapSocket sock)
		{
			int offset = 0;
			int length = i_length;
			while (length > 0)
			{
				int read = sock.Read(_buffer, offset, length);
				if (read < 0)
				{
					return false;
				}
				offset += read;
				length -= read;
			}
			return true;
		}

		public Db4objects.Db4o.YapWriter ReadEmbeddedObject()
		{
			int id = ReadInt();
			int length = ReadInt();
			Db4objects.Db4o.YapWriter bytes = null;
			Db4objects.Db4o.Foundation.Tree tio = Db4objects.Db4o.TreeInt.Find(i_embedded, id
				);
			if (tio != null)
			{
				bytes = (Db4objects.Db4o.YapWriter)((Db4objects.Db4o.TreeIntObject)tio)._object;
			}
			else
			{
				bytes = Stream().ReadWriterByAddress(i_trans, id, length);
				if (bytes != null)
				{
					bytes.SetID(id);
				}
			}
			if (bytes != null)
			{
				bytes.SetUpdateDepth(GetUpdateDepth());
				bytes.SetInstantiationDepth(GetInstantiationDepth());
			}
			return bytes;
		}

		public Db4objects.Db4o.YapWriter ReadYapBytes()
		{
			int length = ReadInt();
			if (length == 0)
			{
				return null;
			}
			Db4objects.Db4o.YapWriter yb = new Db4objects.Db4o.YapWriter(i_trans, length);
			System.Array.Copy(_buffer, _offset, yb._buffer, 0, length);
			_offset += length;
			return yb;
		}

		public void RemoveFirstBytes(int aLength)
		{
			i_length -= aLength;
			byte[] temp = new byte[i_length];
			System.Array.Copy(_buffer, aLength, temp, 0, i_length);
			_buffer = temp;
			_offset -= aLength;
			if (_offset < 0)
			{
				_offset = 0;
			}
		}

		public void Address(int a_address)
		{
			i_address = a_address;
		}

		public void SetCascadeDeletes(int depth)
		{
			i_cascadeDelete = depth;
		}

		public void SetID(int a_id)
		{
			i_id = a_id;
		}

		public void SetInstantiationDepth(int a_depth)
		{
			i_instantionDepth = a_depth;
		}

		public void SetTransaction(Db4objects.Db4o.Transaction aTrans)
		{
			i_trans = aTrans;
		}

		public void SetUpdateDepth(int a_depth)
		{
			i_updateDepth = a_depth;
		}

		public void SlotDelete()
		{
			i_trans.SlotDelete(i_id, i_address, i_length);
		}

		public void Trim4(int a_offset, int a_length)
		{
			byte[] temp = new byte[a_length];
			System.Array.Copy(_buffer, a_offset, temp, 0, a_length);
			_buffer = temp;
			i_length = a_length;
		}

		internal void UseSlot(int a_adress)
		{
			i_address = a_adress;
			_offset = 0;
		}

		public void UseSlot(int a_adress, int a_length)
		{
			i_address = a_adress;
			_offset = 0;
			if (a_length > _buffer.Length)
			{
				_buffer = new byte[a_length];
			}
			i_length = a_length;
		}

		public void UseSlot(int a_id, int a_adress, int a_length)
		{
			i_id = a_id;
			UseSlot(a_adress, a_length);
		}

		public void Write()
		{
			File().WriteBytes(this, i_address, _addressOffset);
		}

		public void WriteEmbedded()
		{
			Db4objects.Db4o.YapWriter finalThis = this;
			ForEachEmbedded(new _AnonymousInnerClass326(this, finalThis));
			i_embedded = null;
		}

		private sealed class _AnonymousInnerClass326 : Db4objects.Db4o.IVisitorYapBytes
		{
			public _AnonymousInnerClass326(YapWriter _enclosing, Db4objects.Db4o.YapWriter finalThis
				)
			{
				this._enclosing = _enclosing;
				this.finalThis = finalThis;
			}

			public void Visit(Db4objects.Db4o.YapWriter a_bytes)
			{
				a_bytes.WriteEmbedded();
				this._enclosing.Stream().WriteEmbedded(finalThis, a_bytes);
			}

			private readonly YapWriter _enclosing;

			private readonly Db4objects.Db4o.YapWriter finalThis;
		}

		public void WriteEmbeddedNull()
		{
			WriteInt(0);
			WriteInt(0);
		}

		public void WriteEncrypt()
		{
			WriteEncrypt(File(), i_address, _addressOffset);
		}

		public void WritePayload(Db4objects.Db4o.YapWriter payLoad, bool topLevel)
		{
			CheckMinimumPayLoadOffsetAndWritePointerAndLength(payLoad.GetLength(), topLevel);
			System.Array.Copy(payLoad._buffer, 0, _buffer, _payloadOffset, payLoad._buffer.Length
				);
			TransferPayLoadAddress(payLoad, _payloadOffset);
			_payloadOffset += payLoad._buffer.Length;
		}

		private void CheckMinimumPayLoadOffsetAndWritePointerAndLength(int length, bool alignToBlockSize
			)
		{
			if (_payloadOffset <= _offset + (Db4objects.Db4o.YapConst.INT_LENGTH * 2))
			{
				_payloadOffset = _offset + (Db4objects.Db4o.YapConst.INT_LENGTH * 2);
			}
			if (alignToBlockSize)
			{
				_payloadOffset = Stream().AlignToBlockSize(_payloadOffset);
			}
			WriteInt(_payloadOffset);
			WriteInt(length);
		}

		public int ReserveAndPointToPayLoadSlot(int length)
		{
			CheckMinimumPayLoadOffsetAndWritePointerAndLength(length, false);
			int linkOffset = _offset;
			_offset = _payloadOffset;
			_payloadOffset += length;
			return linkOffset;
		}

		public Db4objects.Db4o.YapReader ReadPayloadWriter(int offset, int length)
		{
			Db4objects.Db4o.YapWriter payLoad = new Db4objects.Db4o.YapWriter(i_trans, 0, length
				);
			System.Array.Copy(_buffer, offset, payLoad._buffer, 0, length);
			TransferPayLoadAddress(payLoad, offset);
			return payLoad;
		}

		private void TransferPayLoadAddress(Db4objects.Db4o.YapWriter toWriter, int offset
			)
		{
			int blockedOffset = offset / Stream().BlockSize();
			toWriter.i_address = i_address + blockedOffset;
			toWriter.i_id = toWriter.i_address;
			toWriter._addressOffset = _addressOffset;
		}

		internal void WriteShortString(string a_string)
		{
			WriteShortString(i_trans, a_string);
		}

		public void MoveForward(int length)
		{
			_addressOffset += length;
		}

		public void WriteForward()
		{
			Write();
			_addressOffset += i_length;
			_offset = 0;
		}

		public override string ToString()
		{
			return base.ToString();
			return "id " + i_id + " adr " + i_address + " len " + i_length;
		}

		public void NoXByteCheck()
		{
			if (Db4objects.Db4o.Debug.xbytes && Db4objects.Db4o.Deploy.overwrite)
			{
				SetID(Db4objects.Db4o.YapConst.IGNORE_ID);
			}
		}

		public void WriteIDs(Db4objects.Db4o.Foundation.IIntIterator4 idIterator, int maxCount
			)
		{
			int savedOffset = _offset;
			WriteInt(0);
			int actualCount = 0;
			while (idIterator.MoveNext())
			{
				WriteInt(idIterator.CurrentInt());
				actualCount++;
				if (actualCount >= maxCount)
				{
					break;
				}
			}
			int secondSavedOffset = _offset;
			_offset = savedOffset;
			WriteInt(actualCount);
			_offset = secondSavedOffset;
		}
	}
}
