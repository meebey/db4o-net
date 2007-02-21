/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.IO;
using Db4objects.Db4o;

namespace Sharpen.IO
{
	public class File
	{
		public static readonly char separatorChar = Path.DirectorySeparatorChar;
		public static readonly string separator = ""+Path.DirectorySeparatorChar;
		
		private string _path;

		public File(string path)
		{
			_path = path;
		}

		public File(string dir, string file)
		{
			if (dir == null)
			{
				_path = file;
			}
			else
			{
				_path = Path.Combine(dir, file);
			}
		}

		public virtual bool Delete()
		{
			if (Exists())
			{
				System.IO.File.Delete(_path);
				return !Exists();
			}
			return false;
		}

		public bool Exists()
		{
			return System.IO.File.Exists(_path) || Directory.Exists(_path);
		}

		public string GetCanonicalPath()
		{
			return _path;
		}

		public string GetAbsolutePath()
		{
			return _path;
		}

		public string GetName()
		{
			int index = _path.LastIndexOf(separator);
			return _path.Substring(index + 1);
		}

		public string GetPath()
		{
			return _path;
		}

		public bool IsDirectory()
		{
#if CF_1_0 || CF_2_0
			return System.IO.Directory.Exists(_path);
#else
			return (System.IO.File.GetAttributes(_path) & FileAttributes.Directory) != 0;
#endif
		}

		public long Length()
		{
			return new FileInfo(_path).Length;
		}

		public string[] List()
		{
			return Directory.GetFiles(_path);
		}

		public bool Mkdir()
		{
			if (Exists())
			{
				return false;
			}
			Directory.CreateDirectory(_path);
			return Exists();
		}

		public bool Mkdirs()
		{
			if (Exists())
			{
				return false;
			}
			int pos = _path.LastIndexOf(separator);
			if (pos > 0)
			{
				new File(_path.Substring(0, pos)).Mkdirs();
			}
			return Mkdir();
		}

		public void RenameTo(File file)
		{
			new FileInfo(_path).MoveTo(file.GetPath());
		}
	}
}
