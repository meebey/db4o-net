namespace Db4objects.Db4o.Tests.CLI1.Aliases
{
    class CFHelper
    {
        public static bool AreEqual(object l, object r)
        {
            if (l == r) return true;
            if (l == null || r == null) return false;
            return l.Equals(r);
        }
    }
}