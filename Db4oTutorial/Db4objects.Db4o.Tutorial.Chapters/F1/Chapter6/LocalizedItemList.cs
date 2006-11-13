using System.Globalization;

namespace Db4objects.Db4o.Tutorial.F1.Chapter6
{
    /// <summary>
    /// A CultureInfo aware list of objects.
    /// CultureInfo objects hold a native pointer to 
    /// a system structure.
    /// </summary>
    public class LocalizedItemList
    {
        CultureInfo _culture;
        string[] _items;

        public LocalizedItemList(CultureInfo culture, string[] items)
        {
            _culture = culture;
            _items = items;
        }

        override public string ToString()
        {
            return string.Join(string.Concat(_culture.TextInfo.ListSeparator,  " "), _items);
        }
    }
}