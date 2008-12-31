/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Typehandlers
{
	/// <summary>Typehandler that ignores all fields on a class</summary>
	public class IgnoreFieldsTypeHandler : ITypeHandler4, IFirstClassHandler
	{
		public virtual void Defragment(IDefragmentContext context)
		{
		}

		// do nothing
		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
		}

		// do nothing
		public virtual object Read(IReadContext context)
		{
			return null;
		}

		public virtual void Write(IWriteContext context, object obj)
		{
		}

		// do nothing
		public virtual IPreparedComparison PrepareComparison(IContext context, object obj
			)
		{
			return null;
		}

		public virtual void CascadeActivation(ActivationContext4 context)
		{
		}

		// do nothing
		public virtual void CollectIDs(QueryingReadContext context)
		{
		}

		// do nothing
		public virtual ITypeHandler4 ReadCandidateHandler(QueryingReadContext context)
		{
			return null;
		}
	}
}
