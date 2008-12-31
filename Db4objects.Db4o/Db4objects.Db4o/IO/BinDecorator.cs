/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.IO
{
	/// <summary>Wrapper baseclass for all classes that wrap Bin.</summary>
	/// <remarks>
	/// Wrapper baseclass for all classes that wrap Bin.
	/// Each class that adds functionality to a Bin must
	/// extend this class to allow db4o to access the
	/// delegate instance with
	/// <see cref="#undecorate()">#undecorate()</see>
	/// .
	/// </remarks>
	public class BinDecorator : IBin
	{
		protected readonly IBin _bin;

		/// <summary>Default constructor.</summary>
		/// <remarks>Default constructor.</remarks>
		/// <param name="bin">
		/// the
		/// <see cref="Db4objects.Db4o.IO.IBin">Db4objects.Db4o.IO.IBin</see>
		/// that is to be wrapped.
		/// </param>
		public BinDecorator(IBin bin)
		{
			_bin = bin;
		}

		/// <summary>
		/// closes the BinDecorator and the underlying
		/// <see cref="Db4objects.Db4o.IO.IBin">Db4objects.Db4o.IO.IBin</see>
		/// .
		/// </summary>
		public virtual void Close()
		{
			_bin.Close();
		}

		/// <seealso cref="Db4objects.Db4o.IO.IBin.Length"></seealso>
		public virtual long Length()
		{
			return _bin.Length();
		}

		/// <seealso cref="Db4objects.Db4o.IO.IBin.Read">Db4objects.Db4o.IO.IBin.Read</seealso>
		public virtual int Read(long position, byte[] buffer, int bytesToRead)
		{
			return _bin.Read(position, buffer, bytesToRead);
		}

		/// <seealso cref="Db4objects.Db4o.IO.IBin.Sync">Db4objects.Db4o.IO.IBin.Sync</seealso>
		public virtual void Sync()
		{
			_bin.Sync();
		}

		/// <seealso cref="Db4objects.Db4o.IO.IBin.SyncRead">Db4objects.Db4o.IO.IBin.SyncRead
		/// 	</seealso>
		public virtual int SyncRead(long position, byte[] bytes, int bytesToRead)
		{
			return _bin.SyncRead(position, bytes, bytesToRead);
		}

		/// <seealso cref="Db4objects.Db4o.IO.IBin.Write">Db4objects.Db4o.IO.IBin.Write</seealso>
		public virtual void Write(long position, byte[] bytes, int bytesToWrite)
		{
			_bin.Write(position, bytes, bytesToWrite);
		}
	}
}
