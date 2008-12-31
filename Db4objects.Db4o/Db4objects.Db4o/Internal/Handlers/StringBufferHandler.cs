/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Text;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal.Handlers
{
	public sealed class StringBufferHandler : ITypeHandler4, IBuiltinTypeHandler, ISecondClassTypeHandler
		, IVariableLengthTypeHandler, IEmbeddedTypeHandler
	{
		private IReflectClass _classReflector;

		public void Defragment(IDefragmentContext context)
		{
			StringHandler(context).Defragment(context);
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		public void Delete(IDeleteContext context)
		{
			StringHandler(context).Delete(context);
		}

		public object Read(IReadContext context)
		{
			object read = StringHandler(context).Read(context);
			if (null == read)
			{
				return null;
			}
			return new StringBuilder((string)read);
		}

		public void Write(IWriteContext context, object obj)
		{
			StringHandler(context).Write(context, obj.ToString());
		}

		private ITypeHandler4 StringHandler(IContext context)
		{
			return Handlers(context)._stringHandler;
		}

		private HandlerRegistry Handlers(IContext context)
		{
			return ((IInternalObjectContainer)context.ObjectContainer()).Handlers();
		}

		public IPreparedComparison PrepareComparison(IContext context, object obj)
		{
			return StringHandler(context).PrepareComparison(context, obj);
		}

		public IReflectClass ClassReflector()
		{
			return _classReflector;
		}

		public void RegisterReflector(IReflector reflector)
		{
			_classReflector = reflector.ForClass(typeof(StringBuilder));
		}
	}
}
