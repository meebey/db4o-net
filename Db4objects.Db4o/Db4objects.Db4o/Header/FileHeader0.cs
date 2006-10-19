namespace Db4objects.Db4o.Header
{
	/// <exclude></exclude>
	public class FileHeader0 : Db4objects.Db4o.Header.FileHeader
	{
		internal const int LENGTH = 2 + (Db4objects.Db4o.YapConst.INT_LENGTH * 4);

		private Db4objects.Db4o.YapConfigBlock _configBlock;

		private Db4objects.Db4o.PBootRecord _bootRecord;

		public override void Close()
		{
			_configBlock.Close();
		}

		protected override Db4objects.Db4o.Header.FileHeader NewOnSignatureMatch(Db4objects.Db4o.YapFile
			 file, Db4objects.Db4o.YapReader reader)
		{
			byte firstFileByte = reader.ReadByte();
			if (firstFileByte != Db4objects.Db4o.YapConst.YAPBEGIN)
			{
				if (firstFileByte != Db4objects.Db4o.YapConst.YAPFILEVERSION)
				{
					return null;
				}
				file.BlockSizeReadFromFile(reader.ReadByte());
			}
			else
			{
				if (reader.ReadByte() != Db4objects.Db4o.YapConst.YAPFILE)
				{
					return null;
				}
			}
			return new Db4objects.Db4o.Header.FileHeader0();
		}

		protected override void ReadFixedPart(Db4objects.Db4o.YapFile file, Db4objects.Db4o.YapReader
			 reader)
		{
			_configBlock = Db4objects.Db4o.YapConfigBlock.ForExistingFile(file, reader.ReadInt
				());
			SkipConfigurationLockTime(reader);
			ReadClassCollectionAndFreeSpace(file, reader);
		}

		private void SkipConfigurationLockTime(Db4objects.Db4o.YapReader reader)
		{
			reader.IncrementOffset(Db4objects.Db4o.YapConst.ID_LENGTH);
		}

		public override void ReadVariablePart(Db4objects.Db4o.YapFile file)
		{
			if (_configBlock._bootRecordID <= 0)
			{
				return;
			}
			file.ShowInternalClasses(true);
			object bootRecord = file.GetByID1(file.GetSystemTransaction(), _configBlock._bootRecordID
				);
			file.ShowInternalClasses(false);
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

		public override void InitNew(Db4objects.Db4o.YapFile file)
		{
			_configBlock = Db4objects.Db4o.YapConfigBlock.ForNewFile(file);
			InitBootRecord(file);
		}

		private void InitBootRecord(Db4objects.Db4o.YapFile file)
		{
			file.ShowInternalClasses(true);
			_bootRecord = new Db4objects.Db4o.PBootRecord();
			file.SetInternal(file.GetSystemTransaction(), _bootRecord, false);
			_configBlock._bootRecordID = file.GetID1(file.GetSystemTransaction(), _bootRecord
				);
			WriteVariablePart(file, 1);
			file.ShowInternalClasses(false);
		}

		public override Db4objects.Db4o.Transaction InterruptedTransaction()
		{
			return _configBlock.GetTransactionToCommit();
		}

		public override void WriteTransactionPointer(Db4objects.Db4o.Transaction systemTransaction
			, int transactionAddress)
		{
			WriteTransactionPointer(systemTransaction, transactionAddress, _configBlock.Address
				(), Db4objects.Db4o.YapConfigBlock.TRANSACTION_OFFSET);
		}

		public virtual Db4objects.Db4o.MetaIndex GetUUIDMetaIndex()
		{
			return _bootRecord.GetUUIDMetaIndex();
		}

		public override int Length()
		{
			return LENGTH;
		}

		public override void WriteFixedPart(Db4objects.Db4o.YapFile file, bool shuttingDown
			, Db4objects.Db4o.YapWriter writer, int blockSize_, int freespaceID)
		{
			writer.Append(Db4objects.Db4o.YapConst.YAPFILEVERSION);
			writer.Append((byte)blockSize_);
			writer.WriteInt(_configBlock.Address());
			writer.WriteInt((int)TimeToWrite(_configBlock.OpenTime(), shuttingDown));
			writer.WriteInt(file.SystemData().ClassCollectionID());
			writer.WriteInt(freespaceID);
			if (Db4objects.Db4o.Debug.xbytes && Db4objects.Db4o.Deploy.overwrite)
			{
				writer.SetID(Db4objects.Db4o.YapConst.IGNORE_ID);
			}
			writer.Write();
		}

		public override void WriteVariablePart(Db4objects.Db4o.YapFile file, int part)
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
