/* Copyright (C) 2004 - 2009   db4objects Inc.   http://www.db4o.com */
using System;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Net;

namespace Db4objects.Db4o.Internal.Convert.Conversions
{
    partial class ReindexNetDateTime_7_8
    {
        private void ReindexDateTimeField(FieldMetadata field)
        {
            IReflectClass claxx = field.GetStoredType();
            if(claxx == null)
            {
                return;
            }
            Type t = NetReflector.ToNative(claxx);
            if(t == typeof(DateTime)  || t == typeof(DateTime?))
            {
                field.DropIndex();
                field.CreateIndex();
            }
        }
    }
}
