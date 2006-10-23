/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Reflection;
using Sharpen.Lang;

namespace Db4objects.Db4o {

	/// <exclude />
    public class Dynamic {

        public static object GetProperty(object obj, string prop){
            if(obj != null){
                Type type = TypeForObject(obj);
                try{
                    PropertyInfo pi = type.GetProperty(prop, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    return pi.GetValue(obj,null);
                }catch(Exception e){
                }
            }
            return null;
        }

        public static void SetProperty(object obj, string prop, object val){
            if(obj != null){
                Type type = TypeForObject(obj);
                try{
                    PropertyInfo pi = type.GetProperty(prop, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                    pi.SetValue(obj, val, null);
                }catch(Exception e){
                }
            }
        }

        private static Type TypeForObject(object obj){
            Type type = obj as Type;
            if(type != null){
                return type;
            }
            return obj.GetType();
        }
    }
}
