/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o {

    internal class YapReferenceQueue{

        private List4 list;

        internal void Add(YapRef reference) {
            lock(this){
                list = new List4(list, reference);
            }
        }

        internal void Poll(IExtObjectContainer objectContainer) {
            List4 remove = null;
            lock(this){
                System.Collections.IEnumerator i = new Iterator4Impl(list);
                list = null;
                while(i.MoveNext()){
                    YapRef yapRef = (YapRef)i.Current;
                    if(yapRef.IsAlive){
                        list = new List4(list, yapRef);
                    }else{
                        remove = new List4(remove, yapRef.yapObject);
                    }
                }
            }
            System.Collections.IEnumerator j = new Iterator4Impl(remove);
            while(j.MoveNext() && (!objectContainer.IsClosed())){
                objectContainer.Purge(j.Current);
            }
        }
    }
}