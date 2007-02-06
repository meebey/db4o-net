namespace Db4objects.Db4o.Internal
{
	internal sealed class Session
	{
		internal readonly string i_fileName;

		internal Db4objects.Db4o.Internal.ObjectContainerBase i_stream;

		private int i_openCount;

		internal Session(string a_fileName)
		{
			i_fileName = a_fileName;
		}

		/// <summary>returns true, if session is to be closed completely</summary>
		internal bool CloseInstance()
		{
			i_openCount--;
			return i_openCount < 0;
		}

		/// <summary>Will raise an exception if argument class doesn't match this class - violates equals() contract in favor of failing fast.
		/// 	</summary>
		/// <remarks>Will raise an exception if argument class doesn't match this class - violates equals() contract in favor of failing fast.
		/// 	</remarks>
		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (null == obj)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				Db4objects.Db4o.Internal.Exceptions4.ShouldNeverHappen();
			}
			return i_fileName.Equals(((Db4objects.Db4o.Internal.Session)obj).i_fileName);
		}

		public override int GetHashCode()
		{
			return i_fileName.GetHashCode();
		}

		internal string FileName()
		{
			return i_fileName;
		}

		internal Db4objects.Db4o.Internal.ObjectContainerBase SubSequentOpen()
		{
			if (i_stream.IsClosed())
			{
				return null;
			}
			i_openCount++;
			return i_stream;
		}
	}
}
