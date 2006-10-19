namespace Db4objects.Db4o
{
	/// <summary>
	/// Transfer of blobs to and from the db4o system,
	/// if users use the Blob Db4oType.
	/// </summary>
	/// <remarks>
	/// Transfer of blobs to and from the db4o system,
	/// if users use the Blob Db4oType.
	/// </remarks>
	/// <exclude></exclude>
	public class BlobImpl : Db4objects.Db4o.Types.IBlob, Sharpen.Lang.ICloneable, Db4objects.Db4o.IDb4oTypeImpl
	{
		internal const int COPYBUFFER_LENGTH = 4096;

		public string fileName;

		public string i_ext;

		[Db4objects.Db4o.Transient]
		private Sharpen.IO.File i_file;

		[Db4objects.Db4o.Transient]
		private Db4objects.Db4o.MsgBlob i_getStatusFrom;

		public int i_length;

		[Db4objects.Db4o.Transient]
		private double i_status = Db4objects.Db4o.Ext.Status.UNUSED;

		[Db4objects.Db4o.Transient]
		private Db4objects.Db4o.YapStream i_stream;

		[Db4objects.Db4o.Transient]
		private Db4objects.Db4o.Transaction i_trans;

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
			i_ext = "";
			return name;
		}

		private void Copy(Sharpen.IO.File from, Sharpen.IO.File to)
		{
			to.Delete();
			Sharpen.IO.BufferedInputStream @in = new Sharpen.IO.BufferedInputStream(new Sharpen.IO.FileInputStream
				(from));
			Sharpen.IO.BufferedOutputStream @out = new Sharpen.IO.BufferedOutputStream(new Sharpen.IO.FileOutputStream
				(to));
			byte[] buffer = new byte[COPYBUFFER_LENGTH];
			int bytesread = -1;
			while ((bytesread = @in.Read(buffer)) >= 0)
			{
				@out.Write(buffer, 0, bytesread);
			}
			@out.Flush();
			@out.Close();
			@in.Close();
		}

		public virtual object CreateDefault(Db4objects.Db4o.Transaction a_trans)
		{
			Db4objects.Db4o.BlobImpl bi = null;
			try
			{
				bi = (Db4objects.Db4o.BlobImpl)this.MemberwiseClone();
				bi.SetTrans(a_trans);
			}
			catch (Sharpen.Lang.CloneNotSupportedException e)
			{
				return null;
			}
			return bi;
		}

		internal virtual Sharpen.IO.FileInputStream GetClientInputStream()
		{
			return new Sharpen.IO.FileInputStream(i_file);
		}

		internal virtual Sharpen.IO.FileOutputStream GetClientOutputStream()
		{
			return new Sharpen.IO.FileOutputStream(i_file);
		}

		public virtual string GetFileName()
		{
			return fileName;
		}

		internal virtual int GetLength()
		{
			return i_length;
		}

		public virtual double GetStatus()
		{
			if (i_status == Db4objects.Db4o.Ext.Status.PROCESSING && i_getStatusFrom != null)
			{
				return i_getStatusFrom.GetStatus();
			}
			if (i_status == Db4objects.Db4o.Ext.Status.UNUSED)
			{
				if (i_length > 0)
				{
					i_status = Db4objects.Db4o.Ext.Status.AVAILABLE;
				}
			}
			return i_status;
		}

		internal virtual void GetStatusFrom(Db4objects.Db4o.MsgBlob from)
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
				throw new System.IO.IOException(Db4objects.Db4o.Messages.Get(41, file.GetAbsolutePath
					()));
			}
			i_length = (int)file.Length();
			CheckExt(file);
			if (i_stream.IsClient())
			{
				i_file = file;
				Db4objects.Db4o.MsgBlob msg = null;
				lock (i_stream.Lock())
				{
					i_stream.Set(this);
					int id = (int)i_stream.GetID(this);
					msg = (Db4objects.Db4o.MsgBlob)Db4objects.Db4o.Msg.WRITE_BLOB.GetWriterForInt(i_trans
						, id);
					msg._blob = this;
					i_status = Db4objects.Db4o.Ext.Status.QUEUED;
				}
				((Db4objects.Db4o.YapClient)i_stream).ProcessBlobMessage(msg);
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
			i_status = Db4objects.Db4o.Ext.Status.COMPLETED;
		}

		public virtual void PreDeactivate()
		{
		}

		internal virtual Sharpen.IO.File ServerFile(string promptName, bool writeToServer
			)
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
						fileName = "b_" + Sharpen.Runtime.CurrentTimeMillis();
					}
					string tryPath = fileName + i_ext;
					int i = 0;
					while (new Sharpen.IO.File(path, tryPath).Exists())
					{
						tryPath = fileName + "_" + i++ + i_ext;
						if (i == 99)
						{
							i_status = Db4objects.Db4o.Ext.Status.ERROR;
							throw new System.IO.IOException(Db4objects.Db4o.Messages.Get(40));
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
					throw new System.IO.IOException(Db4objects.Db4o.Messages.Get(38));
				}
			}
			string lastTryPath = path + Sharpen.IO.File.separator + fileName;
			if (!writeToServer)
			{
				if (!(new Sharpen.IO.File(lastTryPath).Exists()))
				{
					throw new System.IO.IOException(Db4objects.Db4o.Messages.Get(39));
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

		internal virtual void SetStatus(double status)
		{
			i_status = status;
		}

		public virtual void SetTrans(Db4objects.Db4o.Transaction a_trans)
		{
			i_trans = a_trans;
			i_stream = a_trans.Stream();
		}

		public virtual void WriteLocal(Sharpen.IO.File file)
		{
			Copy(ServerFile(null, false), file);
			i_status = Db4objects.Db4o.Ext.Status.COMPLETED;
		}

		public virtual void WriteTo(Sharpen.IO.File file)
		{
			if (GetStatus() == Db4objects.Db4o.Ext.Status.UNUSED)
			{
				throw new System.IO.IOException(Db4objects.Db4o.Messages.Get(43));
			}
			if (i_stream.IsClient())
			{
				i_file = file;
				Db4objects.Db4o.MsgBlob msg = (Db4objects.Db4o.MsgBlob)Db4objects.Db4o.Msg.READ_BLOB
					.GetWriterForInt(i_trans, (int)i_stream.GetID(this));
				msg._blob = this;
				i_status = Db4objects.Db4o.Ext.Status.QUEUED;
				((Db4objects.Db4o.YapClient)i_stream).ProcessBlobMessage(msg);
			}
			else
			{
				WriteLocal(file);
			}
		}

		public virtual void ReplicateFrom(object obj)
		{
		}

		public virtual object StoredTo(Db4objects.Db4o.Transaction a_trans)
		{
			return this;
		}

		public virtual void SetYapObject(Db4objects.Db4o.YapObject a_yapObject)
		{
		}
	}
}
