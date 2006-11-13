using System.IO;

using Db4objects.Db4o.Query;

using Db4objects.Db4o.Tutorial.F1.Chapter3;

namespace Db4objects.Db4o.Tutorial.F1.Chapter6
{
	public class EvaluationExample : Util
	{
		public static void Main(string[] args)
		{
			File.Delete(Util.YapFileName);
			IObjectContainer db = Db4o.OpenFile(Util.YapFileName);
			try
			{
				StoreCars(db);
				QueryWithEvaluation(db);
			}
			finally
			{
				db.Close();
			}
		}

		public static void StoreCars(IObjectContainer db)
		{
			Pilot pilot1 = new Pilot("Michael Schumacher", 100);
			Car car1 = new Car("Ferrari");
			car1.Pilot = pilot1;
			car1.Snapshot();
			db.Set(car1);
			Pilot pilot2 = new Pilot("Rubens Barrichello", 99);
			Car car2 = new Car("BMW");
			car2.Pilot = pilot2;
			car2.Snapshot();
			car2.Snapshot();
			db.Set(car2);
		}

		public static void QueryWithEvaluation(IObjectContainer db)
		{
			IQuery query = db.Query();
			query.Constrain(typeof (Car));
			query.Constrain(new EvenHistoryEvaluation());
			IObjectSet result = query.Execute();
			Util.ListResult(result);
		}
	}
}