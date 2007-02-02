/* Copyright (C) 2006   db4objects Inc.   http://www.db4o.com */

using System;
using System.Reflection;

using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Config.Attributes
{
	class ConfigurationIntrospector
	{
		Type _type;
		Config4Class _classConfig;
		IConfiguration _config;

		public Type Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public Config4Class ClassConfiguration
		{
			get { return _classConfig; }
			set { _classConfig = value; }
		}

		public IConfiguration IConfiguration
		{
			get { return _config; }
			set { _config = value; }
		}		

		public ConfigurationIntrospector(Type type, Config4Class classConfig, IConfiguration config)
		{
			_type = type;
			_classConfig = classConfig;
			_config = config;
		}

		public void Apply()
		{
			Apply(_type);
			foreach (FieldInfo field in _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				Apply(field);
		}
		
		private void Apply(ICustomAttributeProvider provider)
		{
			foreach (object o in provider.GetCustomAttributes(false))
			{
				IDb4oAttribute attr = o as IDb4oAttribute;
				if (null == attr)
					continue;
				
				attr.Apply(provider, this);
			}
		}
	}
}
