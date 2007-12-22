/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;

namespace Db4objects.Db4o.Internal.Handlers
{
	/// <exclude></exclude>
	public abstract class NetTypeHandler : PrimitiveHandler, INetType
	{
		public NetTypeHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		private int i_linkLength;

		private object i_compareTo;

		public abstract int Compare(object compare, object with);

		public virtual string DotNetClassName()
		{
			string className = this.GetType().FullName;
			int pos = className.IndexOf(".Net");
			if (pos >= 0)
			{
				return "System." + Sharpen.Runtime.Substring(className, pos + 4) + ", mscorlib";
			}
			return DefaultValue().GetType().FullName;
		}

		public abstract bool IsEqual(object compare, object with);

		public virtual void Initialize()
		{
			byte[] bytes = new byte[65];
			for (int i = 0; i < bytes.Length; i++)
			{
				bytes[i] = 55;
			}
			Write(PrimitiveNull(), bytes, 0);
			for (int i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] == 55)
				{
					i_linkLength = i;
					break;
				}
			}
		}

		public virtual int GetID()
		{
			return TypeID();
		}

		public virtual string GetName()
		{
			return DotNetClassName();
		}

		public override int LinkLength()
		{
			return i_linkLength;
		}

		protected override Type PrimitiveJavaClass()
		{
			return null;
		}

		public override object PrimitiveNull()
		{
			return DefaultValue();
		}

		public abstract object Read(byte[] bytes, int offset);

		/// <exception cref="CorruptionException"></exception>
		internal override object Read1(BufferImpl a_bytes)
		{
			int offset = a_bytes._offset;
			object ret = Read(a_bytes._buffer, a_bytes._offset);
			a_bytes._offset = offset + LinkLength();
			return ret;
		}

		public abstract int TypeID();

		public abstract void Write(object obj, byte[] bytes, int offset);

		public override void Write(object a_object, BufferImpl a_bytes)
		{
			int offset = a_bytes._offset;
			if (a_object != null)
			{
				Write(a_object, a_bytes._buffer, a_bytes._offset);
			}
			a_bytes._offset = offset + LinkLength();
		}

		internal override void PrepareComparison1(object obj)
		{
			i_compareTo = obj;
		}

		internal override bool IsEqual1(object obj)
		{
			return IsEqual(i_compareTo, obj);
		}

		internal override bool IsGreater1(object obj)
		{
			if (ClassReflector().IsInstance(obj) && !IsEqual(i_compareTo, obj))
			{
				return Compare(i_compareTo, obj) > 0;
			}
			return false;
		}

		internal override bool IsSmaller1(object obj)
		{
			if (ClassReflector().IsInstance(obj) && !IsEqual(i_compareTo, obj))
			{
				return Compare(i_compareTo, obj) < 0;
			}
			return false;
		}

		public override IPreparedComparison InternalPrepareComparison(object obj)
		{
			throw new NotImplementedException();
		}
	}
}
