using System;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tutorial.F1;

namespace Db4objects.Db4o.Tutorial.F1.Chapter8
{
    public class TransparentPersistenceExample : Util
    {
        public static void Main(String[] args)
        {
            File.Delete(YapFileName);
            ConfigureTransparentPersistence();
            IObjectContainer db = Db4oFactory.OpenFile(YapFileName);
            try
            {
                StoreCarAndSnapshots(db);
                db.Close();
                db = Db4oFactory.OpenFile(YapFileName);
                ModifySnapshotHistory(db);
                db.Close();
                db = Db4oFactory.OpenFile(YapFileName);
                ReadSnapshotHistory(db);
            }
            finally
            {
                db.Close();
            }
        }

        public static void ConfigureTransparentPersistence()
        {
            Db4oFactory.Configure().Add(new TransparentPersistenceSupport());
        }

        public static void StoreCarAndSnapshots(IObjectContainer db)
        {
            Car car = new Car("Ferrari");
            for (int i = 0; i < 3; i++)
            {
                car.snapshot();
            }
            db.Store(car);
        }

    public static void ModifySnapshotHistory(IObjectContainer db) {
    	System.Console.WriteLine("Read all sensors and modify the description:");
        IObjectSet result=db.QueryByExample(typeof(Car));
        Car car=(Car)result.Next();
        SensorReadout readout=car.History;
        while(readout!=null) {
            System.Console.WriteLine(readout);
        	readout.Description = "Modified: " + readout.Description;
            readout = readout.Next;
        }
        db.Commit();
    }

    public static void ReadSnapshotHistory(IObjectContainer db) {
    	System.Console.WriteLine("Read all modified sensors:");
        IObjectSet result=db.QueryByExample(typeof(Car));
        Car car=(Car)result.Next();
        SensorReadout readout=car.History;
        while(readout!=null) {
            System.Console.WriteLine(readout);
            readout=readout.Next;
        }
    }
}
}