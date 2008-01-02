/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <summary>
	/// Common base class for StringHandler and ArrayHandler:
	/// The common pattern for both is that a slot  is one indirection in the database file to this.
	/// </summary>
	/// <remarks>
	/// Common base class for StringHandler and ArrayHandler:
	/// The common pattern for both is that a slot  is one indirection in the database file to this.
	/// </remarks>
	/// <exclude></exclude>
	public abstract class VariableLengthTypeHandler : ITypeHandler4
	{
		private readonly ObjectContainerBase _container;

		public VariableLengthTypeHandler(ObjectContainerBase container)
		{
			_container = container;
		}

		public int LinkLength()
		{
			return Const4.INT_LENGTH + Const4.ID_LENGTH;
		}

		public abstract void Defragment(IDefragmentContext context);

		public virtual ObjectContainerBase Container()
		{
			return _container;
		}

		protected virtual BufferImpl ReadIndirectedBuffer(IReadContext readContext)
		{
			IInternalReadContext context = (IInternalReadContext)readContext;
			int address = context.ReadInt();
			int length = context.ReadInt();
			if (address == 0)
			{
				return null;
			}
			return context.Container().BufferByAddress(address, length);
		}

		public abstract IPreparedComparison PrepareComparison(object arg1);

		public abstract void Delete(IDeleteContext arg1);

		public abstract object Read(IReadContext arg1);

		public abstract void Write(IWriteContext arg1, object arg2);
	}
}
