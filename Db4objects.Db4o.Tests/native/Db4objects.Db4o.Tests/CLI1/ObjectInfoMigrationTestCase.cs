using System.IO;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
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
    public class ObjectInfoMigrationTestCase : ITestCase
    {
        public void _Test52UuidAndVersion()
        {
            string originalFile = WorkspaceServices.WorkspaceTestFilePath("net/db4o52.db4o");
            string fname = Path.GetTempFileName();
            File.Copy(originalFile, fname, true);

            IConfiguration configuration = Db4oFactory.NewConfiguration();
            configuration.AllowVersionUpdates(true);
            configuration.AddAlias(new WildcardAlias("*GenerateDb4o52File", "*" + GetType().Assembly.GetName().Name));
            IObjectClass itemConfig = configuration.ObjectClass(typeof(Db4o52Regression.Item));
            itemConfig.ObjectField("_name").Indexed(true);

            IObjectContainer container = Db4oFactory.OpenFile(configuration, fname);
            try
            {
                StringWriter writer = new StringWriter();
                foreach (Db4o52Regression.Item item in container.Get(typeof(Db4o52Regression.Item)))
                {
                    IObjectInfo info = container.Ext().GetObjectInfo(item);
                    writer.WriteLine("{0}, UUID={1}, Version={2}", item.Name, ToUUIDString(info.GetUUID()),
                                      info.GetVersion());
                }
                Assert.AreEqual(ReadAllText(Path.ChangeExtension(originalFile, ".txt")), writer.ToString());
            }
            finally
            {
                container.Close();
            }
        }
        
        static string ReadAllText(string fname)
        {
        	using (StreamReader reader = File.OpenText(fname))
        	{
        		return reader.ReadToEnd();
        	}
        }

        private static string ToUUIDString(Db4oUUID uuid)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(uuid.GetLongPart());
            builder.Append("-");
            foreach (byte b in uuid.GetSignaturePart())
            {
                builder.Append(b.ToString("X0"));
            }
            return builder.ToString();
        }
    }
}
