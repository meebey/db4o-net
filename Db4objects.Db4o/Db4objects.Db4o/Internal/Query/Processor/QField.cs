/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Types;

namespace Db4objects.Db4o.Internal.Query.Processor
{
	/// <exclude></exclude>
	public class QField : IVisitor4, IUnversioned
	{
		[System.NonSerialized]
		internal Transaction i_trans;

		public string i_name;

		[System.NonSerialized]
		internal FieldMetadata i_yapField;

		public int i_yapClassID;

		public int i_index;

		public QField()
		{
		}

		public QField(Transaction a_trans, string name, FieldMetadata a_yapField, int a_yapClassID
			, int a_index)
		{
			// C/S only	
			i_trans = a_trans;
			i_name = name;
			i_yapField = a_yapField;
			i_yapClassID = a_yapClassID;
			i_index = a_index;
			if (i_yapField != null)
			{
				if (!i_yapField.Alive())
				{
					i_yapField = null;
				}
			}
		}

		internal virtual bool CanHold(IReflectClass claxx)
		{
			return i_yapField == null || i_yapField.CanHold(claxx);
		}

		internal virtual object Coerce(object a_object)
		{
			IReflectClass claxx = null;
			IReflector reflector = i_trans.Reflector();
			if (a_object != null)
			{
				if (a_object is IReflectClass)
				{
					claxx = (IReflectClass)a_object;
				}
				else
				{
					claxx = reflector.ForObject(a_object);
				}
			}
			else
			{
				return a_object;
			}
			if (i_yapField == null)
			{
				return a_object;
			}
			return i_yapField.Coerce(claxx, a_object);
		}

		internal virtual ClassMetadata GetYapClass()
		{
			if (i_yapField != null)
			{
				return i_yapField.HandlerClassMetadata(i_trans.Container());
			}
			return null;
		}

		internal virtual FieldMetadata GetYapField(ClassMetadata yc)
		{
			if (i_yapField != null)
			{
				return i_yapField;
			}
			FieldMetadata yf = yc.FieldMetadataForName(i_name);
			if (yf != null)
			{
				yf.Alive();
			}
			return yf;
		}

		public virtual FieldMetadata GetYapField()
		{
			return i_yapField;
		}

		internal virtual bool IsArray()
		{
			return i_yapField != null && i_yapField.GetHandler() is ArrayHandler;
		}

		internal virtual bool IsClass()
		{
			return i_yapField == null || Handlers4.HandlesClass(i_yapField.GetHandler());
		}

		internal virtual bool IsSimple()
		{
			return i_yapField != null && Handlers4.HandlesSimple(i_yapField.GetHandler());
		}

		internal virtual IPreparedComparison PrepareComparison(object obj)
		{
			if (i_yapField != null)
			{
				return i_yapField.PrepareComparison(obj);
			}
			if (obj == null)
			{
				return Null.Instance;
			}
			ClassMetadata yc = i_trans.Container().ProduceClassMetadata(i_trans.Reflector().ForObject
				(obj));
			FieldMetadata yf = yc.FieldMetadataForName(i_name);
			if (yf != null)
			{
				return yf.PrepareComparison(obj);
			}
			return null;
		}

		internal virtual void Unmarshall(Transaction a_trans)
		{
			if (i_yapClassID != 0)
			{
				ClassMetadata yc = a_trans.Container().ClassMetadataForId(i_yapClassID);
				i_yapField = yc.i_fields[i_index];
			}
		}

		public virtual void Visit(object obj)
		{
			((QCandidate)obj).UseField(this);
		}

		public override string ToString()
		{
			if (i_yapField != null)
			{
				return "QField " + i_yapField.ToString();
			}
			return base.ToString();
		}
	}
}
