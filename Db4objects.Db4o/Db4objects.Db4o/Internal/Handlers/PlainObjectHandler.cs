/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Reflect;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <summary>Tyehandler for naked plain objects (java.lang.Object).</summary>
	/// <remarks>Tyehandler for naked plain objects (java.lang.Object).</remarks>
	public class PlainObjectHandler : IReferenceTypeHandler
	{
		public virtual void Defragment(IDefragmentContext context)
		{
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
		}

		public virtual void Activate(IReferenceActivationContext context)
		{
		}

		public virtual void Write(IWriteContext context, object obj)
		{
		}

		public virtual bool CanHold(IReflectClass type)
		{
			return ReflectClasses.AreEqual(typeof(object), type);
		}
	}
}
