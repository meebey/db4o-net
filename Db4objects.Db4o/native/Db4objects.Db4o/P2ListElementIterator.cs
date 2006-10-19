/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Collections;
using Sharpen.Lang;
using Sharpen.Util;

namespace Db4objects.Db4o {

    internal class P2ListElementIterator : IEnumerator{

        private P2LinkedList i_list;

        private P1ListElement i_preprevious;

        private P1ListElement i_previous;

        private P1ListElement i_next;

        private bool i_firstMoved;
      
        internal P2ListElementIterator(P2LinkedList p2linkedlist, P1ListElement p1listelement) : base() {
            i_list = p2linkedlist;
            i_next = p1listelement;
            CheckNextActive();
        }

        public Object Current {
            get{
                if(i_next == null || ! i_firstMoved){
                    throw new InvalidOperationException("Enumerator is positioned before first or after last.");
                }
                lock (i_next.StreamLock()) {
                    return i_next.ActivatedObject(i_list.ElementActivationDepth());
                }
            }
        }

        public bool MoveNext(){
            if (i_next != null) {
                lock (i_next.StreamLock()) {
                    if(! i_firstMoved){
                        i_firstMoved = true;
                        return i_next != null;
                    }
                    i_preprevious = i_previous;
                    i_previous = i_next;
                    Object obj1 = i_next.ActivatedObject(i_list.ElementActivationDepth());
                    i_next = i_next.i_next;
                    CheckNextActive();
                    return i_next != null;
                }
            }
            return false;
        }

        public void Reset(){
            i_preprevious = null;
            i_previous = null;
            i_firstMoved = false;
            i_next = i_list.i_first;
            CheckNextActive();
        }
      
        protected void CheckNextActive() {
            if (i_next != null) i_next.CheckActive();
        }
      
        public bool HasNext(){
            return i_next != null;
        }

        internal P1ListElement Move(int i) {
            if (i < 0){
                return null;
            }
            for (int i_0_1 = 0; i_0_1 < i; i_0_1++) {
                if (HasNext()){
                    NextElement();
                } else{
                    return null;
                }
            }
            if (HasNext()){
                return NextElement();
            }
            return null;
        }

        internal P1ListElement NextElement() {
            i_preprevious = i_previous;
            i_previous = i_next;
            i_next = i_next.i_next;
            CheckNextActive();
            return i_previous;
        }
      
    }
}