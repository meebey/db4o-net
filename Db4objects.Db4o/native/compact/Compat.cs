/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Db4objects.Db4o.Reflect;
using Sharpen.Lang;


#if CF_2_0
namespace System
{
	class SerializableAttribute : Attribute
	{
	}
}

namespace Db4objects.Db4o
{
	/// <exclude />
	public class Compat
	{
	}
}
#endif