using System;
using System.IO;
using Db4objects.Db4o;

namespace Db4objects.Db4o.Tutorial.F1.Chapter5
{
	public class DeepExample : Util
    {
        public static void Main(string[] args)
        {
            File.Delete(Util.YapFileName);
            IObjectContainer db = Db4oFactory.OpenFile(Util.YapFileName);
            try
            {
                StoreCar(db);
                db.Close();
                SetCascadeOnUpdate();
                db = Db4oFactory.OpenFile(Util.YapFileName);
                TakeManySnapshots(db);
                db.Close();
                db = Db4oFactory.OpenFile(Util.YapFileName);
                RetrieveAllSnapshots(db);
                db.Close();
                db = Db4oFactory.OpenFile(Util.YapFileName);
                RetrieveSnapshotsSequentially(db);
                RetrieveSnapshotsSequentiallyImproved(db);
                db.Close();
                SetActivationDepth();
                db = Db4oFactory.OpenFile(Util.YapFileName);
                RetrieveSnapshotsSequentially(db);
            }
            finally
            {
                db.Close();
            }
        }
        
        public static void StoreCar(IObjectContainer db)
        {
            Pilot pilot = new Pilot("Rubens Barrichello", 99);
            Car car = new Car("BMW");
            car.Pilot = pilot;
            db.Set(car);
        }
        
        public static void SetCascadeOnUpdate()
        {
            Db4oFactory.Configure().ObjectClass(typeof(Car)).CascadeOnUpdate(true);
        }
        
        public static void TakeManySnapshots(IObjectContainer db)
        {
            IObjectSet result = db.Get(typeof(Car));
            Car car = (Car)result.Next();
            for (int i=0; i<5; i++)
            {
                car.Snapshot();
            }
            db.Set(car);
        }
        
        public static void RetrieveAllSnapshots(IObjectContainer db)
        {
            IObjectSet result = db.Get(typeof(SensorReadout));
            while (result.HasNext())
            {
                Console.WriteLine(result.Next());
            }
        }
        
        public static void RetrieveSnapshotsSequentially(IObjectContainer db)
        {
            IObjectSet result = db.Get(typeof(Car));
            Car car = (Car)result.Next();
            SensorReadout readout = car.GetHistory();
            while (readout != null)
            {
                Console.WriteLine(readout);
                readout = readout.Next;
            }
        }
        
        public static void RetrieveSnapshotsSequentiallyImproved(IObjectContainer db)
        {
            IObjectSet result = db.Get(typeof(Car));
            Car car = (Car)result.Next();
            SensorReadout readout = car.GetHistory();
            while (readout != null)
            {
                db.Activate(readout, 1);
                Console.WriteLine(readout);
                readout = readout.Next;
            }
        }
        
        public static void SetActivationDepth()
        {
            Db4oFactory.Configure().ObjectClass(typeof(TemperatureSensorReadout))
                .CascadeOnActivate(true);
        }
        
    }
}
