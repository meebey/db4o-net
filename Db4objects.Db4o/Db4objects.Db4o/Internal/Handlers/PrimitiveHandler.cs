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

		private bool i_compareToIsNull;

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
		internal abstract object Read1(Db4objects.Db4o.Internal.Buffer reader);

		public virtual object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer buffer)
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

		public abstract void Write(object a_object, Db4objects.Db4o.Internal.Buffer a_bytes
			);

		public virtual void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer a_writer, object
			 a_object)
		{
			if (a_object == null)
			{
				a_object = PrimitiveNull();
			}
			Write(a_object, a_writer);
		}

		public virtual IComparable4 PrepareComparison(object obj)
		{
			if (obj == null)
			{
				i_compareToIsNull = true;
				return Null.INSTANCE;
			}
			i_compareToIsNull = false;
			PrepareComparison1(obj);
			return this;
		}

		internal abstract void PrepareComparison1(object obj);

		public virtual int CompareTo(object obj)
		{
			if (i_compareToIsNull)
			{
				if (obj == null)
				{
					return 0;
				}
				return 1;
			}
			if (obj == null)
			{
				return -1;
			}
			if (IsEqual1(obj))
			{
				return 0;
			}
			if (IsGreater1(obj))
			{
				return 1;
			}
			return -1;
		}

		internal abstract bool IsEqual1(object obj);

		internal abstract bool IsGreater1(object obj);

		internal abstract bool IsSmaller1(object obj);

		public abstract int LinkLength();

		public void Defragment(DefragmentContext context)
		{
			int linkLength = LinkLength();
			context.Readers().IncrementOffset(linkLength);
		}

		public virtual void DefragIndexEntry(BufferPair readers)
		{
			try
			{
				Read1(readers.Source());
				Read1(readers.Target());
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
	}
}
