namespace Db4objects.Db4o.CS
{
	public class ClassMeta
	{
		public static Db4objects.Db4o.CS.ClassMeta NewSystemClass(string className)
		{
			return new Db4objects.Db4o.CS.ClassMeta(className, true);
		}

		public static Db4objects.Db4o.CS.ClassMeta NewUserClass(string className)
		{
			return new Db4objects.Db4o.CS.ClassMeta(className, false);
		}

		private string _className;

		private bool _isSystemClass;

		private Db4objects.Db4o.CS.ClassMeta _superClass;

		private Db4objects.Db4o.CS.FieldMeta[] _fields;

		public ClassMeta()
		{
		}

		private ClassMeta(string className, bool systemClass)
		{
			_className = className;
			_isSystemClass = systemClass;
		}

		public virtual Db4objects.Db4o.CS.FieldMeta[] GetFields()
		{
			return _fields;
		}

		public virtual void SetFields(Db4objects.Db4o.CS.FieldMeta[] fields)
		{
			this._fields = fields;
		}

		public virtual Db4objects.Db4o.CS.ClassMeta GetSuperClass()
		{
			return _superClass;
		}

		public virtual void SetSuperClass(Db4objects.Db4o.CS.ClassMeta superClass)
		{
			this._superClass = superClass;
		}

		public virtual string GetClassName()
		{
			return _className;
		}

		public virtual bool IsSystemClass()
		{
			return _isSystemClass;
		}
	}
}
