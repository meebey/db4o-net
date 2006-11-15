/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Globalization;
using Sharpen.Lang;
using Db4objects.Db4o;

namespace Db4objects.Db4o.Config
{
	/// <exclude />
	public class TCultureInfo : IObjectConstructor
	{
		public Object OnInstantiate(IObjectContainer store, object stored)
		{
			return CultureInfo.GetCultureInfo((int) stored);
		}

		public Object OnStore(IObjectContainer store, object obj)
		{
			CultureInfo culture = (CultureInfo) obj;
			return culture.LCID;
		}

		public void OnActivate(IObjectContainer container, object applicationObject, object storedObject)
		{
		}

		public Type StoredClass()
		{
			return typeof(int);
		}
	}
}
