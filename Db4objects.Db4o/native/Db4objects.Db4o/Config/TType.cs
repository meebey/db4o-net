/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using Db4objects.Db4o.Reflect.Net;
using Sharpen.Lang;
using Db4objects.Db4o;

namespace Db4objects.Db4o.Config {

	/// <exclude />
    public class TType : IObjectConstructor {
		
        public void OnActivate(IObjectContainer objectContainer, object obj, object members) {
        }
      
        public Object OnInstantiate(IObjectContainer objectContainer, object obj) {
            try {
                return TypeReference.FromString((string) obj).Resolve();
            }  catch (Exception exception) { 
                return null;
            }
        }
      
        public Object OnStore(IObjectContainer objectContainer, object obj) {
            return TypeReference.FromType(obj.GetType()).GetUnversionedName();
        }
      
        public System.Type StoredClass() {
            return typeof(string);
        }
    }
}