/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Bench.Crud;
using Db4objects.Db4o.Bench.Logging;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.Tutorial.F1.Chapter10
{
    /// <summary>
    /// Very simple CRUD (Create, Read, Update, Delete) application to
    /// produce log files as an input for I/O-benchmarking.
    /// </summary>
    /// <remarks>
    /// Very simple CRUD (Create, Read, Update, Delete) application to
    /// produce log files as an input for I/O-benchmarking.
    /// </remarks>
    public class CrudApplication
    {
        private static readonly string DatabaseFile = "simplecrud.db4o";

        public CrudApplication()
        { }

        public virtual void Run(int itemCount)
        {
            IConfiguration config = Prepare(itemCount);
            Create(itemCount, config);
            Read(config);
            Update(config);
            Delete(config);
            DeleteDbFile();
        }

        private IConfiguration Prepare(int itemCount)
        {
            DeleteDbFile();
            RandomAccessFileAdapter rafAdapter = new RandomAccessFileAdapter();
            IoAdapter ioAdapter = new LoggingIoAdapter(rafAdapter, LogFileName(itemCount));
            IConfiguration config = Db4oFactory.CloneConfiguration();
            config.Io(ioAdapter);
            ioAdapter.Close();
            return config;
        }

        private void Create(int itemCount, IConfiguration config)
        {
            IObjectContainer oc = Db4oFactory.OpenFile(config, DatabaseFile);
            for (int i = 0; i < itemCount; i++)
            {
                oc.Store(Item.NewItem(i));
                // preventing heap space problems by committing from time to time
                if (i % 100000 == 0)
                {
                    oc.Commit();
                }
            }
            oc.Commit();
            oc.Close();
        }

        private void Read(IConfiguration config)
        {
            IObjectContainer oc = Db4oFactory.OpenFile(config, DatabaseFile);
            IObjectSet objectSet = oc.Query(typeof(Item));
            while (objectSet.HasNext())
            {
                Item item = (Item)objectSet.Next();
            }
            oc.Close();
        }

        private void Update(IConfiguration config)
        {
            IObjectContainer oc = Db4oFactory.OpenFile(config, DatabaseFile);
            IObjectSet objectSet = oc.Query(typeof(Item));
            while (objectSet.HasNext())
            {
                Item item = (Item)objectSet.Next();
                item.Change();
                oc.Store(item);
            }
            oc.Close();
        }

        private void Delete(IConfiguration config)
        {
            IObjectContainer oc = Db4oFactory.OpenFile(config, DatabaseFile);
            IObjectSet objectSet = oc.Query(typeof(Item));
            while (objectSet.HasNext())
            {
                oc.Delete(objectSet.Next());
                // adding commit results in more syncs in the log, 
                // which is necessary for meaningful statistics!
                oc.Commit();
            }
            oc.Close();
        }

        private void DeleteDbFile()
        {
            new Sharpen.IO.File(DatabaseFile).Delete();
        }

        public static string LogFileName(int itemCount)
        {
            return "simplecrud_" + itemCount + ".log";
        }
    }
}
