/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */
using System.IO;

namespace Db4objects.Db4o.Foundation.IO
{
    public class File4
    {
        public static void Delete(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        public static void Copy(string from, string to)
        {
            File.Copy(from, to, true);
        }
    }
}