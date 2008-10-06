using System;
using System.Collections;
using System.IO;

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Tutorial.F1.Chapter3
{	
	public class CollectionsExample : Util
	{
        readonly static string YapFileName = Path.Combine(
                               Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                               "formula1.yap");  
		
		public static void Main(string[] args)
		{
			File.Delete(YapFileName);            
			IObjectContainer db = Db4oFactory.OpenFile(YapFileName);
				StoreFirstCar(db);
				StoreSecondCar(db);
				RetrieveAllSensorReadout(db);
				RetrieveSensorReadoutQBE(db);
				RetrieveCarQBE(db);
				RetrieveCollections(db);
				RetrieveArrays(db);
				RetrieveSensorReadoutQuery(db);
				RetrieveCarQuery(db);
				db.Close();
				UpdateCar();
				UpdateCollection();
				DeleteAll();
				RetrieveAllSensorReadout(db);
			
		}
        
		public static void StoreFirstCar(IObjectContainer db)
		{
			Car car1 = new Car("Ferrari");
			Pilot pilot1 = new Pilot("Michael Schumacher", 100);
			car1.Pilot = pilot1;
			db.Store(car1);
		}
        
		public static void StoreSecondCar(IObjectContainer db)
		{
			Pilot pilot2 = new Pilot("Rubens Barrichello", 99);
			Car car2 = new Car("BMW");
			car2.Pilot = pilot2;
			car2.Snapshot();
			car2.Snapshot();
			db.Store(car2);       
		}
        
		public static void RetrieveAllSensorReadout(IObjectContainer db)
		{
			IObjectSet result = db.QueryByExample(typeof(SensorReadout));
			ListResult(result);
		}
        
		public static void RetrieveSensorReadoutQBE(IObjectContainer db)
		{
			SensorReadout proto = new SensorReadout(new double[] { 0.3, 0.1 }, DateTime.MinValue, null);
			IObjectSet result = db.QueryByExample(proto);
			ListResult(result);
		}
        
		public static void RetrieveCarQBE(IObjectContainer db)
		{
			SensorReadout protoReadout = new SensorReadout(new double[] { 0.6, 0.2 }, DateTime.MinValue, null);
			IList protoHistory = new ArrayList();
			protoHistory.Add(protoReadout);
			Car protoCar = new Car(null, protoHistory);
			IObjectSet result = db.QueryByExample(protoCar);
			ListResult(result);
		}
        
		public static void RetrieveCollections(IObjectContainer db)
		{
			IObjectSet result = db.QueryByExample(new ArrayList());
			ListResult(result);
		}
        
		public static void RetrieveArrays(IObjectContainer db)
		{
			IObjectSet result = db.QueryByExample(new double[] { 0.6, 0.4 });
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

		public static void UpdateCar()
		{
            IConfiguration config = Db4oFactory.NewConfiguration();
            config.ObjectClass(typeof(Car)).CascadeOnUpdate(true);
            IObjectContainer db = Db4oFactory.OpenFile(config, YapFileName);
			IObjectSet result = db.QueryByExample(new Car("BMW", null));
			Car car = (Car)result.Next();
			car.Snapshot();
			db.Store(car);
			RetrieveAllSensorReadout(db);
            db.Close();
		}
        
		public static void UpdateCollection()
		{
            IConfiguration config = Db4oFactory.NewConfiguration();
            config.ObjectClass(typeof(Car)).CascadeOnUpdate(true);
            IObjectContainer db = Db4oFactory.OpenFile(config, YapFileName);
			IQuery query = db.Query();
			query.Constrain(typeof(Car));
			IObjectSet result = query.Descend("_history").Execute();
			IList history = (IList)result.Next();
			history.RemoveAt(0);
			db.Store(history);
			Car proto = new Car(null, null);
			result = db.QueryByExample(proto);
			foreach (Car car in result)
			{	
				foreach (object readout in car.History)
				{
					Console.WriteLine(readout);
				}
			}
            db.Close();
		}
        
		public static void DeleteAll()
		{
            IConfiguration config = Db4oFactory.NewConfiguration();
            config.ObjectClass(typeof(Car)).CascadeOnDelete(true);
            IObjectContainer db = Db4oFactory.OpenFile(config, YapFileName);

			IObjectSet result = db.QueryByExample(new Car(null, null));
			foreach (object car in result)
			{
				db.Delete(car);
			}
			IObjectSet readouts = db.QueryByExample(new SensorReadout(null, DateTime.MinValue, null));
			foreach (object readout in readouts)
			{
				db.Delete(readout);
			}
            db.Close();
		}
	}
}
