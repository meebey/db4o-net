/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using Sharpen.Lang;
using Db4objects.Db4o;

namespace Db4objects.Db4o.Config
{
	/// <exclude />
	public class TClass : ObjectConstructor
	{
		public void OnActivate(ObjectContainer objectContainer, object obj, object members)
		{
		}

		public Object OnInstantiate(ObjectContainer objectContainer, object obj)
		{
			try
			{
				return Class.ForName((String)obj);
			}
			catch (Exception exception)
			{
				return null;
			}
		}

		public Object OnStore(ObjectContainer objectContainer, object obj)
		{
			return ((Class)obj).GetName();
		}

		public System.Type StoredClass()
		{
			return typeof(string);
		}
	}
}