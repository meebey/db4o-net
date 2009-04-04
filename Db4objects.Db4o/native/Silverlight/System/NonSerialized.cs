/* Copyright (C) 2009   db4objects Inc.   http://www.db4o.com */

using Db4objects.Db4o;

#if SILVERLIGHT
namespace System
{
	public class NonSerialized : TransientAttribute
	{
	}
}
#endif