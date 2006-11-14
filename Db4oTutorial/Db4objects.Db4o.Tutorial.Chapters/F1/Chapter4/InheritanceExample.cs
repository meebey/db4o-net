using System;
using System.IO;
using Db4objects.Db4o;

using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Tutorial.F1.Chapter4
{   
    public class InheritanceExample : Util
    {        
        public static void Main(string[] args)
        {
            File.Delete(Util.YapFileName);          
            IObjectContainer db = Db4oFactory.OpenFile(Util.YapFileName);
            try
            {
                StoreFirstCar(db);
                StoreSecondCar(db);
                RetrieveTemperatureReadoutsQBE(db);
                RetrieveAllSensorReadoutsQBE(db);
                RetrieveAllSensorReadoutsQBEAlternative(db);
                RetrieveAllSensorReadoutsQuery(db);
                RetrieveAllObjects(db);
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
        
        public static void RetrieveAllSensorReadoutsQBE(IObjectContainer db)
        {
            SensorReadout proto = new SensorReadout(DateTime.MinValue, null, null);
            IObjectSet result = db.Get(proto);
            ListResult(result);
        }
        
        public static void RetrieveTemperatureReadoutsQBE(IObjectContainer db)
        {
            SensorReadout proto = new TemperatureSensorReadout(DateTime.MinValue, null, null, 0.0);
            IObjectSet result = db.Get(proto);
            ListResult(result);
        }
        
        public static void RetrieveAllSensorReadoutsQBEAlternative(IObjectContainer db)
        {
            IObjectSet result = db.Get(typeof(SensorReadout));
            ListResult(result);
        }
        
        public static void RetrieveAllSensorReadoutsQuery(IObjectContainer db)
        {
            IQuery query = db.Query();
            query.Constrain(typeof(SensorReadout));
            IObjectSet result = query.Execute();
            ListResult(result);
        }
        
        public static void RetrieveAllObjects(IObjectContainer db)
        {
            IObjectSet result = db.Get(new object());
            ListResult(result);
        }
    }
}
