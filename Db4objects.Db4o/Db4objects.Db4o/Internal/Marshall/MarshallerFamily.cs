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

		private static int FAMILY_VERSION = Db4objects.Db4o.Internal.Marshall.MarshallerFamily.FamilyVersion
			.BTREE_FIELD_INDEXES;

		public readonly Db4objects.Db4o.Internal.Marshall.ArrayMarshaller _array;

		public readonly Db4objects.Db4o.Internal.Marshall.ClassMarshaller _class;

		public readonly Db4objects.Db4o.Internal.Marshall.IFieldMarshaller _field;

		public readonly Db4objects.Db4o.Internal.Marshall.ObjectMarshaller _object;

		public readonly Db4objects.Db4o.Internal.Marshall.PrimitiveMarshaller _primitive;

		public readonly Db4objects.Db4o.Internal.Marshall.StringMarshaller _string;

		public readonly Db4objects.Db4o.Internal.Marshall.UntypedMarshaller _untyped;

		private readonly int _converterVersion;

		private static readonly Db4objects.Db4o.Internal.Marshall.MarshallerFamily[] allVersions
			 = new Db4objects.Db4o.Internal.Marshall.MarshallerFamily[] { new Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			(0, new Db4objects.Db4o.Internal.Marshall.ArrayMarshaller0(), new Db4objects.Db4o.Internal.Marshall.ClassMarshaller0
			(), new Db4objects.Db4o.Internal.Marshall.FieldMarshaller0(), new Db4objects.Db4o.Internal.Marshall.ObjectMarshaller0
			(), new Db4objects.Db4o.Internal.Marshall.PrimitiveMarshaller0(), new Db4objects.Db4o.Internal.Marshall.StringMarshaller0
			(), new Db4objects.Db4o.Internal.Marshall.UntypedMarshaller0()), new Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			(Db4objects.Db4o.Internal.Convert.Conversions.ClassIndexesToBTrees_5_5.VERSION, 
			new Db4objects.Db4o.Internal.Marshall.ArrayMarshaller1(), new Db4objects.Db4o.Internal.Marshall.ClassMarshaller1
			(), new Db4objects.Db4o.Internal.Marshall.FieldMarshaller0(), new Db4objects.Db4o.Internal.Marshall.ObjectMarshaller1
			(), new Db4objects.Db4o.Internal.Marshall.PrimitiveMarshaller1(), new Db4objects.Db4o.Internal.Marshall.StringMarshaller1
			(), new Db4objects.Db4o.Internal.Marshall.UntypedMarshaller1()), new Db4objects.Db4o.Internal.Marshall.MarshallerFamily
			(Db4objects.Db4o.Internal.Convert.Conversions.FieldIndexesToBTrees_5_7.VERSION, 
			new Db4objects.Db4o.Internal.Marshall.ArrayMarshaller1(), new Db4objects.Db4o.Internal.Marshall.ClassMarshaller2
			(), new Db4objects.Db4o.Internal.Marshall.FieldMarshaller1(), new Db4objects.Db4o.Internal.Marshall.ObjectMarshaller1
			(), new Db4objects.Db4o.Internal.Marshall.PrimitiveMarshaller1(), new Db4objects.Db4o.Internal.Marshall.StringMarshaller1
			(), new Db4objects.Db4o.Internal.Marshall.UntypedMarshaller1()) };

		private MarshallerFamily(int converterVersion, Db4objects.Db4o.Internal.Marshall.ArrayMarshaller
			 arrayMarshaller, Db4objects.Db4o.Internal.Marshall.ClassMarshaller classMarshaller
			, Db4objects.Db4o.Internal.Marshall.IFieldMarshaller fieldMarshaller, Db4objects.Db4o.Internal.Marshall.ObjectMarshaller
			 objectMarshaller, Db4objects.Db4o.Internal.Marshall.PrimitiveMarshaller primitiveMarshaller
			, Db4objects.Db4o.Internal.Marshall.StringMarshaller stringMarshaller, Db4objects.Db4o.Internal.Marshall.UntypedMarshaller
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

		public static Db4objects.Db4o.Internal.Marshall.MarshallerFamily Version(int n)
		{
			return allVersions[n];
		}

		public static Db4objects.Db4o.Internal.Marshall.MarshallerFamily Current()
		{
			if (FAMILY_VERSION < Db4objects.Db4o.Internal.Marshall.MarshallerFamily.FamilyVersion
				.BTREE_FIELD_INDEXES)
			{
				throw new System.InvalidOperationException("Using old marshaller versions to write database files is not supported, source code has been removed."
					);
			}
			return Version(FAMILY_VERSION);
		}

		public static Db4objects.Db4o.Internal.Marshall.MarshallerFamily ForConverterVersion
			(int n)
		{
			Db4objects.Db4o.Internal.Marshall.MarshallerFamily result = allVersions[0];
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
