namespace Db4objects.Db4o.Tests.CLI1.Aliases
{
    class JavaFromNetWithSameNameTestCase : JavaFromNetAliasesTestCase
    {
        protected override System.Type GetAliasedDataType()
        {
            return typeof (com.db4o.test.aliases.Person2);
        }
    }
}


namespace com.db4o.test.aliases
{
    public class Person2
    {
        private string _name;

        public Person2(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public override bool Equals(object obj)
        {
            Person2 other = obj as Person2;
            if (null == other) return false;
            return Db4objects.Db4o.Tests.CLI1.Aliases.CFHelper.AreEqual(_name, other._name);
        }
    }
}
