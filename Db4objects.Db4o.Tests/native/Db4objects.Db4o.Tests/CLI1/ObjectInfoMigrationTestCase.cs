/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Util;
using Db4oUnit;

namespace Db4o52Regression
{
    public class Item
    {
        private string _name;

        public Item(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}

namespace Db4objects.Db4o.Tests.CLI1
{
    public class ObjectInfoMigration52TestCase : ObjectInfoMigrationTestCaseBase
    {
        protected override string OriginalTestFile
        {
            get { return WorkspaceServices.WorkspaceTestFilePath("net/db4o52.db4o"); }
        }
    }

    public class ObjectInfoMigration57TestCase : ObjectInfoMigrationTestCaseBase
    {
        protected override string OriginalTestFile
        {
            get { return WorkspaceServices.WorkspaceTestFilePath("net/db4o57.db4o"); }
        }
    }

    public abstract class ObjectInfoMigrationTestCaseBase : ITestCase, ITestLifeCycle
    {
        protected abstract string OriginalTestFile
        {
            get;
        }

        private static Regex UUIDRegex = new Regex(@"UUID=(.+),\s");

        private IObjectContainer _container;

        public void SetUp()
        {
            if (null == OriginalTestFile)
            {
                Sharpen.Runtime.Err.WriteLine("Build environment not available. Skipping test case...");
                return;
            }

            _container = Db4oFactory.OpenFile(GetConfiguration(), GetTempTestFile());
        }

        public void TestUuidAndVersion()
        {
            if (null == _container)
            {
                return;
            }
            AreEqualTrimmed(GetExpectedItemStrings(), GetItemsString());
        }

        public void TestNewItemsAreReachable()
        {
            if (null == _container)
            {
                return;
            }

            StoreNewItemsAndAssert();
        }

        public void TestNewItemsStoredAfterAccessingOldItemsAreReachable()
        {
            if (null == _container)
            {
                return;
            }
            
            AccessOldItems();
            StoreNewItemsAndAssert();
        }

        public void _TestGetByUUID()
        {
            if (null == _container)
            {
                return;
            }
            
            foreach (string line in GetExpectedItemStrings().Split('\n'))
            {
                if (line.Length == 0) continue;
                Db4oUUID uuid = ExtractUUID(line);
                Db4o52Regression.Item item = (Db4o52Regression.Item) _container.Ext().GetByUUID(uuid);
                Assert.IsNotNull(item, line);
                Assert.AreEqual(uuid, GetObjectInfo(item).GetUUID());
            }
        }

        private static Db4oUUID ExtractUUID(string line)
        {
            string uuidString = ExtractUUIDString(line);
            return ToUUID(uuidString);
        }

        public void TestUUIDConversion()
        {
            if (null == _container)
            {
                return;
            }

            foreach (string line in GetExpectedItemStrings().Split('\n'))
            {
                if (line.Length == 0) continue;

                string uuidString = ExtractUUIDString(line);
                Assert.AreEqual(uuidString, ToUUIDString(ToUUID(uuidString)));
            }
        }

        private static string ExtractUUIDString(string line)
        {
            return UUIDRegex.Match(line).Groups[1].Value;
        }

        private void AccessOldItems()
        {
            GetItemsString();
        }

        private void StoreNewItemsAndAssert()
        {
            string newItemsString = StoreNewItems();
            ReOpen();
            AreEqualTrimmed(newItemsString + GetExpectedItemStrings(), GetItemsString());
        }

        private string StoreNewItems()
        {
            StringWriter writer = new StringWriter();
            for (int i = 0; i < 3; ++i)
            {
                Db4o52Regression.Item newItem = new Db4o52Regression.Item("New " + i);
                _container.Store(newItem);
                WriteItemString(writer, newItem);
                WriteItemString(Sharpen.Runtime.Out, newItem);
            }
            return writer.ToString().Trim();
        }

        private static void AreEqualTrimmed(string expected, string actual)
        {
            Assert.AreEqual(NormalizeWhiteSpace(expected), NormalizeWhiteSpace(actual));
        }

        private static string NormalizeWhiteSpace(string s)
        {
            return s.Trim().Replace("\r", "");
        }

        private string GetItemsString()
        {
            StringWriter writer = new StringWriter();
            foreach (Db4o52Regression.Item item in GetItems())
            {
                WriteItemString(writer, item);
            }
            return writer.ToString();
        }

        private IObjectSet GetItems()
        {
            IQuery q = _container.Query();
            q.Constrain(typeof(Db4o52Regression.Item));
            q.Descend("_name").OrderAscending();
            return q.Execute();
        }

        private void ReOpen()
        {
            string fname = ((LocalObjectContainer)_container).FileName();
            _container.Close();
            _container = Db4oFactory.OpenFile(GetConfiguration(), fname);
        }

        private void WriteItemString(TextWriter writer, Db4o52Regression.Item item)
        {
            IObjectInfo info = GetObjectInfo(item);
            writer.WriteLine("{0}, UUID={1}, Version={2}", item.Name, ToUUIDString(info.GetUUID()),
                             info.GetVersion());
        }

        private IObjectInfo GetObjectInfo(Db4o52Regression.Item item)
        {
            return _container.Ext().GetObjectInfo(item);
        }

        private string GetExpectedItemStrings()
        {
            string text = ReadAllText(Path.ChangeExtension(OriginalTestFile, ".txt"));
            string[] lines = text.Split('\n');
            Array.Sort(lines);
            return string.Join("\n", lines);
        }

        public void TearDown()
        {
            if (null != _container)
            {
                _container.Close();
                _container = null;
            }
        }
        
        static string ReadAllText(string fname)
        {
        	using (StreamReader reader = File.OpenText(fname))
        	{
        		return reader.ReadToEnd();
        	}
        }

        private static Db4oUUID ToUUID(string s)
        {
            string[] parts = s.Split('-');
            return new Db4oUUID(long.Parse(parts[0]), ParseSignature(parts[1]));
        }

        private static byte[] ParseSignature(string s)
        {
            byte[] signature = new byte[s.Length/2];
            for (int i = 0; i < signature.Length; ++i)
            {
                signature[i] = byte.Parse(s.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return signature;
        }

        private static string ToUUIDString(Db4oUUID uuid)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(uuid.GetLongPart());
            builder.Append("-");
            foreach (byte b in uuid.GetSignaturePart())
            {
                builder.Append(b.ToString("X02"));
            }
            return builder.ToString();
        }

        private string GetTempTestFile()
        {
            string fname = Path.GetTempFileName();
            File.Copy(OriginalTestFile, fname, true);
            return fname;
        }

        private IConfiguration GetConfiguration()
        {
            IConfiguration configuration = Db4oFactory.NewConfiguration();
            configuration.AllowVersionUpdates(true);
            configuration.AddAlias(new WildcardAlias("*GenerateDb4o52File", "*" + GetType().Assembly.GetName().Name));
            IObjectClass itemConfig = configuration.ObjectClass(typeof(Db4o52Regression.Item));
            itemConfig.ObjectField("_name").Indexed(true);
            return configuration;
        }

    }
}
