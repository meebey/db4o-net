namespace Db4objects.Db4o.CS
{
	public class FieldMeta
	{
		private string _fieldName;

		private Db4objects.Db4o.CS.ClassMeta _fieldClass;

		public FieldMeta()
		{
		}

		public FieldMeta(string fieldName, Db4objects.Db4o.CS.ClassMeta fieldClass)
		{
			_fieldName = fieldName;
			_fieldClass = fieldClass;
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
