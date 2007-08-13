/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public abstract class PrimitiveHandler : IIndexableTypeHandler
	{
		protected readonly ObjectContainerBase _stream;

		protected IReflectClass _classReflector;

		private IReflectClass _primitiveClassReflector;

		public PrimitiveHandler(ObjectContainerBase stream)
		{
			_stream = stream;
		}

		private bool i_compareToIsNull;

		public virtual void CascadeActivation(Transaction a_trans, object a_object, int a_depth
			, bool a_activate)
		{
		}

		public virtual object Coerce(IReflectClass claxx, object obj)
		{
			return Handlers4.HandlerCanHold(this, claxx) ? obj : No4.INSTANCE;
		}

		public abstract object DefaultValue();

		public virtual void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer a_bytes)
		{
			a_bytes.IncrementOffset(LinkLength());
		}

		public virtual bool HasFixedLength()
		{
			return true;
		}

		public virtual object IndexEntryToObject(Transaction trans, object indexEntry)
		{
			return indexEntry;
		}

		public virtual void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, bool topLevel, object obj, bool withIndirection)
		{
			if (topLevel)
			{
				header.AddBaseLength(LinkLength());
			}
			else
			{
				header.AddPayLoadLength(LinkLength());
			}
		}

		protected abstract Type PrimitiveJavaClass();

		public abstract object PrimitiveNull();

		public virtual object ReadQuery(Transaction trans, MarshallerFamily mf, bool withRedirection
			, Db4objects.Db4o.Internal.Buffer reader, bool toArray)
		{
			return Read1(reader);
		}

		public virtual object Read(MarshallerFamily mf, StatefulBuffer buffer, bool redirect
			)
		{
			return Read1(buffer);
		}

		internal abstract object Read1(Db4objects.Db4o.Internal.Buffer reader);

		public virtual QCandidate ReadSubCandidate(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates, bool withIndirection)
		{
			try
			{
				object obj = ReadQuery(candidates.i_trans, mf, withIndirection, reader, true);
				if (obj != null)
				{
					return new QCandidate(candidates, obj, 0, true);
				}
			}
			catch (CorruptionException)
			{
			}
			return null;
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer a_reader)
		{
			try
			{
				return Read1(a_reader);
			}
			catch (CorruptionException)
			{
			}
			return null;
		}

		public virtual object ReadIndexEntry(MarshallerFamily mf, StatefulBuffer a_writer
			)
		{
			return Read(mf, a_writer, true);
		}

		public virtual IReflectClass ClassReflector()
		{
			if (_classReflector != null)
			{
				return _classReflector;
			}
			_classReflector = _stream.Reflector().ForClass(DefaultValue().GetType());
			Type clazz = PrimitiveJavaClass();
			if (clazz != null)
			{
				_primitiveClassReflector = _stream.Reflector().ForClass(clazz);
			}
			return _classReflector;
		}

		/// <summary>classReflector() has to be called first, before this returns a value</summary>
		public virtual IReflectClass PrimitiveClassReflector()
		{
			return _primitiveClassReflector;
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

		public virtual object Write(MarshallerFamily mf, object a_object, bool topLevel, 
			StatefulBuffer a_bytes, bool withIndirection, bool restoreLinkeOffset)
		{
			if (a_object == null)
			{
				a_object = PrimitiveNull();
			}
			Write(a_object, a_bytes);
			return a_object;
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

		public void Defrag(MarshallerFamily mf, BufferPair readers, bool redirect)
		{
			int linkLength = LinkLength();
			readers.IncrementOffset(linkLength);
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

		public abstract int GetID();
	}
}
