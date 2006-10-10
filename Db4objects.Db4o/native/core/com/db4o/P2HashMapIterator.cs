/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Collections;
using Sharpen.Lang;
using Sharpen.Util;

namespace Db4objects.Db4o {

    internal class P2HashMapIterator : IDictionaryEnumerator  {

        private P1HashElement i_previous;

        private P1HashElement i_current;

        private P2HashMap i_map;

        private int i_nextIndex;

        private bool i_firstMoved;
      
        internal P2HashMapIterator(P2HashMap p2hashmap) : base() {
            i_map = p2hashmap;
            i_nextIndex = -1;
            GetNextCurrent();
        }

        public virtual Object Current{
            get{
                return Entry;
            }
        }

        public DictionaryEntry Entry{
            get{
                lock (i_map.StreamLock()) {
                    i_map.CheckActive();
                    CheckFirstMoved();
                    Object key = i_current.ActivatedKey(i_map.ElementActivationDepth());
                    return new DictionaryEntry(key,  i_map.Get4(key));
                }
            }
        }

        public Object Key{
            get{
                lock (i_map.StreamLock()) {
                    i_map.CheckActive();
                    CheckFirstMoved();
                    return i_current.ActivatedKey(i_map.ElementActivationDepth());
                }
            }
        }

        public bool MoveNext(){
            lock (i_map.StreamLock()) {
                i_map.CheckActive();
                if(! i_firstMoved){
                    i_firstMoved = true;
                }else{
                    if(i_current != null){
                        GetNextCurrent();
                    }
                }
                return i_current != null;
            }
        }

        public void Reset(){
            lock (i_map.StreamLock()) {
                i_map.CheckActive();
                i_previous = null;
                i_current = null;
                i_firstMoved = false;
                i_nextIndex = -1;
                GetNextCurrent();
            }
        }

        public Object Value{
            get{
                lock (i_map.StreamLock()) {
                    i_map.CheckActive();
                    CheckFirstMoved();
                    return i_map.Get4(i_current.ActivatedKey(i_map.ElementActivationDepth()));
                }
            }
        }

        private void CheckFirstMoved(){
            if(i_current == null || ! i_firstMoved){
                throw new InvalidOperationException("Enumerator is positioned before first or after last.");
            }
        }

        private int CurrentIndex() {
            if (i_current == null) return -1;
            return i_current.i_hashCode & i_map.i_mask;
        }
      
        private void GetNextCurrent() {
            i_previous = i_current;
            i_current = (P1HashElement)NextElement();
            if (i_current != null) i_current.CheckActive();
        }
      
        internal bool HasNext() {
            return i_current != null;
        }
      
        internal Object Next() {
            Object ret = null;
            if (i_current != null){
                ret = i_current.ActivatedKey(i_map.ElementActivationDepth());
            }
            GetNextCurrent();
            return ret;
        }
      
        private P1ListElement NextElement() {
            if (i_current != null && i_current.i_next != null) return i_current.i_next;
            if (i_nextIndex <= CurrentIndex()) SearchNext();
            if (i_nextIndex >= 0) return i_map.i_table[i_nextIndex];
            return null;
        }

        private void SearchNext() {
            if (i_nextIndex > -2) {
                while (++i_nextIndex < i_map.i_tableSize) {
                    if (i_map.i_table[i_nextIndex] != null) return;
                }
                i_nextIndex = -2;
            }
        }
    }
}