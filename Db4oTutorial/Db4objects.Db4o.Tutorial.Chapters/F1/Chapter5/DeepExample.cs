using System;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;

namespace Db4odoc.Tutorial.F1.Chapter6
{
	public class DeepExample : Util
    {
        readonly static string YapFileName = Path.Combine(
                               Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                               "formula1.yap");  
		
        public static void Main(string[] args)
        {
            File.Delete(YapFileName);
            IObjectContainer db = Db4oFactory.OpenFile(YapFileName);
                StoreCar(db);
                db.Close();
                TakeManySnapshots();
                db = Db4oFactory.OpenFile(YapFileName);
                RetrieveAllSnapshots(db);
                db.Close();
                db = Db4oFactory.OpenFile(YapFileName);
                RetrieveSnapshotsSequentially(db);
                RetrieveSnapshotsSequentiallyImproved(db);
                db.Close();
                RetrieveSnapshotsSequentiallyCascade();
        }
        
        public static void StoreCar(IObjectContainer db)
        {
            Pilot pilot = new Pilot("Rubens Barrichello", 99);
            Car car = new Car("BMW");
            car.Pilot = pilot;
            db.Store(car);
        }
        
        public static void TakeManySnapshots()
        {
            IConfiguration config = Db4oFactory.NewConfiguration();
            config.ObjectClass(typeof(Car)).CascadeOnUpdate(true);
            IObjectContainer db = Db4oFactory.OpenFile(config, YapFileName);
            IObjectSet result = db.QueryByExample(typeof(Car));
            Car car = (Car)result.Next();
            for (int i=0; i<5; i++)
            {
                car.Snapshot();
            }
            db.Store(car);
            db.Close();
        }
        
        public static void RetrieveAllSnapshots(IObjectContainer db)
        {
            IObjectSet result = db.QueryByExample(typeof(SensorReadout));
            while (result.HasNext())
            {
                Console.WriteLine(result.Next());
            }
        }
        
        public static void RetrieveSnapshotsSequentially(IObjectContainer db)
        {
            IObjectSet result = db.QueryByExample(typeof(Car));
            Car car = (Car)result.Next();
            SensorReadout readout = car.GetHistory();
            while (readout != null)
            {
                Console.WriteLine(readout);
                readout = readout.Next;
            }
        }
        
        
        public static void RetrieveSnapshotsSequentiallyCascade()
        {
            IConfiguration config = Db4oFactory.NewConfiguration();
            config.ObjectClass(typeof(TemperatureSensorReadout))
                .CascadeOnActivate(true);
            IObjectContainer db = Db4oFactory.OpenFile(config, YapFileName);
        
            IObjectSet result = db.QueryByExample(typeof(Car));
            Car car = (Car)result.Next();
            SensorReadout readout = car.GetHistory();
            while (readout != null)
            {
                Console.WriteLine(readout);
                readout = readout.Next;
            }
            db.Close();
        }
        
        public static void RetrieveSnapshotsSequentiallyImproved(IObjectContainer db)
        {
            IObjectSet result = db.QueryByExample(typeof(Car));
            Car car = (Car)result.Next();
            SensorReadout readout = car.GetHistory();
            while (readout != null)
            {
                db.Activate(readout, 1);
                Console.WriteLine(readout);
                readout = readout.Next;
            }
        }
        
    }
}
