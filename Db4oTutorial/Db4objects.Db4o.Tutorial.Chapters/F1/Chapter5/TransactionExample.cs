using System;
using System.IO;
using Db4objects.Db4o;

namespace Db4odoc.Tutorial.F1.Chapter6
{
    public class TransactionExample : Util
    {
        readonly static string YapFileName = Path.Combine(
                               Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                               "formula1.yap");  
        public static void Main(string[] args)
        {
            File.Delete(YapFileName);
            IObjectContainer db=Db4oFactory.OpenFile(YapFileName);
            try
            {
                StoreCarCommit(db);
                db.Close();
                db = Db4oFactory.OpenFile(YapFileName);
                ListAllCars(db);
                StoreCarRollback(db);
                db.Close();
                db = Db4oFactory.OpenFile(YapFileName);
                ListAllCars(db);
                CarSnapshotRollback(db);
                CarSnapshotRollbackRefresh(db);
            }
            finally
            {
                db.Close();
            }
        }
        
        public static void StoreCarCommit(IObjectContainer db)
        {
            Pilot pilot = new Pilot("Rubens Barrichello", 99);
            Car car = new Car("BMW");
            car.Pilot = pilot;
            db.Store(car);
            db.Commit();
        }
    
        public static void ListAllCars(IObjectContainer db)
        {
            IObjectSet result = db.QueryByExample(typeof(Car));
            ListResult(result);
        }
        
        public static void StoreCarRollback(IObjectContainer db)
        {
            Pilot pilot = new Pilot("Michael Schumacher", 100);
            Car car = new Car("Ferrari");
            car.Pilot = pilot;
            db.Store(car);
            db.Rollback();
        }
    
        public static void CarSnapshotRollback(IObjectContainer db)
        {
            IObjectSet result = db.QueryByExample(new Car("BMW"));
            Car car = (Car)result.Next();
            car.Snapshot();
            db.Store(car);
            db.Rollback();
            Console.WriteLine(car);
        }
    
        public static void CarSnapshotRollbackRefresh(IObjectContainer db)
        {
            IObjectSet result=db.QueryByExample(new Car("BMW"));
            Car car=(Car)result.Next();
            car.Snapshot();
            db.Store(car);
            db.Rollback();
            db.Ext().Refresh(car, int.MaxValue);
            Console.WriteLine(car);
        }
    }
}
