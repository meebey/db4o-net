/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using Sharpen.Lang;
using Db4objects.Db4o;

namespace Db4objects.Db4o.Config {

	/// <exclude />
    public class TType : ObjectConstructor {
		
        public void OnActivate(ObjectContainer objectContainer, object obj, object members) {
        }
      
        public Object OnInstantiate(ObjectContainer objectContainer, object obj) {
            try { 
                return Class.ForName((String)obj).GetNetType();
            }  catch (Exception exception) { 
                return null;
            }
        }
      
        public Object OnStore(ObjectContainer objectContainer, object obj) {
            return Class.GetClassForType((System.Type)obj).GetName(); 
        }
      
        public System.Type StoredClass() {
            return typeof(string);
        }
    }
}