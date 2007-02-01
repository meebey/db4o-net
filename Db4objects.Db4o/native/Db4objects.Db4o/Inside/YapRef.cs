/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Inside
{
	internal class YapRef : WeakReference
	{
		public object yapObject;

		internal YapRef(Object queue, Object yapObject, Object obj) : base(obj, false){
			this.yapObject = yapObject;
			((YapReferenceQueue) queue).Add(this);
		}

		public object Get(){
			return this.Target;
		}
	}
}