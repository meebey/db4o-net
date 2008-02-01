using System;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tutorial.F1;

namespace Db4objects.Db4o.Tutorial.F1.Chapter7
{
    public class TransparentActivationExample : Util
    {
        public static void Main(String[] args)
        {
            File.Delete(YapFileName);
            IObjectContainer db = Db4oFactory.OpenFile(YapFileName);
            try
            {
                StoreCarAndSnapshots(db);
                db.Close();

                db = Db4oFactory.OpenFile(YapFileName);
                RetrieveSnapshotsSequentially(db);
                db.Close();

                ConfigureTransparentActivation();
                db = Db4oFactory.OpenFile(YapFileName);
                RetrieveSnapshotsSequentially(db);
                db.Close();

                db = Db4oFactory.OpenFile(YapFileName);
                DemonstrateTransparentActivation(db);
                db.Close();
            }
            finally
            {
                db.Close();
            }
        }

        public static void ConfigureTransparentActivation()
        {
            Db4oFactory.Configure().Add(new TransparentActivationSupport());
        }

        public static void SetCascadeOnUpdate()
        {
            Db4oFactory.Configure().ObjectClass(typeof (Car)).CascadeOnUpdate(true);
        }

        public static void StoreCarAndSnapshots(IObjectContainer db)
        {
            Pilot pilot = new Pilot("Kimi Raikkonen", 110);
            Car car = new Car("Ferrari");
            car.Pilot = pilot;
            for (int i = 0; i < 5; i++)
            {
                car.snapshot();
            }
            db.Store(car);
        }

        public static void RetrieveSnapshotsSequentially(IObjectContainer db)
        {
            IObjectSet result = db.Get(typeof (Car));
            Car car = (Car) result.Next();
            SensorReadout readout = car.History;
            while (readout != null)
            {
                Console.WriteLine(readout);
                readout = readout.Next;
            }
        }

        public static void DemonstrateTransparentActivation(IObjectContainer db)
        {
            IObjectSet result = db.Get(typeof (Car));
            Car car = (Car) result.Next();

            Console.WriteLine("#PilotWithoutActivation before the car is activated");
            Console.WriteLine(car.PilotWithoutActivation);

            Console.WriteLine("accessing 'Pilot' property activates the car object");
            Console.WriteLine(car.Pilot);

            Console.WriteLine("Accessing PilotWithoutActivation property after the car is activated");
            Console.WriteLine(car.PilotWithoutActivation);
        }
    }
}