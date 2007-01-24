namespace Db4objects.Db4o.CS
{
	public class FieldMeta
	{
		public string _fieldName;

		public Db4objects.Db4o.CS.ClassMeta _fieldClass;

		public bool _isPrimitive;

		public bool _isArray;

		public bool _isNArray;

		public FieldMeta()
		{
		}

		public FieldMeta(string fieldName, Db4objects.Db4o.CS.ClassMeta fieldClass, bool 
			isPrimitive, bool isArray, bool isNArray)
		{
			_fieldName = fieldName;
			_fieldClass = fieldClass;
			_isPrimitive = isPrimitive;
			_isArray = isArray;
			_isNArray = isNArray;
		}

		public virtual Db4objects.Db4o.CS.ClassMeta GetFieldClass()
		{
			return _fieldClass;
		}

		public virtual string GetFieldName()
		{
			return _fieldName;
		}
	}
}
