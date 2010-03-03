/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fileheader;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class FileHeader0 : FileHeader
	{
		internal const int HeaderLength = 2 + (Const4.IntLength * 4);

		private ConfigBlock _configBlock;

		private PBootRecord _bootRecord;

		private readonly LocalObjectContainer _container;

		public FileHeader0(LocalObjectContainer container)
		{
			// The header format is:
			// Old format
			// -------------------------
			// {
			// Y
			// [Rest]
			// New format
			// -------------------------
			// (byte)4
			// block size in bytes 1 to 127
			// [Rest]
			// Rest (only ints)
			// -------------------
			// address of the extended configuration block, see YapConfigBlock
			// headerLock
			// YapClassCollection ID
			// FreeBySize ID
			_container = container;
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override void Close()
		{
			_configBlock.Close();
		}

		protected override FileHeader NewOnSignatureMatch(LocalObjectContainer file, ByteArrayBuffer
			 reader)
		{
			byte firstFileByte = reader.ReadByte();
			if (firstFileByte != Const4.Yapbegin)
			{
				if (firstFileByte != Const4.Yapfileversion)
				{
					return null;
				}
				file.BlockSizeReadFromFile(reader.ReadByte());
			}
			else
			{
				if (reader.ReadByte() != Const4.Yapfile)
				{
					return null;
				}
			}
			return new Db4objects.Db4o.Internal.Fileheader.FileHeader0(file);
		}

		/// <exception cref="Db4objects.Db4o.Ext.OldFormatException"></exception>
		protected override void Read(LocalObjectContainer file, ByteArrayBuffer reader)
		{
			_configBlock = ConfigBlock.ForExistingFile(file, reader.ReadInt());
			SkipConfigurationLockTime(reader);
			ReadClassCollectionAndFreeSpace(file, reader);
		}

		private void SkipConfigurationLockTime(ByteArrayBuffer reader)
		{
			reader.IncrementOffset(Const4.IdLength);
		}

		private object GetBootRecord(LocalObjectContainer file)
		{
			file.ShowInternalClasses(true);
			try
			{
				return file.GetByID(file.SystemTransaction(), _configBlock._bootRecordID);
			}
			finally
			{
				file.ShowInternalClasses(false);
			}
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public override void InitNew(LocalObjectContainer file)
		{
			_configBlock = ConfigBlock.ForNewFile(file);
			InitBootRecord(file);
		}

		private void InitBootRecord(LocalObjectContainer file)
		{
			file.ShowInternalClasses(true);
			try
			{
				_bootRecord = new PBootRecord();
				file.StoreInternal(file.SystemTransaction(), _bootRecord, false);
				_configBlock._bootRecordID = file.GetID(file.SystemTransaction(), _bootRecord);
				WriteVariablePart(file, 1);
			}
			finally
			{
				file.ShowInternalClasses(false);
			}
		}

		public override void CompleteInterruptedTransaction(LocalObjectContainer container
			)
		{
			_configBlock.CompleteInterruptedTransaction();
		}

		public override void WriteTransactionPointer(Transaction systemTransaction, int transactionPointer1
			, int transactionPointer2)
		{
			WriteTransactionPointer(systemTransaction, transactionPointer1, transactionPointer2
				, _configBlock.Address(), ConfigBlock.TransactionOffset);
		}

		public virtual MetaIndex GetUUIDMetaIndex()
		{
			return _bootRecord.GetUUIDMetaIndex();
		}

		public override int Length()
		{
			return HeaderLength;
		}

		public override void WriteFixedPart(LocalObjectContainer file, bool startFileLockingThread
			, bool shuttingDown, StatefulBuffer writer, int blockSize_, int freespaceID)
		{
			writer.WriteByte(Const4.Yapfileversion);
			writer.WriteByte((byte)blockSize_);
			writer.WriteInt(_configBlock.Address());
			writer.WriteInt((int)TimeToWrite(_configBlock.OpenTime(), shuttingDown));
			writer.WriteInt(file.SystemData().ClassCollectionID());
			writer.WriteInt(freespaceID);
			writer.Write();
			file.SyncFiles();
		}

		public override void WriteVariablePart(LocalObjectContainer file, int part)
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

		public override void ReadIdentity(LocalObjectContainer container)
		{
			if (_configBlock._bootRecordID <= 0)
			{
				return;
			}
			object bootRecord = Debug4.readBootRecord ? GetBootRecord(container) : null;
			if (!(bootRecord is PBootRecord))
			{
				InitBootRecord(container);
				container.GenerateNewIdentity();
				return;
			}
			_bootRecord = (PBootRecord)bootRecord;
			container.Activate(bootRecord, int.MaxValue);
			container.SetNextTimeStampId(_bootRecord.i_versionGenerator);
			container.SystemData().Identity(_bootRecord.i_db);
		}

		public override IRunnable Commit()
		{
			WriteVariablePart(_container, 2);
			return Runnable4.DoNothing;
		}

		public override FileHeader Convert(LocalObjectContainer file)
		{
			return this;
		}
	}
}
