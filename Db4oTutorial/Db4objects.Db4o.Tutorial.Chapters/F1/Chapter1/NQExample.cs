using Db4objects.Db4o;
using Db4objects.Db4o.Query;

using Db4objects.Db4o.Tutorial;

namespace Db4objects.Db4o.Tutorial.F1.Chapter1
{
	public class NQExample : Util
	{
		public static void Main(string[] args)
		{
			IObjectContainer db = Db4o.OpenFile(Util.YapFileName);
			try
			{
				StorePilots(db);
				RetrieveComplexSODA(db);
				RetrieveComplexNQ(db);
				RetrieveArbitraryCodeNQ(db);
				ClearDatabase(db);
			}
			finally
			{
				db.Close();
			}
		}
    
		public static void StorePilots(IObjectContainer db)
		{
			db.Set(new Pilot("Michael Schumacher", 100));
			db.Set(new Pilot("Rubens Barrichello", 99));
		}
    
		public static void RetrieveComplexSODA(IObjectContainer db)
		{
			IQuery query=db.Query();
			query.Constrain(typeof(Pilot));
			IQuery pointQuery=query.Descend("_points");
			query.Descend("_name").Constrain("Rubens Barrichello")
				.Or(pointQuery.Constrain(99).Greater()
				.And(pointQuery.Constrain(199).Smaller()));
			IObjectSet result=query.Execute();
			ListResult(result);
		}

		public static void RetrieveComplexNQ(IObjectContainer db)
		{
			IObjectSet result = db.Query(new ComplexQuery());
			ListResult(result);
		}

		public static void RetrieveArbitraryCodeNQ(IObjectContainer db)
		{
			IObjectSet result = db.Query(new ArbitraryQuery(new int[]{1,100}));
			ListResult(result);
		}
    
		public static void ClearDatabase(IObjectContainer db)
		{
			IObjectSet result = db.Get(typeof(Pilot));
			while (result.HasNext())
			{
				db.Delete(result.Next());
			}
		}
	}
}
