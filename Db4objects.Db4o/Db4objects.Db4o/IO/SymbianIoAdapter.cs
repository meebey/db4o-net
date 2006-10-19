namespace Db4objects.Db4o.IO
{
	/// <summary>
	/// Workaround for two I/O bugs in Symbian JDK versions:<br />
	/// - seek() cannot move beyond the current file length.<br />
	/// Fix: Write padding bytes up to the seek target if necessary<br />
	/// - Under certain (rare) conditions, calls to RAF.length() seems
	/// to garble up following reads.<br />
	/// Fix: Use a second RAF handle to the file for length() calls
	/// only.<br /><br />
	/// <b>Usage:</b><br />
	/// Db4o.configure().io(new com.db4o.io.SymbianIoAdapter())<br /><br />
	/// TODO:<br />
	/// - BasicClusterTest C/S fails (in AllTests context only)
	/// </summary>
	public class SymbianIoAdapter : Db4objects.Db4o.IO.RandomAccessFileAdapter
	{
		private byte[] _seekBytes = new byte[500];

		private string _path;

		private long _pos;

		private long _length;

		protected SymbianIoAdapter(string path, bool lockFile, long initialLength) : base
			(path, lockFile, initialLength)
		{
			_path = path;
			_pos = 0;
			SetLength();
		}

		private void SetLength()
		{
			_length = RetrieveLength();
		}

		private long RetrieveLength()
		{
			Sharpen.IO.RandomAccessFile file = new Sharpen.IO.RandomAccessFile(_path, "r");
			try
			{
				return file.Length();
			}
			finally
			{
				file.Close();
			}
		}

		public SymbianIoAdapter() : base()
		{
		}

		public override Db4objects.Db4o.IO.IoAdapter Open(string path, bool lockFile, long
			 initialLength)
		{
			return new Db4objects.Db4o.IO.SymbianIoAdapter(path, lockFile, initialLength);
		}

		public override long GetLength()
		{
			SetLength();
			return _length;
		}

		public override int Read(byte[] bytes, int length)
		{
			int ret = base.Read(bytes, length);
			_pos += ret;
			return ret;
		}

		public override void Write(byte[] buffer, int length)
		{
			base.Write(buffer, length);
			_pos += length;
			if (_pos > _length)
			{
				SetLength();
			}
		}

		public override void Seek(long pos)
		{
			if (pos > _length)
			{
				SetLength();
			}
			if (pos > _length)
			{
				int len = (int)(pos - _length);
				base.Seek(_length);
				_pos = _length;
				if (len < 500)
				{
					Write(_seekBytes, len);
				}
				else
				{
					Write(new byte[len]);
				}
			}
			base.Seek(pos);
			_pos = pos;
		}
	}
}
