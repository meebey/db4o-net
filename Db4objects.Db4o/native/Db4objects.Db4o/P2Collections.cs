/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using Sharpen.Lang;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o {

   internal class P2Collections : IDb4oCollections {

      internal IExtObjectContainer i_stream;
      
      internal P2Collections(Object a_stream) : base() {
         i_stream = (IExtObjectContainer)a_stream;
      }
      
      public IDb4oList NewLinkedList() {
          lock(i_stream.Lock()){
              if (Unobfuscated.CreateDb4oList(i_stream)){
                  IDb4oList l = new P2LinkedList();
                  i_stream.Set(l);
                  return l;
              }
              return null;
          }
      }
      
      public IDb4oMap NewHashMap(int size) {
          lock(i_stream.Lock()){
              if (Unobfuscated.CreateDb4oList(i_stream)) return new P2HashMap(size);
              return null;
          }
      }

       public IDb4oMap NewIdentityHashMap(int size) {
           lock(i_stream.Lock()){
               if(Unobfuscated.CreateDb4oList(i_stream)){
                   P2HashMap m = new P2HashMap(size);
                   m.i_type = 1;
                   i_stream.Set(m);
                   return m;
               }
               return null;
           }
       }
   }
}