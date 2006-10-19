/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Reflection;

namespace Sharpen.Lang.Reflect
{
    public class Method
	{
        private MethodInfo _methodInfo;

        internal Method(MethodInfo methodInfo)
		{
            this._methodInfo = methodInfo;
        }

		internal MethodInfo MethodInfo
		{
			get { return _methodInfo; }
		}

        public Object Invoke(Object obj, Object[] args)
		{
            return _methodInfo.Invoke(obj, args);
        }

        public String GetName()
		{
            return _methodInfo.Name;
        }

		public Sharpen.Lang.Class[] GetParameterTypes() 
		{
			ParameterInfo[] parameters = _methodInfo.GetParameters();
			Sharpen.Lang.Class[] types = new Sharpen.Lang.Class[parameters.Length];
			for (int i=0; i<parameters.Length; ++i)
			{
				types[i] = Sharpen.Lang.Class.GetClassForType(parameters[i].ParameterType);
			}
			return types;
		}

		public Sharpen.Lang.Class GetReturnType() 
		{
			return Sharpen.Lang.Class.GetClassForType(_methodInfo.ReturnType);
		}
    }
}

