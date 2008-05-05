/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public abstract class PrimitiveHandler : IIndexableTypeHandler, IBuiltinTypeHandler
		, IEmbeddedTypeHandler
	{
		protected IReflectClass _classReflector;

		private IReflectClass _primitiveClassReflector;

		private object _primitiveNull;

		public virtual object Coerce(IReflector reflector, IReflectClass claxx, object obj
			)
		{
			return Handlers4.HandlerCanHold(this, reflector, claxx) ? obj : No4.Instance;
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

		protected virtual Type JavaClass()
		{
			if (NullableArrayHandling.Disabled())
			{
				return DefaultValue().GetType();
			}
			return Platform4.NullableTypeFor(PrimitiveJavaClass());
		}

		protected virtual object PrimitiveNull()
		{
			if (_primitiveNull == null)
			{
				IReflectClass claxx = (_primitiveClassReflector == null ? _classReflector : _primitiveClassReflector
					);
				_primitiveNull = claxx.NullValue();
			}
			return _primitiveNull;
		}

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
		internal abstract object Read1(ByteArrayBuffer reader);

		public virtual object ReadIndexEntry(ByteArrayBuffer buffer)
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
			return _classReflector;
		}

		public virtual IReflectClass PrimitiveClassReflector()
		{
			return _primitiveClassReflector;
		}

		public virtual void RegisterReflector(IReflector reflector)
		{
			_classReflector = reflector.ForClass(JavaClass());
			Type clazz = PrimitiveJavaClass();
			if (clazz != null)
			{
				_primitiveClassReflector = reflector.ForClass(clazz);
			}
		}

		public abstract void Write(object a_object, ByteArrayBuffer a_bytes);

		public virtual void WriteIndexEntry(ByteArrayBuffer a_writer, object a_object)
		{
			if (a_object == null)
			{
				a_object = PrimitiveNull();
			}
			Write(a_object, a_writer);
		}

		// redundant, only added to make Sun JDK 1.2's java happy :(
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

		public virtual IPreparedComparison PrepareComparison(IContext context, object obj
			)
		{
			if (obj == null)
			{
				return Null.Instance;
			}
			return InternalPrepareComparison(obj);
		}

		public abstract IPreparedComparison InternalPrepareComparison(object obj);
	}
}
