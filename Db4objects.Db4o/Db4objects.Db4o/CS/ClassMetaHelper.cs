namespace Db4objects.Db4o.CS
{
	public class ClassMetaHelper
	{
		private Db4objects.Db4o.Foundation.Hashtable4 _classMetaTable = new Db4objects.Db4o.Foundation.Hashtable4
			();

		private Db4objects.Db4o.Foundation.Hashtable4 _genericClassTable = new Db4objects.Db4o.Foundation.Hashtable4
			();

		public virtual Db4objects.Db4o.CS.ClassMeta GetClassMeta(System.Type claxx)
		{
			string className = claxx.FullName;
			if (IsSystemClass(className))
			{
				return Db4objects.Db4o.CS.ClassMeta.NewSystemClass(className);
			}
			Db4objects.Db4o.CS.ClassMeta existing = LookupClassMeta(className);
			if (existing != null)
			{
				return existing;
			}
			return NewUserClassMeta(claxx);
		}

		private Db4objects.Db4o.CS.ClassMeta NewUserClassMeta(System.Type claxx)
		{
			Db4objects.Db4o.CS.ClassMeta classMeta = Db4objects.Db4o.CS.ClassMeta.NewUserClass
				(claxx.FullName);
			classMeta.SetSuperClass(MapSuperclass(claxx));
			RegisterClassMeta(claxx.FullName, classMeta);
			classMeta.SetFields(MapFields(Sharpen.Runtime.GetDeclaredFields(claxx)));
			return classMeta;
		}

		private Db4objects.Db4o.CS.ClassMeta MapSuperclass(System.Type claxx)
		{
			System.Type superClass = claxx.BaseType;
			if (superClass != null && superClass != typeof(object))
			{
				return GetClassMeta(superClass);
			}
			return null;
		}

		private Db4objects.Db4o.CS.FieldMeta[] MapFields(System.Reflection.FieldInfo[] fields
			)
		{
			Db4objects.Db4o.CS.FieldMeta[] fieldsMeta = new Db4objects.Db4o.CS.FieldMeta[fields
				.Length];
			for (int i = 0; i < fields.Length; ++i)
			{
				System.Reflection.FieldInfo field = fields[i];
				fieldsMeta[i] = new Db4objects.Db4o.CS.FieldMeta(field.Name, GetClassMeta(field.GetType
					()));
			}
			return fieldsMeta;
		}

		private static bool IsSystemClass(string className)
		{
			return className.StartsWith("java");
		}

		private Db4objects.Db4o.CS.ClassMeta LookupClassMeta(string className)
		{
			return (Db4objects.Db4o.CS.ClassMeta)_classMetaTable.Get(className);
		}

		private void RegisterClassMeta(string className, Db4objects.Db4o.CS.ClassMeta classMeta
			)
		{
			_classMetaTable.Put(className, classMeta);
		}

		public virtual Db4objects.Db4o.Reflect.Generic.GenericClass ClassMetaToGenericClass
			(Db4objects.Db4o.Reflect.Generic.GenericReflector reflector, Db4objects.Db4o.CS.ClassMeta
			 classMeta)
		{
			if (classMeta.IsSystemClass())
			{
				return (Db4objects.Db4o.Reflect.Generic.GenericClass)reflector.ForName(classMeta.
					GetClassName());
			}
			string className = classMeta.GetClassName();
			Db4objects.Db4o.Reflect.Generic.GenericClass genericClass = LookupGenericClass(className
				);
			if (genericClass != null)
			{
				return genericClass;
			}
			Db4objects.Db4o.Reflect.Generic.GenericClass genericSuperClass = null;
			Db4objects.Db4o.CS.ClassMeta superClassMeta = classMeta.GetSuperClass();
			if (superClassMeta != null)
			{
				genericSuperClass = ClassMetaToGenericClass(reflector, superClassMeta);
				RegisterGenericClass(superClassMeta.GetClassName(), genericSuperClass);
			}
			genericClass = new Db4objects.Db4o.Reflect.Generic.GenericClass(reflector, null, 
				className, genericSuperClass);
			RegisterGenericClass(className, genericClass);
			Db4objects.Db4o.CS.FieldMeta[] fields = classMeta.GetFields();
			Db4objects.Db4o.Reflect.Generic.GenericField[] genericFields = new Db4objects.Db4o.Reflect.Generic.GenericField
				[fields.Length];
			for (int i = 0; i < fields.Length; ++i)
			{
				Db4objects.Db4o.CS.ClassMeta fieldClassMeta = fields[i].GetFieldClass();
				string fieldName = fields[i].GetFieldName();
				Db4objects.Db4o.Reflect.Generic.GenericClass genericFieldClass = ClassMetaToGenericClass
					(reflector, fieldClassMeta);
				genericFields[i] = new Db4objects.Db4o.Reflect.Generic.GenericField(fieldName, genericFieldClass
					, false, false, false);
			}
			genericClass.InitFields(genericFields);
			return genericClass;
		}

		private Db4objects.Db4o.Reflect.Generic.GenericClass LookupGenericClass(string className
			)
		{
			return (Db4objects.Db4o.Reflect.Generic.GenericClass)_genericClassTable.Get(className
				);
		}

		private void RegisterGenericClass(string className, Db4objects.Db4o.Reflect.Generic.GenericClass
			 classMeta)
		{
			_genericClassTable.Put(className, classMeta);
		}
	}
}
