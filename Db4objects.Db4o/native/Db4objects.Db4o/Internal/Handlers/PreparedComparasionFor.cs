using System;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Internal.Handlers
{
    internal class PreparedComparasionFor<T> : IPreparedComparison where T : IComparable<T>
    {
        private readonly T _source;
        public PreparedComparasionFor(T source)
        {
            _source = source;
        }

        public int CompareTo(object obj)
        {
            T target = ((T)obj);
            return _source.CompareTo(target);
        }
    }
}
