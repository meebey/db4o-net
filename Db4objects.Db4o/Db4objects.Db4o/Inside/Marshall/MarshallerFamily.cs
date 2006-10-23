namespace Db4objects.Db4o.Inside.Marshall
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

		private static int FAMILY_VERSION = Db4objects.Db4o.Inside.Marshall.MarshallerFamily.FamilyVersion
			.BTREE_FIELD_INDEXES;

		public readonly Db4objects.Db4o.Inside.Marshall.ArrayMarshaller _array;

		public readonly Db4objects.Db4o.Inside.Marshall.ClassMarshaller _class;

		public readonly Db4objects.Db4o.Inside.Marshall.IFieldMarshaller _field;

		public readonly Db4objects.Db4o.Inside.Marshall.ObjectMarshaller _object;

		public readonly Db4objects.Db4o.Inside.Marshall.PrimitiveMarshaller _primitive;

		public readonly Db4objects.Db4o.Inside.Marshall.StringMarshaller _string;

		public readonly Db4objects.Db4o.Inside.Marshall.UntypedMarshaller _untyped;

		private readonly int _converterVersion;

		private static readonly Db4objects.Db4o.Inside.Marshall.MarshallerFamily[] allVersions
			 = new Db4objects.Db4o.Inside.Marshall.MarshallerFamily[] { new Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			(0, new Db4objects.Db4o.Inside.Marshall.ArrayMarshaller0(), new Db4objects.Db4o.Inside.Marshall.ClassMarshaller0
			(), new Db4objects.Db4o.Inside.Marshall.FieldMarshaller0(), new Db4objects.Db4o.Inside.Marshall.ObjectMarshaller0
			(), new Db4objects.Db4o.Inside.Marshall.PrimitiveMarshaller0(), new Db4objects.Db4o.Inside.Marshall.StringMarshaller0
			(), new Db4objects.Db4o.Inside.Marshall.UntypedMarshaller0()), new Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			(Db4objects.Db4o.Inside.Convert.Conversions.ClassIndexesToBTrees_5_5.VERSION, new 
			Db4objects.Db4o.Inside.Marshall.ArrayMarshaller1(), new Db4objects.Db4o.Inside.Marshall.ClassMarshaller1
			(), new Db4objects.Db4o.Inside.Marshall.FieldMarshaller0(), new Db4objects.Db4o.Inside.Marshall.ObjectMarshaller1
			(), new Db4objects.Db4o.Inside.Marshall.PrimitiveMarshaller1(), new Db4objects.Db4o.Inside.Marshall.StringMarshaller1
			(), new Db4objects.Db4o.Inside.Marshall.UntypedMarshaller1()), new Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			(Db4objects.Db4o.Inside.Convert.Conversions.FieldIndexesToBTrees_5_7.VERSION, new 
			Db4objects.Db4o.Inside.Marshall.ArrayMarshaller1(), new Db4objects.Db4o.Inside.Marshall.ClassMarshaller2
			(), new Db4objects.Db4o.Inside.Marshall.FieldMarshaller1(), new Db4objects.Db4o.Inside.Marshall.ObjectMarshaller1
			(), new Db4objects.Db4o.Inside.Marshall.PrimitiveMarshaller1(), new Db4objects.Db4o.Inside.Marshall.StringMarshaller1
			(), new Db4objects.Db4o.Inside.Marshall.UntypedMarshaller1()) };

		private MarshallerFamily(int converterVersion, Db4objects.Db4o.Inside.Marshall.ArrayMarshaller
			 arrayMarshaller, Db4objects.Db4o.Inside.Marshall.ClassMarshaller classMarshaller
			, Db4objects.Db4o.Inside.Marshall.IFieldMarshaller fieldMarshaller, Db4objects.Db4o.Inside.Marshall.ObjectMarshaller
			 objectMarshaller, Db4objects.Db4o.Inside.Marshall.PrimitiveMarshaller primitiveMarshaller
			, Db4objects.Db4o.Inside.Marshall.StringMarshaller stringMarshaller, Db4objects.Db4o.Inside.Marshall.UntypedMarshaller
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

		public static Db4objects.Db4o.Inside.Marshall.MarshallerFamily Version(int n)
		{
			return allVersions[n];
		}

		public static Db4objects.Db4o.Inside.Marshall.MarshallerFamily Current()
		{
			if (FAMILY_VERSION < Db4objects.Db4o.Inside.Marshall.MarshallerFamily.FamilyVersion
				.BTREE_FIELD_INDEXES)
			{
				throw new System.InvalidOperationException("Using old marshaller versions to write database files is not supported, source code has been removed."
					);
			}
			return Version(FAMILY_VERSION);
		}

		public static Db4objects.Db4o.Inside.Marshall.MarshallerFamily ForConverterVersion
			(int n)
		{
			Db4objects.Db4o.Inside.Marshall.MarshallerFamily result = allVersions[0];
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
