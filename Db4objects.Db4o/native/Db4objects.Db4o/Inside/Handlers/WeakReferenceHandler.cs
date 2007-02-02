/* Copyright (C) 2004 - 2007   db4objects Inc.   http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Inside.Handlers
{
	internal class WeakReferenceHandler : WeakReference
	{
		public object ObjectReference;

		internal WeakReferenceHandler(Object queue, Object objectRef, Object obj) : base(obj, false){
			this.ObjectReference = objectRef;
			((WeakReferenceHandlerQueue) queue).Add(this);
		}

		public object Get(){
			return this.Target;
		}
	}
}