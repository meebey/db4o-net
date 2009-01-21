using System;
using System.IO;

using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4odoc.Tutorial;
using Db4odoc.Tutorial.F1;

namespace Db4odoc.Tutorial.F1.Chapter3
{
    public class OMEExample : Util
    {
        readonly static string YapFileName = Path.Combine(
                               Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                               "ome.db4o");

        public static void Main(string[] args)
        {
            File.Delete(YapFileName);
            StorePilots();
        }

        public static void StorePilots()
        {
            IObjectContainer db = Db4oFactory.OpenFile(YapFileName);
            try
            {
                Pilot pilot1 = new Pilot("Michael Schumacher", 100);
                db.Store(pilot1);
                Console.WriteLine("Stored {0}", pilot1);
                Pilot pilot2 = new Pilot("Rubens Barrichello", 99);
                db.Store(pilot2);
                Console.WriteLine("Stored {0}", pilot2);
            }
            finally
            {
                db.Close();
            }
        }

    }
}
