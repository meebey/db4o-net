/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Internal.CS
{
	public class ClassInfoHelper
	{
		private Hashtable4 _classMetaTable = new Hashtable4();

		private Hashtable4 _genericClassTable = new Hashtable4();

		public virtual ClassInfo GetClassMeta(IReflectClass claxx)
		{
			string className = claxx.GetName();
			if (IsSystemClass(className))
			{
				return ClassInfo.NewSystemClass(className);
			}
			ClassInfo existing = LookupClassMeta(className);
			if (existing != null)
			{
				return existing;
			}
			return NewUserClassMeta(claxx);
		}

		private ClassInfo NewUserClassMeta(IReflectClass claxx)
		{
			ClassInfo classMeta = ClassInfo.NewUserClass(claxx.GetName());
			classMeta.SetSuperClass(MapSuperclass(claxx));
			RegisterClassMeta(claxx.GetName(), classMeta);
			classMeta.SetFields(MapFields(claxx.GetDeclaredFields()));
			return classMeta;
		}

		private ClassInfo MapSuperclass(IReflectClass claxx)
		{
			IReflectClass superClass = claxx.GetSuperclass();
			if (superClass != null)
			{
				return GetClassMeta(superClass);
			}
			return null;
		}

		private FieldInfo[] MapFields(IReflectField[] fields)
		{
			FieldInfo[] fieldsMeta = new FieldInfo[fields.Length];
			for (int i = 0; i < fields.Length; ++i)
			{
				IReflectField field = fields[i];
				bool isArray = field.GetFieldType().IsArray();
				IReflectClass fieldClass = isArray ? field.GetFieldType().GetComponentType() : field
					.GetFieldType();
				bool isPrimitive = fieldClass.IsPrimitive();
				fieldsMeta[i] = new FieldInfo(field.GetName(), GetClassMeta(fieldClass), isPrimitive
					, isArray, false);
			}
			return fieldsMeta;
		}

		private static bool IsSystemClass(string className)
		{
			return className.StartsWith("java");
		}

		private ClassInfo LookupClassMeta(string className)
		{
			return (ClassInfo)_classMetaTable.Get(className);
		}

		private void RegisterClassMeta(string className, ClassInfo classMeta)
		{
			_classMetaTable.Put(className, classMeta);
		}

		public virtual GenericClass ClassMetaToGenericClass(GenericReflector reflector, ClassInfo
			 classMeta)
		{
			if (classMeta.IsSystemClass())
			{
				return (GenericClass)reflector.ForName(classMeta.GetClassName());
			}
			string className = classMeta.GetClassName();
			GenericClass genericClass = LookupGenericClass(className);
			if (genericClass != null)
			{
				return genericClass;
			}
			GenericClass genericSuperClass = null;
			ClassInfo superClassMeta = classMeta.GetSuperClass();
			if (superClassMeta != null)
			{
				genericSuperClass = ClassMetaToGenericClass(reflector, superClassMeta);
			}
			genericClass = new GenericClass(reflector, null, className, genericSuperClass);
			RegisterGenericClass(className, genericClass);
			FieldInfo[] fields = classMeta.GetFields();
			GenericField[] genericFields = new GenericField[fields.Length];
			for (int i = 0; i < fields.Length; ++i)
			{
				ClassInfo fieldClassMeta = fields[i].GetFieldClass();
				string fieldName = fields[i].GetFieldName();
				GenericClass genericFieldClass = ClassMetaToGenericClass(reflector, fieldClassMeta
					);
				genericFields[i] = new GenericField(fieldName, genericFieldClass, fields[i]._isPrimitive
					, fields[i]._isArray, fields[i]._isNArray);
			}
			genericClass.InitFields(genericFields);
			return genericClass;
		}

		private GenericClass LookupGenericClass(string className)
		{
			return (GenericClass)_genericClassTable.Get(className);
		}

		private void RegisterGenericClass(string className, GenericClass classMeta)
		{
			_genericClassTable.Put(className, classMeta);
		}
	}
}
