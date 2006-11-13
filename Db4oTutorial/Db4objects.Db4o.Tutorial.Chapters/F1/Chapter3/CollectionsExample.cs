using System;
using System.Collections;
using System.IO;

using Db4objects.Db4o;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Tutorial.F1.Chapter3
{	
	public class CollectionsExample : Util
	{
		public static void Main(string[] args)
		{
			File.Delete(Util.YapFileName);            
			IObjectContainer db = Db4o.OpenFile(Util.YapFileName);
			try
			{
				StoreFirstCar(db);
				StoreSecondCar(db);
				RetrieveAllSensorReadouts(db);
				RetrieveSensorReadoutQBE(db);
				RetrieveCarQBE(db);
				RetrieveCollections(db);
				RetrieveArrays(db);
				RetrieveSensorReadoutQuery(db);
				RetrieveCarQuery(db);
				db.Close();
				UpdateCarPart1();
				db = Db4o.OpenFile(Util.YapFileName);
				UpdateCarPart2(db);
				UpdateCollection(db);
				db.Close();
				DeleteAllPart1();
				db=Db4o.OpenFile(Util.YapFileName);
				DeleteAllPart2(db);
				RetrieveAllSensorReadouts(db);
			}
			finally
			{
				db.Close();
			}
		}
        
		public static void StoreFirstCar(IObjectContainer db)
		{
			Car car1 = new Car("Ferrari");
			Pilot pilot1 = new Pilot("Michael Schumacher", 100);
			car1.Pilot = pilot1;
			db.Set(car1);
		}
        
		public static void StoreSecondCar(IObjectContainer db)
		{
			Pilot pilot2 = new Pilot("Rubens Barrichello", 99);
			Car car2 = new Car("BMW");
			car2.Pilot = pilot2;
			car2.Snapshot();
			car2.Snapshot();
			db.Set(car2);       
		}
        
		public static void RetrieveAllSensorReadouts(IObjectContainer db)
		{
			IObjectSet result = db.Get(typeof(SensorReadout));
			ListResult(result);
		}
        
		public static void RetrieveSensorReadoutQBE(IObjectContainer db)
		{
			SensorReadout proto = new SensorReadout(new double[] { 0.3, 0.1 }, DateTime.MinValue, null);
			IObjectSet result = db.Get(proto);
			ListResult(result);
		}
        
		public static void RetrieveCarQBE(IObjectContainer db)
		{
			SensorReadout protoReadout = new SensorReadout(new double[] { 0.6, 0.2 }, DateTime.MinValue, null);
			IList protoHistory = new ArrayList();
			protoHistory.Add(protoReadout);
			Car protoCar = new Car(null, protoHistory);
			IObjectSet result = db.Get(protoCar);
			ListResult(result);
		}
        
		public static void RetrieveCollections(IObjectContainer db)
		{
			IObjectSet result = db.Get(new ArrayList());
			ListResult(result);
		}
        
		public static void RetrieveArrays(IObjectContainer db)
		{
			IObjectSet result = db.Get(new double[] { 0.6, 0.4 });
			ListResult(result);
		}
        
		public static void RetrieveSensorReadoutQuery(IObjectContainer db)
		{
			IQuery query = db.Query();
			query.Constrain(typeof(SensorReadout));
			IQuery valueQuery = query.Descend("_values");
			valueQuery.Constrain(0.3);
			valueQuery.Constrain(0.1);
			IObjectSet results = query.Execute();
			ListResult(results);
		}
        
		public static void RetrieveCarQuery(IObjectContainer db)
		{
			IQuery query = db.Query();
			query.Constrain(typeof(Car));
			IQuery historyQuery = query.Descend("_history");
			historyQuery.Constrain(typeof(SensorReadout));
			IQuery valueQuery = historyQuery.Descend("_values");
			valueQuery.Constrain(0.3);
			valueQuery.Constrain(0.1);
			IObjectSet results = query.Execute();
			ListResult(results);
		}

		public class RetrieveSensorReadoutPredicate : Predicate
		{
			public bool Match(SensorReadout candidate)
			{
				return Array.IndexOf(candidate.Values, 0.3) > -1
					&& Array.IndexOf(candidate.Values, 0.1) > -1;
			}
		}
        
		public static void RetrieveSensorReadoutNative(IObjectContainer db) 
		{
			IObjectSet results = db.Query(new RetrieveSensorReadoutPredicate());
			ListResult(results);
		}

		public class RetrieveCarPredicate : Predicate
		{
			public bool Match(Car car)
			{
				foreach (SensorReadout sensor in car.History)
				{
					if (Array.IndexOf(sensor.Values, 0.3) > -1
						&& Array.IndexOf(sensor.Values, 0.1) > -1)
					{
						return true; 
					}
				}
				return false;
			}
		}

		public static void RetrieveCarNative(IObjectContainer db)
		{
			IObjectSet results = db.Query(new RetrieveCarPredicate());
			ListResult(results);
		}

		public static void UpdateCarPart1()
		{
			Db4o.Configure().ObjectClass(typeof(Car)).CascadeOnUpdate(true);
		}
        
		public static void UpdateCarPart2(IObjectContainer db)
		{
			IObjectSet result = db.Get(new Car("BMW", null));
			Car car = (Car)result.Next();
			car.Snapshot();
			db.Set(car);
			RetrieveAllSensorReadouts(db);
		}
        
		public static void UpdateCollection(IObjectContainer db)
		{
			IQuery query = db.Query();
			query.Constrain(typeof(Car));
			IObjectSet result = query.Descend("_history").Execute();
			IList history = (IList)result.Next();
			history.RemoveAt(0);
			db.Set(history);
			Car proto = new Car(null, null);
			result = db.Get(proto);
			foreach (Car car in result)
			{	
				foreach (object readout in car.History)
				{
					Console.WriteLine(readout);
				}
			}
		}
        
		public static void DeleteAllPart1()
		{
			Db4o.Configure().ObjectClass(typeof(Car)).CascadeOnDelete(true);
		}

		public static void DeleteAllPart2(IObjectContainer db)
		{
			IObjectSet result = db.Get(new Car(null, null));
			foreach (object car in result)
			{
				db.Delete(car);
			}
			IObjectSet readouts = db.Get(new SensorReadout(null, DateTime.MinValue, null));
			foreach (object readout in readouts)
			{
				db.Delete(readout);
			}
		}
	}
}
