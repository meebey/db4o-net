/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Collections;
using Sharpen.Lang;
using Sharpen.Util;

namespace Db4objects.Db4o {

    internal class P2HashMapKeySet : ICollection {

        protected P2HashMap i_map;
      
        internal P2HashMapKeySet(P2HashMap p2hashmap) : base() {
            i_map = p2hashmap;
        }

        public int Count{
            get{
                lock (i_map.StreamLock()) {
                    i_map.CheckActive();
                    return i_map.i_size;
                }
            }
        }

        public void CopyTo(Array arr, int pos){
            lock (i_map.StreamLock()) {
                i_map.CheckActive();
                P2HashMapKeyIterator i = new P2HashMapKeyIterator(i_map);
                while (i.HasNext()) {
                    arr.SetValue(i.Next(), pos++);
                }
            }
        }

        public IEnumerator GetEnumerator(){
            i_map.CheckActive();
            return new P2HashMapKeyIterator(i_map);
        }

        public bool IsSynchronized{
            get{
                return true;
            }
        }

        public Object SyncRoot{
            get{
                i_map.CheckActive();
                return i_map.StreamLock();
            }
        }
    }
}