/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	internal class ObjectAnalyzer
	{
		private readonly PartialObjectContainer _container;

		private readonly object _obj;

		private Db4objects.Db4o.Internal.ClassMetadata _classMetadata;

		private Db4objects.Db4o.Internal.ObjectReference _ref;

		private bool _notStorable;

		internal ObjectAnalyzer(PartialObjectContainer container, object obj)
		{
			_container = container;
			_obj = obj;
			_ref = _container.ReferenceForObject(_obj);
			if (_ref == null)
			{
				IReflectClass claxx = _container.Reflector().ForObject(_obj);
				if (claxx == null)
				{
					NotStorable(_obj, claxx);
					return;
				}
				if (!DetectClassMetadata(claxx))
				{
					return;
				}
				IReflectClass substituteClass = _classMetadata.ClassSubstitute();
				if (substituteClass != null)
				{
					if (!DetectClassMetadata(substituteClass))
					{
						return;
					}
				}
			}
			else
			{
				_classMetadata = _ref.GetYapClass();
			}
			if (IsPlainObjectOrPrimitive(_classMetadata))
			{
				NotStorable(_obj, _classMetadata.ClassReflector());
			}
		}

		private bool DetectClassMetadata(IReflectClass claxx)
		{
			_classMetadata = _container.GetActiveClassMetadata(claxx);
			if (_classMetadata == null)
			{
				_classMetadata = _container.ProduceClassMetadata(claxx);
				if (_classMetadata == null)
				{
					NotStorable(_obj, claxx);
					return false;
				}
				_ref = _container.ReferenceForObject(_obj);
			}
			return true;
		}

		private void NotStorable(object obj, IReflectClass claxx)
		{
			_container.NotStorable(claxx, obj);
			_notStorable = true;
		}

		internal virtual bool NotStorable()
		{
			return _notStorable;
		}

		private bool IsPlainObjectOrPrimitive(Db4objects.Db4o.Internal.ClassMetadata classMetadata
			)
		{
			return classMetadata.GetID() == HandlerRegistry.ANY_ID || classMetadata.IsPrimitive
				();
		}

		internal virtual Db4objects.Db4o.Internal.ObjectReference ObjectReference()
		{
			return _ref;
		}

		public virtual Db4objects.Db4o.Internal.ClassMetadata ClassMetadata()
		{
			return _classMetadata;
		}
	}
}
