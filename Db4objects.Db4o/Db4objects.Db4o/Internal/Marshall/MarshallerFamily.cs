/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal.Convert.Conversions;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public class MarshallerFamily
	{
		public class FamilyVersion
		{
			public const int PRE_MARSHALLER = 0;

			public const int MARSHALLER = 1;

			public const int BTREE_FIELD_INDEXES = 2;
		}

		private static int FAMILY_VERSION = MarshallingSpike.enabled ? MarshallingSpike.FamilyVersion
			() : MarshallerFamily.FamilyVersion.BTREE_FIELD_INDEXES;

		public readonly ArrayMarshaller _array;

		public readonly ClassMarshaller _class;

		public readonly IFieldMarshaller _field;

		public readonly ObjectMarshaller _object;

		public readonly PrimitiveMarshaller _primitive;

		public readonly StringMarshaller _string;

		public readonly UntypedMarshaller _untyped;

		private readonly int _converterVersion;

		private static readonly MarshallerFamily[] allVersions = MarshallingSpike.enabled
			 ? MarshallingSpike.MarshallerFamily() : new MarshallerFamily[] { new MarshallerFamily
			(0, new ArrayMarshaller0(), new ClassMarshaller0(), new FieldMarshaller0(), new 
			ObjectMarshaller0(), new PrimitiveMarshaller0(), new StringMarshaller0(), new UntypedMarshaller0
			()), new MarshallerFamily(ClassIndexesToBTrees_5_5.VERSION, new ArrayMarshaller1
			(), new ClassMarshaller1(), new FieldMarshaller0(), new ObjectMarshaller1(), new 
			PrimitiveMarshaller1(), new StringMarshaller1(), new UntypedMarshaller1()), new 
			MarshallerFamily(FieldIndexesToBTrees_5_7.VERSION, new ArrayMarshaller1(), new ClassMarshaller2
			(), new FieldMarshaller1(), new ObjectMarshaller1(), new PrimitiveMarshaller1(), 
			new StringMarshaller1(), new UntypedMarshaller1()) };

		public MarshallerFamily(int converterVersion, ArrayMarshaller arrayMarshaller, ClassMarshaller
			 classMarshaller, IFieldMarshaller fieldMarshaller, ObjectMarshaller objectMarshaller
			, PrimitiveMarshaller primitiveMarshaller, StringMarshaller stringMarshaller, UntypedMarshaller
			 untypedMarshaller)
		{
			_converterVersion = converterVersion;
			_array = arrayMarshaller;
			_array._family = this;
			_class = classMarshaller;
			_class._family = this;
			_field = fieldMarshaller;
			_object = objectMarshaller;
			_object._family = this;
			_primitive = primitiveMarshaller;
			_primitive._family = this;
			_string = stringMarshaller;
			_untyped = untypedMarshaller;
			_untyped._family = this;
		}

		public static MarshallerFamily Version(int n)
		{
			return allVersions[n];
		}

		public static MarshallerFamily Current()
		{
			if (FAMILY_VERSION < MarshallerFamily.FamilyVersion.BTREE_FIELD_INDEXES)
			{
				throw new InvalidOperationException("Using old marshaller versions to write database files is not supported, source code has been removed."
					);
			}
			return Version(FAMILY_VERSION);
		}

		public static MarshallerFamily ForConverterVersion(int n)
		{
			MarshallerFamily result = allVersions[0];
			for (int i = 1; i < allVersions.Length; i++)
			{
				if (allVersions[i]._converterVersion > n)
				{
					return result;
				}
				result = allVersions[i];
			}
			return result;
		}
	}
}
