/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Internal.IX
{
	[System.Serializable]
	public class IxException : Exception
	{
		private int _address;

		private int _addressOffset;

		public IxException(int address, int addressOffset) : this(null, null, address, addressOffset
			)
		{
		}

		public IxException(string msg, int address, int addressOffset) : this(msg, null, 
			address, addressOffset)
		{
		}

		public IxException(Exception cause, int address, int addressOffset) : this(null, 
			cause, address, addressOffset)
		{
		}

		public IxException(string msg, Exception cause, int address, int addressOffset) : 
			base(EnhancedMessage(msg, address, addressOffset), cause)
		{
			_address = address;
			_addressOffset = addressOffset;
		}

		public virtual int Address()
		{
			return _address;
		}

		public virtual int AddressOffset()
		{
			return _addressOffset;
		}

		private static string EnhancedMessage(string msg, int address, int addressOffset)
		{
			string enhancedMessage = "Ix " + address + "," + addressOffset;
			if (msg != null)
			{
				enhancedMessage += ": " + msg;
			}
			return enhancedMessage;
		}
	}
}
