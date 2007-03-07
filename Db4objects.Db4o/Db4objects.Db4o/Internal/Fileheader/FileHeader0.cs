namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class FileHeader0 : Db4objects.Db4o.Internal.Fileheader.FileHeader
	{
		internal const int LENGTH = 2 + (Db4objects.Db4o.Internal.Const4.INT_LENGTH * 4);

		private Db4objects.Db4o.Internal.ConfigBlock _configBlock;

		private Db4objects.Db4o.PBootRecord _bootRecord;

		public override void Close()
		{
			_configBlock.Close();
		}

		protected override Db4objects.Db4o.Internal.Fileheader.FileHeader NewOnSignatureMatch
			(Db4objects.Db4o.Internal.LocalObjectContainer file, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			byte firstFileByte = reader.ReadByte();
			if (firstFileByte != Db4objects.Db4o.Internal.Const4.YAPBEGIN)
			{
				if (firstFileByte != Db4objects.Db4o.Internal.Const4.YAPFILEVERSION)
				{
					return null;
				}
				file.BlockSizeReadFromFile(reader.ReadByte());
			}
			else
			{
				if (reader.ReadByte() != Db4objects.Db4o.Internal.Const4.YAPFILE)
				{
					return null;
				}
			}
			return new Db4objects.Db4o.Internal.Fileheader.FileHeader0();
		}

		protected override void ReadFixedPart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, Db4objects.Db4o.Internal.Buffer reader)
		{
			_configBlock = Db4objects.Db4o.Internal.ConfigBlock.ForExistingFile(file, reader.
				ReadInt());
			SkipConfigurationLockTime(reader);
			ReadClassCollectionAndFreeSpace(file, reader);
		}

		private void SkipConfigurationLockTime(Db4objects.Db4o.Internal.Buffer reader)
		{
			reader.IncrementOffset(Db4objects.Db4o.Internal.Const4.ID_LENGTH);
		}

		public override void ReadVariablePart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file)
		{
			if (_configBlock._bootRecordID <= 0)
			{
				return;
			}
			object bootRecord = GetBootRecord(file);
			if (!(bootRecord is Db4objects.Db4o.PBootRecord))
			{
				InitBootRecord(file);
				file.GenerateNewIdentity();
				return;
			}
			_bootRecord = (Db4objects.Db4o.PBootRecord)bootRecord;
			file.Activate(bootRecord, int.MaxValue);
			file.SetNextTimeStampId(_bootRecord.i_versionGenerator);
			file.SystemData().Identity(_bootRecord.i_db);
		}

		private object GetBootRecord(Db4objects.Db4o.Internal.LocalObjectContainer file)
		{
			file.ShowInternalClasses(true);
			try
			{
				return file.GetByID1(file.GetSystemTransaction(), _configBlock._bootRecordID);
			}
			finally
			{
				file.ShowInternalClasses(false);
			}
		}

		public override void InitNew(Db4objects.Db4o.Internal.LocalObjectContainer file)
		{
			_configBlock = Db4objects.Db4o.Internal.ConfigBlock.ForNewFile(file);
			InitBootRecord(file);
		}

		private void InitBootRecord(Db4objects.Db4o.Internal.LocalObjectContainer file)
		{
			file.ShowInternalClasses(true);
			try
			{
				_bootRecord = new Db4objects.Db4o.PBootRecord();
				file.SetInternal(file.GetSystemTransaction(), _bootRecord, false);
				_configBlock._bootRecordID = file.GetID1(_bootRecord);
				WriteVariablePart(file, 1);
			}
			finally
			{
				file.ShowInternalClasses(false);
			}
		}

		public override Db4objects.Db4o.Internal.Transaction InterruptedTransaction()
		{
			return _configBlock.GetTransactionToCommit();
		}

		public override void WriteTransactionPointer(Db4objects.Db4o.Internal.Transaction
			 systemTransaction, int transactionAddress)
		{
			WriteTransactionPointer(systemTransaction, transactionAddress, _configBlock.Address
				(), Db4objects.Db4o.Internal.ConfigBlock.TRANSACTION_OFFSET);
		}

		public virtual Db4objects.Db4o.MetaIndex GetUUIDMetaIndex()
		{
			return _bootRecord.GetUUIDMetaIndex();
		}

		public override int Length()
		{
			return LENGTH;
		}

		public override void WriteFixedPart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, bool startFileLockingThread, bool shuttingDown, Db4objects.Db4o.Internal.StatefulBuffer
			 writer, int blockSize_, int freespaceID)
		{
			writer.Append(Db4objects.Db4o.Internal.Const4.YAPFILEVERSION);
			writer.Append((byte)blockSize_);
			writer.WriteInt(_configBlock.Address());
			writer.WriteInt((int)TimeToWrite(_configBlock.OpenTime(), shuttingDown));
			writer.WriteInt(file.SystemData().ClassCollectionID());
			writer.WriteInt(freespaceID);
			if (Db4objects.Db4o.Debug.xbytes && Db4objects.Db4o.Deploy.overwrite)
			{
				writer.SetID(Db4objects.Db4o.Internal.Const4.IGNORE_ID);
			}
			writer.Write();
		}

		public override void WriteVariablePart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, int part)
		{
			if (part == 1)
			{
				_configBlock.Write();
			}
			else
			{
				if (part == 2)
				{
					_bootRecord.Write(file);
				}
			}
		}
	}
}
