/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;

namespace Db4objects.Db4o.Reflect.Net
{
    public class NetArray : Db4objects.Db4o.Reflect.Core.AbstractReflectArray
    {
        public NetArray(IReflector reflector) : base(reflector)
        {
        }
        
        private static Type GetNetType(IReflectClass clazz)
		{
			return ((NetClass)clazz).GetNetType();
		}
        
        public override object NewInstance(IReflectClass componentType, int[] dimensions)
        {
            Type type = GetNetType(componentType);
            return UnfoldArrayCreation(GetArrayType(type, dimensions.Length - 1), dimensions, 0);
        }

        private static object UnfoldArrayCreation(Type type, int[] dimensions, int dimensionIndex)
        {   
            int length = dimensions[dimensionIndex];
            Array array = Array.CreateInstance(type, length);
            if (dimensionIndex == dimensions.Length - 1)
            {
                return array;
            }
            for (int i=0; i<length; ++i)
            {
                object value = UnfoldArrayCreation(type.GetElementType(), dimensions, dimensionIndex + 1);
                array.SetValue(value, i);
            }
            return array;
        }

        private static System.Type GetArrayType(Type type, int dimensions)
        {
			if (dimensions < 1) throw new ArgumentOutOfRangeException("dimensions");
            
            Type arrayType = MakeArrayType(type);
            for (int i=1; i<dimensions; ++i)
            {
                arrayType = MakeArrayType(arrayType);
            }
            return arrayType;
        }

        private static Type MakeArrayType(Type type)
        {
        	return Sharpen.Lang.ArrayTypeReference.MakeArrayType(type, 1);
        }

        public override object NewInstance(IReflectClass componentType, int length)
        {
            return System.Array.CreateInstance(GetNetType(componentType), length);
        }
    }
}
