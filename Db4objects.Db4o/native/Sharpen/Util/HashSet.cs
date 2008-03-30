/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */
using System;
using System.Collections;
using System.Collections.Specialized;

namespace Sharpen.Util
{
    
    public class HashSet : ISet
    {
        protected IDictionary _dictionary = null;
        private readonly static object _object = new object();

        public HashSet()
        {
            _dictionary = new Hashtable();
        }

        public HashSet(ICollection initialValues)
            : this()
        {
            this.AddAll(initialValues);
        }


        public bool Add(object o)
        {
            if (_dictionary[o] != null)
                return false;
            else
            {
                _dictionary.Add(o, _object);
                return true;
            }
        }

        public bool AddAll(ICollection c)
        {
            bool changed = false;
            foreach (object o in c)
                changed |= this.Add(o);
            return changed;
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(object o)
        {
            return _dictionary[o] != null;
        }

        public bool ContainsAll(ICollection c)
        {
            foreach (object o in c)
            {
                if (!this.Contains(o))
                    return false;
            }
            return true;
        }

        public bool IsEmpty
        {
            get { return _dictionary.Count == 0; }
        }

        public bool Remove(object o)
        {
            bool contained = this.Contains(o);
            if (contained)
            {
                _dictionary.Remove(o);
            }
            return contained;
        }

        public bool RemoveAll(ICollection c)
        {
            bool changed = false;
            foreach (object o in c)
                changed |= this.Remove(o);
            return changed;
        }



        public void CopyTo(Array array, int index)
        {
            _dictionary.Keys.CopyTo(array, index);
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return _dictionary.SyncRoot; }
        }

        public IEnumerator GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

    }
}
