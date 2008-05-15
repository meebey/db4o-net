/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers
{
	public class ArrayHandler3 : ArrayHandler
	{
		protected override bool IsPrimitive(IReflectClass claxx)
		{
			// TODO: Check if this is correct. 
			// In this case we may get the nullable type
			// associated with this arrayhandler, but
			// we always want to use the non-nullable
			// type if we read with the old arrayHandler.
			return false;
			return claxx.IsPrimitive();
		}

		protected sealed override bool UseJavaHandling()
		{
			return !Deploy.csharp;
		}

		protected override bool HasNullBitmap()
		{
			return false;
		}

		protected override IReflectClass ClassReflector(IReflector reflector, ClassMetadata
			 classMetadata, bool isPrimitive)
		{
			if (Deploy.csharp && NullableArrayHandling.Enabled())
			{
				IReflectClass primitiveClaxx = Handlers4.PrimitiveClassReflector(classMetadata, reflector
					);
				if (primitiveClaxx != null)
				{
					return primitiveClaxx;
				}
			}
			return base.ClassReflector(reflector, classMetadata, isPrimitive);
		}
	}
}
