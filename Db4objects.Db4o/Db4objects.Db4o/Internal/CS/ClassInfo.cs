namespace Db4objects.Db4o.Internal.CS
{
	public class ClassInfo
	{
		public static Db4objects.Db4o.Internal.CS.ClassInfo NewSystemClass(string className
			)
		{
			return new Db4objects.Db4o.Internal.CS.ClassInfo(className, true);
		}

		public static Db4objects.Db4o.Internal.CS.ClassInfo NewUserClass(string className
			)
		{
			return new Db4objects.Db4o.Internal.CS.ClassInfo(className, false);
		}

		public string _className;

		public bool _isSystemClass;

		public Db4objects.Db4o.Internal.CS.ClassInfo _superClass;

		public Db4objects.Db4o.Internal.CS.FieldInfo[] _fields;

		public ClassInfo()
		{
		}

		private ClassInfo(string className, bool systemClass)
		{
			_className = className;
			_isSystemClass = systemClass;
		}

		public virtual Db4objects.Db4o.Internal.CS.FieldInfo[] GetFields()
		{
			return _fields;
		}

		public virtual void SetFields(Db4objects.Db4o.Internal.CS.FieldInfo[] fields)
		{
			this._fields = fields;
		}

		public virtual Db4objects.Db4o.Internal.CS.ClassInfo GetSuperClass()
		{
			return _superClass;
		}

		public virtual void SetSuperClass(Db4objects.Db4o.Internal.CS.ClassInfo superClass
			)
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
