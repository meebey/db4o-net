namespace Db4objects.Db4o.IO.Crypt
{
	/// <summary>
	/// XTeaEncryptionFileAdapter is an encryption IoAdapter plugin for db4o file IO
	/// <br />
	/// that realized XTEA encryption algorithm.
	/// </summary>
	/// <remarks>
	/// XTeaEncryptionFileAdapter is an encryption IoAdapter plugin for db4o file IO
	/// <br />
	/// that realized XTEA encryption algorithm. <br />
	/// <br />
	/// Configure db4o to add this encryption mechanism:<br />
	/// <code>Db4o.configure().io(new XTeaEncryptionFileAdapter("password"));
	/// </code><br />
	/// Any changes must be taken with the same password.<br />
	/// <br />
	/// Remember that any configuration settings must be set before opening
	/// ObjectContainer.
	/// </remarks>
	public class XTeaEncryptionFileAdapter : Db4objects.Db4o.IO.IoAdapter
	{
		private Db4objects.Db4o.IO.IoAdapter _adapter;

		private string _key;

		private Db4objects.Db4o.IO.Crypt.XTEA _xtea;

		private long _pos;

		private Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec _iterat;

		private const bool DEBUG = false;

		public XTeaEncryptionFileAdapter(string password) : this(new Db4objects.Db4o.IO.RandomAccessFileAdapter
			(), password, Db4objects.Db4o.IO.Crypt.XTEA.ITERATIONS32)
		{
		}

		public XTeaEncryptionFileAdapter(string password, Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec
			 iterat) : this(new Db4objects.Db4o.IO.RandomAccessFileAdapter(), password, iterat
			)
		{
		}

		public XTeaEncryptionFileAdapter(Db4objects.Db4o.IO.IoAdapter adapter, string password
			, Db4objects.Db4o.IO.Crypt.XTEA.IterationSpec iterat)
		{
			_adapter = adapter;
			_key = password;
			_iterat = iterat;
		}

		public XTeaEncryptionFileAdapter(Db4objects.Db4o.IO.IoAdapter adapter, string password
			)
		{
			_adapter = adapter;
			_key = password;
			_iterat = Db4objects.Db4o.IO.Crypt.XTEA.ITERATIONS32;
		}

		private XTeaEncryptionFileAdapter(Db4objects.Db4o.IO.IoAdapter adapter, Db4objects.Db4o.IO.Crypt.XTEA
			 xtea)
		{
			_adapter = adapter;
			_xtea = xtea;
		}

		/// <summary>implement to close the adapter</summary>
		public override void Close()
		{
			_adapter.Close();
		}

		public override void Delete(string path)
		{
			_adapter.Delete(path);
		}

		public override bool Exists(string path)
		{
			return _adapter.Exists(path);
		}

		/// <summary>implement to return the absolute length of the file</summary>
		public override long GetLength()
		{
			return _adapter.GetLength();
		}

		/// <summary>implement to open the file</summary>
		public override Db4objects.Db4o.IO.IoAdapter Open(string path, bool lockFile, long
			 initialLength)
		{
			return new Db4objects.Db4o.IO.Crypt.XTeaEncryptionFileAdapter(_adapter.Open(path, 
				lockFile, initialLength), new Db4objects.Db4o.IO.Crypt.XTEA(_key, _iterat));
		}

		/// <summary>implement to read and decrypt a buffer</summary>
		public override int Read(byte[] bytes, int length)
		{
			long origPos = _pos;
			int fullLength = length;
			int prePad = (int)(_pos % 8);
			fullLength += prePad;
			int overhang = fullLength % 8;
			int postPad = (overhang == 0 ? 0 : 8 - (overhang));
			fullLength += postPad;
			byte[] pb = new byte[fullLength];
			if (prePad != 0)
			{
				Seek(_pos - prePad);
			}
			int readResult = _adapter.Read(pb);
			_xtea.Decrypt(pb);
			System.Array.Copy(pb, prePad, bytes, 0, length);
			Seek(origPos + length);
			return readResult;
		}

		/// <summary>implement to set the read/write pointer in the file</summary>
		public override void Seek(long pos)
		{
			_pos = pos;
			_adapter.Seek(pos);
		}

		/// <summary>implement to flush the file contents to storage</summary>
		public override void Sync()
		{
			_adapter.Sync();
		}

		/// <summary>implement to write and encrypt a buffer</summary>
		public override void Write(byte[] buffer, int length)
		{
			long origPos = _pos;
			int fullLength = length;
			int prePad = (int)(_pos % 8);
			fullLength += prePad;
			int overhang = fullLength % 8;
			int postPad = (overhang == 0 ? 0 : 8 - (overhang));
			fullLength += postPad;
			byte[] pb = new byte[fullLength];
			if (prePad != 0)
			{
				Seek(origPos - prePad);
			}
			if (BlockSize() % 8 != 0 || prePad != 0)
			{
				Read(pb);
				Seek(origPos - prePad);
			}
			System.Array.Copy(buffer, 0, pb, prePad, length);
			if (prePad == 0)
			{
			}
			_xtea.Encrypt(pb);
			_adapter.Write(pb, pb.Length);
			Seek(origPos + length);
		}

		private void Log(string msg, byte[] buf)
		{
			System.Console.Out.WriteLine("\n " + msg);
			for (int idx = 0; idx < buf.Length; idx++)
			{
				System.Console.Out.Write(buf[idx] + " ");
			}
		}
	}
}
