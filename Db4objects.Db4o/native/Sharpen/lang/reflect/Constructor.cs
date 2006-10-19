/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Reflection;
using Sharpen.Lang;

namespace Sharpen.Lang.Reflect {

    public class Constructor {

        private ConstructorInfo constructorInfo;

        internal Constructor(ConstructorInfo constructorInfo) {
            this.constructorInfo = constructorInfo;
        }

        public Class[] GetParameterTypes() {
            ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
            Class[] classes = new Class[parameterInfos.Length];
            for(int i = 0; i < parameterInfos.Length; i++) {
                classes[i] = Class.GetClassForType(parameterInfos[i].ParameterType);
            }
            return classes;
        }

        public Object NewInstance(Object[] parameters) {
            return constructorInfo.Invoke(parameters);
        }
    }
}
