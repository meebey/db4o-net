using System.Collections.Generic;
using System.Linq;

using Db4objects.Db4o;
using Db4objects.Db4o.Linq;
using Db4objects.Db4o.Query;

using Db4objects.Db4o.Tutorial;

namespace Db4objects.Db4o.Tutorial.F1.Chapter9
{
	public class NQExample : Util
	{
		public static void Main(string[] args)
		{
			IObjectContainer db = Db4oFactory.OpenFile(Util.YapFileName);
			try
			{
				StoreObjects(db);
                RetrievePilot(db);
                RetrievePilotByCars(db);
                RetrievePilotUnoptimized(db);
				ClearDatabase(db);
			}
			finally
			{
				db.Close();
			}
		}
    
		public static void StoreObjects(IObjectContainer db)
		{
			db.Store(new Car("Ferrari", (new Pilot("Michael Schumacher", 100))));
			db.Store(new Car("BMW", (new Pilot("Rubens Barrichello", 99))));
		}
    
		public static void RetrievePilot(IObjectContainer db)
		{
            IEnumerable<Pilot> result = from Pilot p in db
                                        where p.Name.StartsWith("Michael")
                                        select p;
			ListResult(result);
		}

        public static void RetrievePilotByCars(IObjectContainer db)
        {
            IEnumerable<Pilot> result = from Car c in db
                                        where c.Model.StartsWith("F")
                                        && (c.Pilot.Points > 99 && c.Pilot.Points <150)
                                        select c.Pilot;
            ListResult(result);
        }

        public static void RetrievePilotUnoptimized(IObjectContainer db)
        {
            IEnumerable<Pilot> result = from Pilot p in db
                                        where (p.Points - 81) == p.Name.Length
                                        select p;
            ListResult(result);
        }

		public static void ClearDatabase(IObjectContainer db)
		{
			IObjectSet result = db.QueryByExample(null);
			while (result.HasNext())
			{
				db.Delete(result.Next());
			}
		}

        private static void ListResult<T>(IEnumerable<T> result)
        {
            foreach (T t in result)
            {
                System.Console.WriteLine(t);
            }
        }
	}
}
