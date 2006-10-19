namespace Db4objects.Db4o.Inside.Marshall
{
	public abstract class ObjectMarshaller
	{
		public Db4objects.Db4o.Inside.Marshall.MarshallerFamily _family;

		protected abstract class TraverseFieldCommand
		{
			private bool _cancelled = false;

			public virtual int FieldCount(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.YapReader
				 reader)
			{
				return (Db4objects.Db4o.Debug.atHome ? yapClass.ReadFieldCountSodaAtHome(reader) : 
					yapClass.ReadFieldCount(reader));
			}

			public virtual bool Cancelled()
			{
				return _cancelled;
			}

			protected virtual void Cancel()
			{
				_cancelled = true;
			}

			public abstract void ProcessField(Db4objects.Db4o.YapField field, bool isNull, Db4objects.Db4o.YapClass
				 containingClass);
		}

		protected virtual void TraverseFields(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.YapReader
			 reader, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes attributes, Db4objects.Db4o.Inside.Marshall.ObjectMarshaller.TraverseFieldCommand
			 command)
		{
			int fieldIndex = 0;
			while (yc != null && !command.Cancelled())
			{
				int fieldCount = command.FieldCount(yc, reader);
				for (int i = 0; i < fieldCount && !command.Cancelled(); i++)
				{
					command.ProcessField(yc.i_fields[i], IsNull(attributes, fieldIndex), yc);
					fieldIndex++;
				}
				yc = yc.i_ancestor;
			}
		}

		protected abstract bool IsNull(Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, int fieldIndex);

		public abstract void AddFieldIndices(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapWriter writer, Db4objects.Db4o.Inside.Slots.Slot
			 oldSlot);

		public abstract Db4objects.Db4o.TreeInt CollectFieldIDs(Db4objects.Db4o.TreeInt tree
			, Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapWriter reader, string name);

		protected virtual Db4objects.Db4o.YapWriter CreateWriterForNew(Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.YapObject yo, int updateDepth, int length)
		{
			int id = yo.GetID();
			int address = -1;
			if (!trans.Stream().IsClient())
			{
				address = trans.i_file.GetSlot(length);
			}
			trans.SetPointer(id, address, length);
			return CreateWriterForUpdate(trans, updateDepth, id, address, length);
		}

		protected virtual Db4objects.Db4o.YapWriter CreateWriterForUpdate(Db4objects.Db4o.Transaction
			 a_trans, int updateDepth, int id, int address, int length)
		{
			Db4objects.Db4o.YapWriter writer = new Db4objects.Db4o.YapWriter(a_trans, length);
			writer.UseSlot(id, address, length);
			writer.SetUpdateDepth(updateDepth);
			return writer;
		}

		public abstract void DeleteMembers(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapWriter writer, int a_type, bool isUpdate);

		public abstract bool FindOffset(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapReader reader, Db4objects.Db4o.YapField field);

		public abstract void InstantiateFields(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapObject yo, object obj, Db4objects.Db4o.YapWriter
			 reader);

		public abstract Db4objects.Db4o.YapWriter MarshallNew(Db4objects.Db4o.Transaction
			 a_trans, Db4objects.Db4o.YapObject yo, int a_updateDepth);

		public abstract void MarshallUpdate(Db4objects.Db4o.Transaction a_trans, int a_updateDepth
			, Db4objects.Db4o.YapObject a_yapObject, object a_object);

		protected virtual void MarshallUpdateWrite(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapObject
			 yo, object obj, Db4objects.Db4o.YapWriter writer)
		{
			Db4objects.Db4o.YapClass yc = yo.GetYapClass();
			Db4objects.Db4o.YapStream stream = trans.Stream();
			stream.WriteUpdate(yc, writer);
			if (yo.IsActive())
			{
				yo.SetStateClean();
			}
			yo.EndProcessing();
			ObjectOnUpdate(yc, stream, obj);
		}

		private void ObjectOnUpdate(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.YapStream
			 stream, object obj)
		{
			stream.Callbacks().ObjectOnUpdate(obj);
			yc.DispatchEvent(stream, obj, Db4objects.Db4o.EventDispatcher.UPDATE);
		}

		public abstract object ReadIndexEntry(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapField yf, Db4objects.Db4o.YapWriter reader);

		public abstract Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes ReadHeaderAttributes
			(Db4objects.Db4o.YapReader reader);

		public abstract void ReadVirtualAttributes(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapClass
			 yc, Db4objects.Db4o.YapObject yo, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapReader reader);

		public abstract void DefragFields(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.Inside.Marshall.ObjectHeader
			 header, Db4objects.Db4o.ReaderPair readers);

		public abstract void WriteObjectClassID(Db4objects.Db4o.YapReader reader, int id);

		public abstract void SkipMarshallerInfo(Db4objects.Db4o.YapReader reader);

		public abstract void MapStringIDs(Db4objects.Db4o.YapClass yc, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapReader reader, Db4objects.Db4o.IIDMapping mapping
			, int sourceBaseID, int targetBaseID);
	}
}
