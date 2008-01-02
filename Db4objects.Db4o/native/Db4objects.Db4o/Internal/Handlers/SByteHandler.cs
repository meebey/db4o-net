/* Copyright (C) 2004 - 2007   db4objects Inc.   http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Handlers
{
	public class SByteHandler : IntegralTypeHandler
	{
		public SByteHandler(ObjectContainerBase containerBase)
			: base(containerBase)
		{
        }

        public override Object DefaultValue(){
            return (sbyte)0;
        }
      
        public override Object Read(byte[] bytes, int offset){
            return (sbyte)  ((bytes[offset]) - 128) ;
        }

        public override int TypeID(){
            return 20;
        }
      
        public override void Write(Object obj, byte[] bytes, int offset){
            bytes[offset] = (byte)(((sbyte)obj) + 128);
        }

        public override object Read(IReadContext context)
        {
            return (sbyte)(context.ReadByte() - 128);
        }

        public override void Write(IWriteContext context, object obj)
        {
            context.WriteByte((byte)(((sbyte)obj) + 128));
        }
        
        public override IPreparedComparison InternalPrepareComparison(object obj)
        {
            return new PreparedComparasionFor<sbyte>(((sbyte)obj));
        }
    }
}
