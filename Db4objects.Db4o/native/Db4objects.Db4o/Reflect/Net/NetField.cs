using System.Collections;
using System.Reflection;

namespace Db4objects.Db4o.Reflect.Net
{
	public class NetField : Db4objects.Db4o.Reflect.IReflectField
	{
		private readonly Db4objects.Db4o.Reflect.IReflector reflector;

		private readonly System.Reflection.FieldInfo field;

        private static IList _transientMarkers;

		public NetField(Db4objects.Db4o.Reflect.IReflector reflector, System.Reflection.FieldInfo field
			)
		{
			this.reflector = reflector;
			this.field = field;
		}

		public virtual string GetName()
		{
			return field.Name;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass GetFieldType()
		{
			return reflector.ForClass(field.FieldType);
		}

		public virtual bool IsPublic()
		{
			return field.IsPublic;
		}

		public virtual bool IsStatic()
		{
			return field.IsStatic;
		}

		public virtual bool IsTransient()
		{
            return IsTransient(field);
		}

		public virtual void SetAccessible()
		{	
		}

		public virtual object Get(object onObject)
		{
			try
			{
				return field.GetValue(onObject);
			}
			catch
			{
				return null;
			}
		}

		public virtual void Set(object onObject, object attribute)
		{
			try
			{
				field.SetValue(onObject, attribute);
			}
			catch
			{
			}
		}

        public static bool IsTransient(FieldInfo field)
        {
            if (field.IsNotSerialized) return true;
            return CheckForTransient(field.GetCustomAttributes(true));
        }

        private static bool CheckForTransient(object[] attributes)
        {   
            if (attributes == null) return false;

            foreach (object attribute in attributes)
            {
                string attributeName = attribute.ToString();
                if ("Db4objects.Db4o.Transient" == attributeName) return true;
                if (_transientMarkers == null) continue;
                if (_transientMarkers.Contains(attributeName)) return true;
            }
            return false;
        }

        public static void MarkTransient(string attributeName)
        {
            if (_transientMarkers == null)
            {
                _transientMarkers = new ArrayList();
            }
            else if (_transientMarkers.Contains(attributeName))
            {
                return;
            }
            _transientMarkers.Add(attributeName);
        }
	}
}
