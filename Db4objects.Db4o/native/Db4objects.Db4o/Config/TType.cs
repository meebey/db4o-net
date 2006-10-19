/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using Sharpen.Lang;
using Db4objects.Db4o;

namespace Db4objects.Db4o.Config {

	/// <exclude />
    public class TType : IObjectConstructor {
		
        public void OnActivate(IObjectContainer objectContainer, object obj, object members) {
        }
      
        public Object OnInstantiate(IObjectContainer objectContainer, object obj) {
            try { 
                return Class.ForName((String)obj).GetNetType();
            }  catch (Exception exception) { 
                return null;
            }
        }
      
        public Object OnStore(IObjectContainer objectContainer, object obj) {
            return Class.GetClassForType((System.Type)obj).GetName(); 
        }
      
        public System.Type StoredClass() {
            return typeof(string);
        }
    }
}