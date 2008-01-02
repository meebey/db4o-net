/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public abstract class PrimitiveHandler : IIndexableTypeHandler, IBuiltinTypeHandler
	{
		protected readonly ObjectContainerBase _stream;

		protected IReflectClass _classReflector;

		private IReflectClass _primitiveClassReflector;

		public PrimitiveHandler(ObjectContainerBase stream)
		{
			_stream = stream;
		}

		public virtual object Coerce(IReflectClass claxx, object obj)
		{
			return Handlers4.HandlerCanHold(this, claxx) ? obj : No4.INSTANCE;
		}

		public abstract object DefaultValue();

		public virtual void Delete(IDeleteContext context)
		{
			context.Seek(context.Offset() + LinkLength());
		}

		public virtual object IndexEntryToObject(Transaction trans, object indexEntry)
		{
			return indexEntry;
		}

		protected abstract Type PrimitiveJavaClass();

		public abstract object PrimitiveNull();

		/// <param name="mf"></param>
		/// <param name="buffer"></param>
		/// <param name="redirect"></param>
		/// <exception cref="CorruptionException"></exception>
		public virtual object Read(MarshallerFamily mf, StatefulBuffer buffer, bool redirect
			)
		{
			return Read1(buffer);
		}

		/// <exception cref="CorruptionException"></exception>
		internal abstract object Read1(BufferImpl reader);

		public virtual object ReadIndexEntry(BufferImpl buffer)
		{
			try
			{
				return Read1(buffer);
			}
			catch (CorruptionException)
			{
			}
			return null;
		}

		/// <exception cref="CorruptionException"></exception>
		public virtual object ReadIndexEntry(MarshallerFamily mf, StatefulBuffer a_writer
			)
		{
			return Read(mf, a_writer, true);
		}

		public virtual IReflectClass ClassReflector()
		{
			EnsureClassReflectorLoaded();
			return _classReflector;
		}

		public virtual IReflectClass PrimitiveClassReflector()
		{
			EnsureClassReflectorLoaded();
			return _primitiveClassReflector;
		}

		private void EnsureClassReflectorLoaded()
		{
			if (_classReflector != null)
			{
				return;
			}
			_classReflector = _stream.Reflector().ForClass(DefaultValue().GetType());
			Type clazz = PrimitiveJavaClass();
			if (clazz != null)
			{
				_primitiveClassReflector = _stream.Reflector().ForClass(clazz);
			}
		}

		public abstract void Write(object a_object, BufferImpl a_bytes);

		public virtual void WriteIndexEntry(BufferImpl a_writer, object a_object)
		{
			if (a_object == null)
			{
				a_object = PrimitiveNull();
			}
			Write(a_object, a_writer);
		}

		public abstract int LinkLength();

		public void Defragment(IDefragmentContext context)
		{
			context.IncrementOffset(LinkLength());
		}

		public virtual void DefragIndexEntry(DefragmentContextImpl context)
		{
			try
			{
				Read1(context.SourceBuffer());
				Read1(context.TargetBuffer());
			}
			catch (CorruptionException)
			{
				Exceptions4.VirtualException();
			}
		}

		protected virtual Db4objects.Db4o.Internal.Marshall.PrimitiveMarshaller PrimitiveMarshaller
			()
		{
			return MarshallerFamily.Current()._primitive;
		}

		public virtual void Write(IWriteContext context, object obj)
		{
			throw new NotImplementedException();
		}

		public virtual object Read(IReadContext context)
		{
			throw new NotImplementedException();
		}

		public virtual object NullRepresentationInUntypedArrays()
		{
			return PrimitiveNull();
		}

		public virtual IPreparedComparison PrepareComparison(object obj)
		{
			if (obj == null)
			{
				return Null.INSTANCE;
			}
			return InternalPrepareComparison(obj);
		}

		public abstract IPreparedComparison InternalPrepareComparison(object obj);
	}
}
