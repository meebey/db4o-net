/* Copyright (C) 2004 - 2009   Versant Inc.   http://www.db4o.com */
using System;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Net;

namespace Db4objects.Db4o.Internal.Convert.Conversions
{
    partial class ReindexNetDateTime_7_8 : Conversion
    {
		public override void Convert(ConversionStage.SystemUpStage stage)
		{
			ReindexDateTimeFields(stage);
		}

		private static void ReindexDateTimeFields(ConversionStage stage)
		{
			DateTimeFieldReindexer reindexer = new DateTimeFieldReindexer();
			
			ClassMetadataIterator i = stage.File().ClassCollection().Iterator();
			while (i.MoveNext())
			{
				ClassMetadata classmetadata = i.CurrentClass();
				classmetadata.TraverseDeclaredFields(reindexer);
			}
		}
		
		private class DateTimeFieldReindexer : IProcedure4
		{
			public void Apply(object field)
			{
				if (!((FieldMetadata)field).HasIndex())
				{
					return;
				}
				ReindexDateTimeField(((FieldMetadata)field));
			}

			private static void ReindexDateTimeField(IStoredField field)
			{
				IReflectClass claxx = field.GetStoredType();
				if (claxx == null)
				{
					return;
				}

				Type t = NetReflector.ToNative(claxx);
				if (t == typeof(DateTime) || t == typeof(DateTime?))
				{
					field.DropIndex();
					field.CreateIndex();
				}
			}
		}
    }
}
