/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Fileheader;
using Db4objects.Db4o.Internal.Handlers;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <summary>
	/// configuration and agent to write the configuration block
	/// The configuration block also contains the timer lock and
	/// a pointer to the running transaction.
	/// </summary>
	/// <remarks>
	/// configuration and agent to write the configuration block
	/// The configuration block also contains the timer lock and
	/// a pointer to the running transaction.
	/// </remarks>
	/// <exclude></exclude>
	public sealed class ConfigBlock
	{
		private readonly LocalObjectContainer _container;

		private readonly Db4objects.Db4o.Internal.Fileheader.TimerFileLock _timerFileLock;

		private int _address;

		private Transaction _transactionToCommit;

		public int _bootRecordID;

		private const int MinimumLength = Const4.IntLength + (Const4.LongLength * 2) + 1;

		internal const int OpenTimeOffset = Const4.IntLength;

		public const int AccessTimeOffset = OpenTimeOffset + Const4.LongLength;

		public const int TransactionOffset = MinimumLength;

		private const int BootrecordOffset = TransactionOffset + Const4.IntLength * 2;

		private const int IntFormerlyKnownAsBlockOffset = BootrecordOffset + Const4.IntLength;

		private const int EncryptionPasswordLength = 5;

		private const int PasswordOffset = IntFormerlyKnownAsBlockOffset + EncryptionPasswordLength;

		private const int FreespaceSystemOffset = PasswordOffset + 1;

		private const int FreespaceAddressOffset = FreespaceSystemOffset + Const4.IntLength;

		private const int ConverterVersionOffset = FreespaceAddressOffset + Const4.IntLength;

		private const int UuidIndexIdOffset = ConverterVersionOffset + Const4.IntLength;

		private const int Length = MinimumLength + (Const4.IntLength * 7) + EncryptionPasswordLength
			 + 1;

		/// <exception cref="Db4oIOException"></exception>
		public static Db4objects.Db4o.Internal.ConfigBlock ForNewFile(LocalObjectContainer
			 file)
		{
			// ConfigBlock Format
			// int    length of the config block
			// long   last access time for timer lock
			// long   last access time for timer lock (duplicate for atomicity)
			// byte   unicode or not
			// int    transaction-in-process address
			// int    transaction-in-process address (duplicate for atomicity)
			// int    id of PBootRecord
			// int    unused (and lost)
			// 5 bytes of the encryption password
			// byte   freespace system used
			// int    freespace address
			// int    converter versions
			// own length
			// candidate ID and last access time
			// Unicode byte
			// complete possible data in config block
			// (two transaction pointers, PDB ID, lost int, freespace address, converter_version, index id)
			return new Db4objects.Db4o.Internal.ConfigBlock(file, true, 0);
		}

		/// <exception cref="Db4oIOException"></exception>
		/// <exception cref="OldFormatException"></exception>
		public static Db4objects.Db4o.Internal.ConfigBlock ForExistingFile(LocalObjectContainer
			 file, int address)
		{
			return new Db4objects.Db4o.Internal.ConfigBlock(file, false, address);
		}

		/// <exception cref="Db4oIOException"></exception>
		/// <exception cref="OldFormatException"></exception>
		private ConfigBlock(LocalObjectContainer stream, bool isNew, int address)
		{
			_container = stream;
			_timerFileLock = Db4objects.Db4o.Internal.Fileheader.TimerFileLock.ForFile(stream
				);
			TimerFileLock().WriteHeaderLock();
			if (!isNew)
			{
				Read(address);
			}
			TimerFileLock().Start();
		}

		private Db4objects.Db4o.Internal.Fileheader.TimerFileLock TimerFileLock()
		{
			return _timerFileLock;
		}

		public long OpenTime()
		{
			return TimerFileLock().OpenTime();
		}

		public Transaction GetTransactionToCommit()
		{
			return _transactionToCommit;
		}

		private byte[] PasswordToken()
		{
			byte[] pwdtoken = new byte[EncryptionPasswordLength];
			string fullpwd = ConfigImpl().Password();
			if (ConfigImpl().Encrypt() && fullpwd != null)
			{
				try
				{
					byte[] pwdbytes = new LatinStringIO().Write(fullpwd);
					ByteArrayBuffer encwriter = new StatefulBuffer(_container.Transaction(), pwdbytes
						.Length + EncryptionPasswordLength);
					encwriter.Append(pwdbytes);
					encwriter.Append(new byte[EncryptionPasswordLength]);
					_container._handlers.Decrypt(encwriter);
					System.Array.Copy(encwriter._buffer, 0, pwdtoken, 0, EncryptionPasswordLength);
				}
				catch (Exception exc)
				{
					// should never happen
					//if(Debug.atHome) {
					Sharpen.Runtime.PrintStackTrace(exc);
				}
			}
			//}
			return pwdtoken;
		}

		private Db4objects.Db4o.Internal.SystemData SystemData()
		{
			return _container.SystemData();
		}

		/// <exception cref="Db4oIOException"></exception>
		/// <exception cref="OldFormatException"></exception>
		private void Read(int address)
		{
			AddressChanged(address);
			TimerFileLock().WriteOpenTime();
			StatefulBuffer reader = _container.GetWriter(_container.SystemTransaction(), _address
				, Length);
			_container.ReadBytes(reader._buffer, _address, Length);
			int oldLength = reader.ReadInt();
			if (oldLength > Length || oldLength < MinimumLength)
			{
				throw new IncompatibleFileFormatException();
			}
			if (oldLength != Length)
			{
				// TODO: instead of bailing out, somehow trigger wrapping the stream's io adapter in
				// a readonly decorator, issue a  notification and continue?
				if (!AllowVersionUpdate())
				{
					if (AllowAutomaticShutdown())
					{
						Platform4.RemoveShutDownHook(_container);
					}
					throw new OldFormatException();
				}
			}
			reader.ReadLong();
			// open time 
			long lastAccessTime = reader.ReadLong();
			SystemData().StringEncoding(reader.ReadByte());
			if (oldLength > TransactionOffset)
			{
				_transactionToCommit = LocalTransaction.ReadInterruptedTransaction(_container, reader
					);
			}
			if (oldLength > BootrecordOffset)
			{
				_bootRecordID = reader.ReadInt();
			}
			if (oldLength > IntFormerlyKnownAsBlockOffset)
			{
				// this one is dead.
				// Blocksize is in the very first bytes
				reader.ReadInt();
			}
			if (oldLength > PasswordOffset)
			{
				byte[] encpassword = reader.ReadBytes(EncryptionPasswordLength);
				bool nonZeroByte = false;
				for (int i = 0; i < encpassword.Length; i++)
				{
					if (encpassword[i] != 0)
					{
						nonZeroByte = true;
						break;
					}
				}
				if (!nonZeroByte)
				{
					// no password in the databasefile, work without encryption
					_container._handlers.OldEncryptionOff();
				}
				else
				{
					byte[] storedpwd = PasswordToken();
					for (int idx = 0; idx < storedpwd.Length; idx++)
					{
						if (storedpwd[idx] != encpassword[idx])
						{
							_container.FatalException(54);
						}
					}
				}
			}
			if (oldLength > FreespaceSystemOffset)
			{
				SystemData().FreespaceSystem(reader.ReadByte());
			}
			if (oldLength > FreespaceAddressOffset)
			{
				SystemData().FreespaceAddress(reader.ReadInt());
			}
			if (oldLength > ConverterVersionOffset)
			{
				SystemData().ConverterVersion(reader.ReadInt());
			}
			if (oldLength > UuidIndexIdOffset)
			{
				int uuidIndexId = reader.ReadInt();
				if (0 != uuidIndexId)
				{
					SystemData().UuidIndexId(uuidIndexId);
				}
			}
			_container.EnsureFreespaceSlot();
			if (FileHeader.LockedByOtherSession(_container, lastAccessTime))
			{
				_timerFileLock.CheckIfOtherSessionAlive(_container, _address, AccessTimeOffset, lastAccessTime
					);
			}
			if (_container.NeedsLockFileThread())
			{
				// We give the other process a chance to 
				// write its lock.
				Cool.SleepIgnoringInterruption(100);
				_container.SyncFiles();
				TimerFileLock().CheckOpenTime();
			}
			if (oldLength < Length)
			{
				Write();
			}
		}

		private bool AllowAutomaticShutdown()
		{
			return ConfigImpl().AutomaticShutDown();
		}

		private bool AllowVersionUpdate()
		{
			Config4Impl configImpl = ConfigImpl();
			return !configImpl.IsReadOnly() && configImpl.AllowVersionUpdates();
		}

		private Config4Impl ConfigImpl()
		{
			return _container.ConfigImpl();
		}

		public void Write()
		{
			TimerFileLock().CheckHeaderLock();
			AddressChanged(_container.GetSlot(Length).Address());
			StatefulBuffer writer = _container.GetWriter(_container.Transaction(), _address, 
				Length);
			IntHandler.WriteInt(Length, writer);
			for (int i = 0; i < 2; i++)
			{
				writer.WriteLong(TimerFileLock().OpenTime());
			}
			writer.WriteByte(SystemData().StringEncoding());
			IntHandler.WriteInt(0, writer);
			IntHandler.WriteInt(0, writer);
			IntHandler.WriteInt(_bootRecordID, writer);
			IntHandler.WriteInt(0, writer);
			// dead byte from wrong attempt for blocksize
			writer.Append(PasswordToken());
			writer.WriteByte(SystemData().FreespaceSystem());
			_container.EnsureFreespaceSlot();
			IntHandler.WriteInt(SystemData().FreespaceAddress(), writer);
			IntHandler.WriteInt(SystemData().ConverterVersion(), writer);
			IntHandler.WriteInt(SystemData().UuidIndexId(), writer);
			writer.Write();
			WritePointer();
			_container.SyncFiles();
		}

		private void AddressChanged(int address)
		{
			_address = address;
			TimerFileLock().SetAddresses(_address, OpenTimeOffset, AccessTimeOffset);
		}

		private void WritePointer()
		{
			TimerFileLock().CheckHeaderLock();
			StatefulBuffer writer = _container.GetWriter(_container.Transaction(), 0, Const4.
				IdLength);
			writer.MoveForward(2);
			IntHandler.WriteInt(_address, writer);
			writer.NoXByteCheck();
			writer.Write();
			TimerFileLock().WriteHeaderLock();
		}

		public int Address()
		{
			return _address;
		}

		/// <exception cref="Db4oIOException"></exception>
		public void Close()
		{
			TimerFileLock().Close();
		}
	}
}
