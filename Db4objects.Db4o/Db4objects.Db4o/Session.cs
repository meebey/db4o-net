namespace Db4objects.Db4o
{
	internal sealed class Session
	{
		internal readonly string i_fileName;

		internal Db4objects.Db4o.YapStream i_stream;

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

		public override bool Equals(object a_object)
		{
			return i_fileName.Equals(((Db4objects.Db4o.Session)a_object).i_fileName);
		}

		internal string FileName()
		{
			return i_fileName;
		}

		internal Db4objects.Db4o.YapStream SubSequentOpen()
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
