namespace Db4objects.Db4o.IO
{
	/// <summary>Bounded handle into an IoAdapter: Can only access a restricted area.</summary>
	/// <remarks>Bounded handle into an IoAdapter: Can only access a restricted area.</remarks>
	public class IoAdapterWindow
	{
		private Db4objects.Db4o.IO.IoAdapter _io;

		private int _blockOff;

		private int _len;

		private bool _disabled;

		public IoAdapterWindow(Db4objects.Db4o.IO.IoAdapter io, int blockOff, int len)
		{
			_io = io;
			_blockOff = blockOff;
			_len = len;
			_disabled = false;
		}

		/// <returns>Size of this I/O adapter window in bytes.</returns>
		public virtual int Length()
		{
			return _len;
		}

		/// <param name="off">Offset in bytes relative to the window start</param>
		/// <param name="data">Data to write into the window starting from the given offset</param>
		public virtual void Write(int off, byte[] data)
		{
			CheckBounds(off, data);
			_io.BlockSeek(_blockOff + off);
			_io.Write(data);
		}

		/// <param name="off">Offset in bytes relative to the window start</param>
		/// <param name="data">Data buffer to read from the window starting from the given offset
		/// 	</param>
		public virtual int Read(int off, byte[] data)
		{
			CheckBounds(off, data);
			_io.BlockSeek(_blockOff + off);
			return _io.Read(data);
		}

		public virtual void Disable()
		{
			_disabled = true;
		}

		public virtual void Flush()
		{
			if (!_disabled)
			{
				_io.Sync();
			}
		}

		private void CheckBounds(int off, byte[] data)
		{
			if (_disabled)
			{
				throw new System.InvalidOperationException();
			}
			if (data == null || off < 0 || off + data.Length > _len)
			{
				throw new System.ArgumentException();
			}
		}
	}
}
