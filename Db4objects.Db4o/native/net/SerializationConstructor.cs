using System;

namespace Db4objects.Db4o.Reflect.Net
{
	/// <summary>Constructs objects by using System.Runtime.Serialization.FormatterServices.GetUninitializedObject
	/// and bypasses calls to user contructors this way. Not available on CompactFramework
	/// </summary>
	public class SerializationConstructor : Db4objects.Db4o.Reflect.ReflectConstructor
	{
        private Type _type;

		public SerializationConstructor(Type type){
            _type = type;
		}

        public virtual Db4objects.Db4o.Reflect.ReflectClass[] GetParameterTypes() {
            return null;
        }

        public virtual void SetAccessible() {
            // do nothing
        }

        public virtual object NewInstance(object[] parameters) {
            return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(_type);
        }
	}
}
