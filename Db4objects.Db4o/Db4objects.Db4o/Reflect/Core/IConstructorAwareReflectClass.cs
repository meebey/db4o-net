/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Reflect.Core
{
	public interface IConstructorAwareReflectClass : IReflectClass
	{
		/// <summary>
		/// instructs to install or uninstall a special constructor for the
		/// respective platform that avoids calling the constructor for the
		/// respective class
		/// </summary>
		/// <param name="flag">
		/// true to try to install a special constructor, false if
		/// such a constructor is to be removed if present
		/// </param>
		/// <param name="testConstructor">true, if the special constructor shall be tested, false if it shall be set without testing
		/// 	</param>
		/// <returns>true if the special constructor is in place after the call</returns>
		bool SkipConstructor(bool flag, bool testConstructor);
	}
}
