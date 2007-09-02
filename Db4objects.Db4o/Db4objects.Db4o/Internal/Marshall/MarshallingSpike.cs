/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.Convert.Conversions;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <summary>temporary code to be able to factor around ObjectMarshaller code</summary>
	/// <exclude></exclude>
	public class MarshallingSpike
	{
		public const bool enabled = false;

		public static Db4objects.Db4o.Internal.Marshall.MarshallerFamily[] MarshallerFamily
			()
		{
			return new Db4objects.Db4o.Internal.Marshall.MarshallerFamily[] { new Db4objects.Db4o.Internal.Marshall.MarshallerFamily
				(0, new ArrayMarshaller0(), new ClassMarshaller0(), new FieldMarshaller0(), new 
				ObjectMarshaller0(), new PrimitiveMarshaller0(), new StringMarshaller0(), new UntypedMarshaller0
				()), new Db4objects.Db4o.Internal.Marshall.MarshallerFamily(ClassIndexesToBTrees_5_5
				.VERSION, new ArrayMarshaller1(), new ClassMarshaller1(), new FieldMarshaller0()
				, new ObjectMarshaller1(), new PrimitiveMarshaller1(), new StringMarshaller1(), 
				new UntypedMarshaller1()), new Db4objects.Db4o.Internal.Marshall.MarshallerFamily
				(FieldIndexesToBTrees_5_7.VERSION, new ArrayMarshaller1(), new ClassMarshaller2(
				), new FieldMarshaller1(), new ObjectMarshaller1(), new PrimitiveMarshaller1(), 
				new StringMarshaller1(), new UntypedMarshaller1()), new Db4objects.Db4o.Internal.Marshall.MarshallerFamily
				(7, new ArrayMarshaller1(), new ClassMarshaller2(), new FieldMarshaller1(), new 
				ObjectMarshaller2Spike(), new PrimitiveMarshaller1(), new StringMarshaller1(), new 
				UntypedMarshaller1()) };
		}

		public static int FamilyVersion()
		{
			return 3;
		}
	}
}
