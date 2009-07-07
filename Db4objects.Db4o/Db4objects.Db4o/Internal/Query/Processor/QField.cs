/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Marshall;
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
		internal FieldMetadata _fieldMetadata;

		public int i_classMetadataID;

		public int _fieldHandle;

		public QField()
		{
		}

		public QField(Transaction a_trans, string name, FieldMetadata fieldMetadata, int 
			classMetadataID, int a_index)
		{
			// C/S only	
			i_trans = a_trans;
			i_name = name;
			_fieldMetadata = fieldMetadata;
			i_classMetadataID = classMetadataID;
			_fieldHandle = a_index;
			if (_fieldMetadata != null)
			{
				if (!_fieldMetadata.Alive())
				{
					_fieldMetadata = null;
				}
			}
		}

		internal virtual bool CanHold(IReflectClass claxx)
		{
			return _fieldMetadata == null || _fieldMetadata.CanHold(claxx);
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
				// TODO: Review this line for NullableArrayHandling 
				return a_object;
			}
			if (_fieldMetadata == null)
			{
				return a_object;
			}
			return _fieldMetadata.Coerce(claxx, a_object);
		}

		internal virtual ClassMetadata GetYapClass()
		{
			if (_fieldMetadata != null)
			{
				return _fieldMetadata.FieldType();
			}
			return null;
		}

		internal virtual FieldMetadata GetYapField(ClassMetadata yc)
		{
			if (_fieldMetadata != null)
			{
				return _fieldMetadata;
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
			return _fieldMetadata;
		}

		internal virtual bool IsArray()
		{
			return _fieldMetadata != null && Handlers4.HandlesArray(_fieldMetadata.GetHandler
				());
		}

		internal virtual bool IsClass()
		{
			return _fieldMetadata == null || Handlers4.HandlesClass(_fieldMetadata.GetHandler
				());
		}

		internal virtual bool IsSimple()
		{
			return _fieldMetadata != null && Handlers4.HandlesSimple(_fieldMetadata.GetHandler
				());
		}

		internal virtual IPreparedComparison PrepareComparison(IContext context, object obj
			)
		{
			if (_fieldMetadata != null)
			{
				return _fieldMetadata.PrepareComparison(context, obj);
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
				return yf.PrepareComparison(context, obj);
			}
			return null;
		}

		internal virtual void Unmarshall(Transaction a_trans)
		{
			if (i_classMetadataID != 0)
			{
				ClassMetadata yc = a_trans.Container().ClassMetadataForID(i_classMetadataID);
				_fieldMetadata = (FieldMetadata)yc._aspects[_fieldHandle];
			}
		}

		public virtual void Visit(object obj)
		{
			((QCandidate)obj).UseField(this);
		}

		public override string ToString()
		{
			if (_fieldMetadata != null)
			{
				return "QField " + _fieldMetadata.ToString();
			}
			return base.ToString();
		}
	}
}
