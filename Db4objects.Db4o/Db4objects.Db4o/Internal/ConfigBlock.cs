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

		private const int MINIMUM_LENGTH = Const4.INT_LENGTH + (Const4.LONG_LENGTH * 2) +
			 1;

		internal const int OPEN_TIME_OFFSET = Const4.INT_LENGTH;

		public const int ACCESS_TIME_OFFSET = OPEN_TIME_OFFSET + Const4.LONG_LENGTH;

		public const int TRANSACTION_OFFSET = MINIMUM_LENGTH;

		private const int BOOTRECORD_OFFSET = TRANSACTION_OFFSET + Const4.INT_LENGTH * 2;

		private const int INT_FORMERLY_KNOWN_AS_BLOCK_OFFSET = BOOTRECORD_OFFSET + Const4
			.INT_LENGTH;

		private const int ENCRYPTION_PASSWORD_LENGTH = 5;

		private const int PASSWORD_OFFSET = INT_FORMERLY_KNOWN_AS_BLOCK_OFFSET + ENCRYPTION_PASSWORD_LENGTH;

		private const int FREESPACE_SYSTEM_OFFSET = PASSWORD_OFFSET + 1;

		private const int FREESPACE_ADDRESS_OFFSET = FREESPACE_SYSTEM_OFFSET + Const4.INT_LENGTH;

		private const int CONVERTER_VERSION_OFFSET = FREESPACE_ADDRESS_OFFSET + Const4.INT_LENGTH;

		private const int UUID_INDEX_ID_OFFSET = CONVERTER_VERSION_OFFSET + Const4.INT_LENGTH;

		private const int LENGTH = MINIMUM_LENGTH + (Const4.INT_LENGTH * 7) + ENCRYPTION_PASSWORD_LENGTH
			 + 1;

		/// <exception cref="Db4oIOException"></exception>
		public static Db4objects.Db4o.Internal.ConfigBlock ForNewFile(LocalObjectContainer
			 file)
		{
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
			byte[] pwdtoken = new byte[ENCRYPTION_PASSWORD_LENGTH];
			string fullpwd = ConfigImpl().Password();
			if (ConfigImpl().Encrypt() && fullpwd != null)
			{
				try
				{
					byte[] pwdbytes = new LatinStringIO().Write(fullpwd);
					BufferImpl encwriter = new StatefulBuffer(_container.Transaction(), pwdbytes.Length
						 + ENCRYPTION_PASSWORD_LENGTH);
					encwriter.Append(pwdbytes);
					encwriter.Append(new byte[ENCRYPTION_PASSWORD_LENGTH]);
					_container._handlers.Decrypt(encwriter);
					System.Array.Copy(encwriter._buffer, 0, pwdtoken, 0, ENCRYPTION_PASSWORD_LENGTH);
				}
				catch (Exception exc)
				{
					Sharpen.Runtime.PrintStackTrace(exc);
				}
			}
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
				, LENGTH);
			_container.ReadBytes(reader._buffer, _address, LENGTH);
			int oldLength = reader.ReadInt();
			if (oldLength > LENGTH || oldLength < MINIMUM_LENGTH)
			{
				throw new IncompatibleFileFormatException();
			}
			if (oldLength != LENGTH)
			{
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
			long lastAccessTime = reader.ReadLong();
			SystemData().StringEncoding(reader.ReadByte());
			if (oldLength > TRANSACTION_OFFSET)
			{
				_transactionToCommit = LocalTransaction.ReadInterruptedTransaction(_container, reader
					);
			}
			if (oldLength > BOOTRECORD_OFFSET)
			{
				_bootRecordID = reader.ReadInt();
			}
			if (oldLength > INT_FORMERLY_KNOWN_AS_BLOCK_OFFSET)
			{
				reader.ReadInt();
			}
			if (oldLength > PASSWORD_OFFSET)
			{
				byte[] encpassword = reader.ReadBytes(ENCRYPTION_PASSWORD_LENGTH);
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
			if (oldLength > FREESPACE_SYSTEM_OFFSET)
			{
				SystemData().FreespaceSystem(reader.ReadByte());
			}
			if (oldLength > FREESPACE_ADDRESS_OFFSET)
			{
				SystemData().FreespaceAddress(reader.ReadInt());
			}
			if (oldLength > CONVERTER_VERSION_OFFSET)
			{
				SystemData().ConverterVersion(reader.ReadInt());
			}
			if (oldLength > UUID_INDEX_ID_OFFSET)
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
				_timerFileLock.CheckIfOtherSessionAlive(_container, _address, ACCESS_TIME_OFFSET, 
					lastAccessTime);
			}
			if (_container.NeedsLockFileThread())
			{
				Cool.SleepIgnoringInterruption(100);
				_container.SyncFiles();
				TimerFileLock().CheckOpenTime();
			}
			if (oldLength < LENGTH)
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
			AddressChanged(_container.GetSlot(LENGTH).Address());
			StatefulBuffer writer = _container.GetWriter(_container.Transaction(), _address, 
				LENGTH);
			IntHandler.WriteInt(LENGTH, writer);
			for (int i = 0; i < 2; i++)
			{
				writer.WriteLong(TimerFileLock().OpenTime());
			}
			writer.WriteByte(SystemData().StringEncoding());
			IntHandler.WriteInt(0, writer);
			IntHandler.WriteInt(0, writer);
			IntHandler.WriteInt(_bootRecordID, writer);
			IntHandler.WriteInt(0, writer);
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
			TimerFileLock().SetAddresses(_address, OPEN_TIME_OFFSET, ACCESS_TIME_OFFSET);
		}

		private void WritePointer()
		{
			TimerFileLock().CheckHeaderLock();
			StatefulBuffer writer = _container.GetWriter(_container.Transaction(), 0, Const4.
				ID_LENGTH);
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
