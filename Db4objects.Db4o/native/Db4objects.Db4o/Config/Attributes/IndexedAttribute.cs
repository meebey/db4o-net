/* Copyright (C) 2006   db4objects Inc.   http://www.db4o.com */

using System;
using System.Reflection;

namespace Db4objects.Db4o.Config.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class IndexedAttribute : Attribute, IDb4oAttribute
	{
		void IDb4oAttribute.Apply(object subject, ConfigurationIntrospector introspector)
		{
			FieldInfo field = subject as FieldInfo;
			if (null == field)
				return;

			introspector.IConfiguration.ObjectClass(field.DeclaringType.FullName).ObjectField(field.Name).Indexed(true);
		}
	}
}
