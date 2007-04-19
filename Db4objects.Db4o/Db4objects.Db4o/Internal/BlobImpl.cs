using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Types;
using Sharpen;
using Sharpen.IO;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal
{
	/// <summary>
	/// Transfer of blobs to and from the db4o system,
	/// if users use the Blob Db4oType.
	/// </summary>
	/// <remarks>
	/// Transfer of blobs to and from the db4o system,
	/// if users use the Blob Db4oType.
	/// </remarks>
	/// <moveto>com.db4o.internal.blobs</moveto>
	/// <exclude></exclude>
	public class BlobImpl : IBlob, Sharpen.Lang.ICloneable, IDb4oTypeImpl
	{
		public const int COPYBUFFER_LENGTH = 4096;

		public string fileName;

		public string i_ext;

		[System.NonSerialized]
		private Sharpen.IO.File i_file;

		[System.NonSerialized]
		private IBlobStatus i_getStatusFrom;

		public int i_length;

		[System.NonSerialized]
		private double i_status = Status.UNUSED;

		[System.NonSerialized]
		private ObjectContainerBase i_stream;

		[System.NonSerialized]
		private Transaction i_trans;

		public virtual int AdjustReadDepth(int a_depth)
		{
			return 1;
		}

		public virtual bool CanBind()
		{
			return true;
		}

		private string CheckExt(Sharpen.IO.File file)
		{
			string name = file.GetName();
			int pos = name.LastIndexOf(".");
			if (pos > 0)
			{
				i_ext = Sharpen.Runtime.Substring(name, pos);
				return Sharpen.Runtime.Substring(name, 0, pos);
			}
			i_ext = string.Empty;
			return name;
		}

		private void Copy(Sharpen.IO.File from, Sharpen.IO.File to)
		{
			to.Delete();
			BufferedInputStream @in = new BufferedInputStream(new FileInputStream(from));
			try
			{
				BufferedOutputStream @out = new BufferedOutputStream(new FileOutputStream(to));
				try
				{
					byte[] buffer = new byte[COPYBUFFER_LENGTH];
					int bytesread = -1;
					while ((bytesread = @in.Read(buffer)) >= 0)
					{
						@out.Write(buffer, 0, bytesread);
					}
					@out.Flush();
				}
				finally
				{
					@out.Close();
				}
			}
			finally
			{
				@in.Close();
			}
		}

		public virtual object CreateDefault(Transaction a_trans)
		{
			BlobImpl bi = null;
			try
			{
				bi = (BlobImpl)this.MemberwiseClone();
				bi.SetTrans(a_trans);
			}
			catch (CloneNotSupportedException)
			{
				return null;
			}
			return bi;
		}

		public virtual FileInputStream GetClientInputStream()
		{
			return new FileInputStream(i_file);
		}

		public virtual FileOutputStream GetClientOutputStream()
		{
			return new FileOutputStream(i_file);
		}

		public virtual string GetFileName()
		{
			return fileName;
		}

		public virtual int GetLength()
		{
			return i_length;
		}

		public virtual double GetStatus()
		{
			if (i_status == Status.PROCESSING && i_getStatusFrom != null)
			{
				return i_getStatusFrom.GetStatus();
			}
			if (i_status == Status.UNUSED)
			{
				if (i_length > 0)
				{
					i_status = Status.AVAILABLE;
				}
			}
			return i_status;
		}

		public virtual void GetStatusFrom(IBlobStatus from)
		{
			i_getStatusFrom = from;
		}

		public virtual bool HasClassIndex()
		{
			return false;
		}

		public virtual void ReadFrom(Sharpen.IO.File file)
		{
			if (!file.Exists())
			{
				throw new IOException(Messages.Get(41, file.GetAbsolutePath()));
			}
			i_length = (int)file.Length();
			CheckExt(file);
			if (i_stream.IsClient())
			{
				i_file = file;
				((IBlobTransport)i_stream).ReadBlobFrom(i_trans, this, file);
			}
			else
			{
				ReadLocal(file);
			}
		}

		public virtual void ReadLocal(Sharpen.IO.File file)
		{
			bool copied = false;
			if (fileName == null)
			{
				Sharpen.IO.File newFile = new Sharpen.IO.File(ServerPath(), file.GetName());
				if (!newFile.Exists())
				{
					Copy(file, newFile);
					copied = true;
					fileName = newFile.GetName();
				}
			}
			if (!copied)
			{
				Copy(file, ServerFile(CheckExt(file), true));
			}
			lock (i_stream.i_lock)
			{
				i_stream.SetInternal(i_trans, this, false);
			}
			i_status = Status.COMPLETED;
		}

		public virtual void PreDeactivate()
		{
		}

		public virtual Sharpen.IO.File ServerFile(string promptName, bool writeToServer)
		{
			lock (i_stream.i_lock)
			{
				i_stream.Activate1(i_trans, this, 2);
			}
			string path = ServerPath();
			i_stream.ConfigImpl().EnsureDirExists(path);
			if (writeToServer)
			{
				if (fileName == null)
				{
					if (promptName != null)
					{
						fileName = promptName;
					}
					else
					{
						fileName = "b_" + Runtime.CurrentTimeMillis();
					}
					string tryPath = fileName + i_ext;
					int i = 0;
					while (new Sharpen.IO.File(path, tryPath).Exists())
					{
						tryPath = fileName + "_" + i++ + i_ext;
						if (i == 99)
						{
							i_status = Status.ERROR;
							throw new IOException(Messages.Get(40));
						}
					}
					fileName = tryPath;
					lock (i_stream.i_lock)
					{
						i_stream.SetInternal(i_trans, this, false);
					}
				}
			}
			else
			{
				if (fileName == null)
				{
					throw new IOException(Messages.Get(38));
				}
			}
			string lastTryPath = path + Sharpen.IO.File.separator + fileName;
			if (!writeToServer)
			{
				if (!(new Sharpen.IO.File(lastTryPath).Exists()))
				{
					throw new IOException(Messages.Get(39));
				}
			}
			return new Sharpen.IO.File(lastTryPath);
		}

		private string ServerPath()
		{
			string path = i_stream.ConfigImpl().BlobPath();
			if (path == null)
			{
				path = "blobs";
			}
			i_stream.ConfigImpl().EnsureDirExists(path);
			return path;
		}

		public virtual void SetStatus(double status)
		{
			i_status = status;
		}

		public virtual void SetTrans(Transaction a_trans)
		{
			i_trans = a_trans;
			i_stream = a_trans.Stream();
		}

		public virtual void WriteLocal(Sharpen.IO.File file)
		{
			Copy(ServerFile(null, false), file);
			i_status = Status.COMPLETED;
		}

		public virtual void WriteTo(Sharpen.IO.File file)
		{
			if (GetStatus() == Status.UNUSED)
			{
				throw new IOException(Messages.Get(43));
			}
			if (i_stream.IsClient())
			{
				i_file = file;
				i_status = Status.QUEUED;
				((IBlobTransport)i_stream).WriteBlobTo(i_trans, this, file);
			}
			else
			{
				WriteLocal(file);
			}
		}

		public virtual void ReplicateFrom(object obj)
		{
		}

		public virtual object StoredTo(Transaction a_trans)
		{
			return this;
		}

		public virtual void SetObjectReference(ObjectReference a_yapObject)
		{
		}
	}
}
