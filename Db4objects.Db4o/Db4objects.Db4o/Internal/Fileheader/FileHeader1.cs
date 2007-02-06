namespace Db4objects.Db4o.Internal.Fileheader
{
	/// <exclude></exclude>
	public class FileHeader1 : Db4objects.Db4o.Internal.Fileheader.FileHeader
	{
		private static readonly byte[] SIGNATURE = { (byte)'d', (byte)'b', (byte)'4', (byte
			)'o' };

		private static byte VERSION = 1;

		private static readonly int HEADER_LOCK_OFFSET = SIGNATURE.Length + 1;

		private static readonly int OPEN_TIME_OFFSET = HEADER_LOCK_OFFSET + Db4objects.Db4o.Internal.Const4
			.INT_LENGTH;

		private static readonly int ACCESS_TIME_OFFSET = OPEN_TIME_OFFSET + Db4objects.Db4o.Internal.Const4
			.LONG_LENGTH;

		private static readonly int TRANSACTION_POINTER_OFFSET = ACCESS_TIME_OFFSET + Db4objects.Db4o.Internal.Const4
			.LONG_LENGTH;

		internal static readonly int LENGTH = TRANSACTION_POINTER_OFFSET + (Db4objects.Db4o.Internal.Const4
			.INT_LENGTH * 6);

		private Db4objects.Db4o.Internal.Fileheader.TimerFileLock _timerFileLock;

		private Db4objects.Db4o.Internal.Transaction _interruptedTransaction;

		private Db4objects.Db4o.Internal.Fileheader.FileHeaderVariablePart1 _variablePart;

		public override void Close()
		{
			_timerFileLock.Close();
		}

		public override void InitNew(Db4objects.Db4o.Internal.LocalObjectContainer file)
		{
			CommonTasksForNewAndRead(file);
			_variablePart = new Db4objects.Db4o.Internal.Fileheader.FileHeaderVariablePart1(0
				, file.SystemData());
			WriteVariablePart(file, 0);
		}

		protected override Db4objects.Db4o.Internal.Fileheader.FileHeader NewOnSignatureMatch
			(Db4objects.Db4o.Internal.LocalObjectContainer file, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			if (SignatureMatches(reader, SIGNATURE, VERSION))
			{
				return new Db4objects.Db4o.Internal.Fileheader.FileHeader1();
			}
			return null;
		}

		private void NewTimerFileLock(Db4objects.Db4o.Internal.LocalObjectContainer file)
		{
			_timerFileLock = Db4objects.Db4o.Internal.Fileheader.TimerFileLock.ForFile(file);
		}

		public override Db4objects.Db4o.Internal.Transaction InterruptedTransaction()
		{
			return _interruptedTransaction;
		}

		public override int Length()
		{
			return LENGTH;
		}

		protected override void ReadFixedPart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, Db4objects.Db4o.Internal.Buffer reader)
		{
			CommonTasksForNewAndRead(file);
			reader.Seek(TRANSACTION_POINTER_OFFSET);
			_interruptedTransaction = Db4objects.Db4o.Internal.Transaction.ReadInterruptedTransaction
				(file, reader);
			file.BlockSizeReadFromFile(reader.ReadInt());
			ReadClassCollectionAndFreeSpace(file, reader);
			_variablePart = new Db4objects.Db4o.Internal.Fileheader.FileHeaderVariablePart1(reader
				.ReadInt(), file.SystemData());
		}

		private void CommonTasksForNewAndRead(Db4objects.Db4o.Internal.LocalObjectContainer
			 file)
		{
			NewTimerFileLock(file);
			file.i_handlers.OldEncryptionOff();
		}

		public override void ReadVariablePart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file)
		{
			_variablePart.Read(file.GetSystemTransaction());
		}

		public override void WriteFixedPart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, bool shuttingDown, Db4objects.Db4o.Internal.StatefulBuffer writer, int blockSize
			, int freespaceID)
		{
			writer.Append(SIGNATURE);
			writer.Append(VERSION);
			writer.WriteInt((int)TimeToWrite(_timerFileLock.OpenTime(), shuttingDown));
			writer.WriteLong(TimeToWrite(_timerFileLock.OpenTime(), shuttingDown));
			writer.WriteLong(TimeToWrite(Sharpen.Runtime.CurrentTimeMillis(), shuttingDown));
			writer.WriteInt(0);
			writer.WriteInt(0);
			writer.WriteInt(blockSize);
			writer.WriteInt(file.SystemData().ClassCollectionID());
			writer.WriteInt(freespaceID);
			writer.WriteInt(_variablePart.GetID());
			writer.NoXByteCheck();
			writer.Write();
		}

		public override void WriteTransactionPointer(Db4objects.Db4o.Internal.Transaction
			 systemTransaction, int transactionAddress)
		{
			WriteTransactionPointer(systemTransaction, transactionAddress, 0, TRANSACTION_POINTER_OFFSET
				);
		}

		public override void WriteVariablePart(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, int part)
		{
			_variablePart.SetStateDirty();
			_variablePart.Write(file.GetSystemTransaction());
		}
	}
}
