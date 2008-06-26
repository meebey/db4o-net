/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal.Handlers.Array
{
	/// <exclude></exclude>
	public class ArrayVersionHelper
	{
		public virtual int ClassIDFromInfo(ObjectContainerBase container, ArrayInfo info)
		{
			ClassMetadata classMetadata = container.ProduceClassMetadata(info.ReflectClass());
			if (classMetadata == null)
			{
				if (NullableArrayHandling.Disabled())
				{
					// TODO: This one is a terrible low-frequency blunder !!!
					// If YapClass-ID == 99999 then we will get IGNORE back.
					// Discovered on adding the primitives
					return Const4.IgnoreId;
				}
				return 0;
			}
			return classMetadata.GetID();
		}

		public virtual int ClassIdToMarshalledClassId(int classID, bool primitive)
		{
			if (NullableArrayHandling.Disabled())
			{
				if (primitive)
				{
					classID -= Const4.Primitive;
				}
				return -classID;
			}
			return classID;
		}

		public virtual IReflectClass ClassReflector(IReflector reflector, ClassMetadata classMetadata
			, bool isPrimitive)
		{
			return isPrimitive ? Handlers4.PrimitiveClassReflector(classMetadata, reflector) : 
				classMetadata.ClassReflector();
		}

		public virtual bool UseJavaHandling()
		{
			if (NullableArrayHandling.Disabled())
			{
				return !Deploy.csharp;
			}
			return true;
		}

		public virtual bool HasNullBitmap(ArrayInfo info)
		{
			if (NullableArrayHandling.Disabled())
			{
				return false;
			}
			return !info.Primitive();
		}

		public virtual bool IsPreVersion0Format(int elementCount)
		{
			return false;
		}

		public virtual bool IsPrimitive(IReflector reflector, IReflectClass claxx, ClassMetadata
			 classMetadata)
		{
			if (NullableArrayHandling.Disabled())
			{
				return false;
			}
			return claxx.IsPrimitive();
		}

		public virtual IReflectClass ReflectClassFromElementsEntry(ObjectContainerBase container
			, ArrayInfo info, int classID)
		{
			if (NullableArrayHandling.Disabled())
			{
				if (classID == Const4.IgnoreId)
				{
					// TODO: Here is a low-frequency mistake, extremely unlikely.
					// If classID == 99999 by accident then we will get ignore.
					return null;
				}
				info.Primitive(false);
				if (UseJavaHandling())
				{
					if (classID < Const4.Primitive)
					{
						info.Primitive(true);
						classID -= Const4.Primitive;
					}
				}
				classID = -classID;
				ClassMetadata classMetadata0 = container.ClassMetadataForId(classID);
				if (classMetadata0 != null)
				{
					return ClassReflector(container.Reflector(), classMetadata0, info.Primitive());
				}
				return null;
			}
			if (classID == 0)
			{
				return null;
			}
			ClassMetadata classMetadata = container.ClassMetadataForId(classID);
			if (classMetadata == null)
			{
				return null;
			}
			return ClassReflector(container.Reflector(), classMetadata, info.Primitive());
		}

		public virtual void WriteTypeInfo(IWriteContext context, ArrayInfo info)
		{
			if (NullableArrayHandling.Disabled())
			{
				return;
			}
			BitMap4 typeInfoBitmap = new BitMap4(2);
			typeInfoBitmap.Set(0, info.Primitive());
			typeInfoBitmap.Set(1, info.Nullable());
			context.WriteByte(typeInfoBitmap.GetByte(0));
		}

		public virtual void ReadTypeInfo(Transaction trans, IReadBuffer buffer, ArrayInfo
			 info, int classID)
		{
			if (NullableArrayHandling.Disabled())
			{
				return;
			}
			BitMap4 typeInfoBitmap = new BitMap4(buffer.ReadByte());
			info.Primitive(typeInfoBitmap.IsTrue(0));
			info.Nullable(typeInfoBitmap.IsTrue(1));
		}
	}
}
