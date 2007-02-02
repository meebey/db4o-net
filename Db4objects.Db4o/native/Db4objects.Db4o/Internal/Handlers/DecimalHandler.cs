/* Copyright (C) 2004 - 2007   db4objects Inc.   http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Internal.Handlers
{
	public class DecimalHandler : IntegralTypeHandler
	{
		public DecimalHandler(ObjectContainerBase containerBase)
			: base(containerBase)
		{
        }

        public override int Compare(Object o1, Object o2){
            return ((decimal)o2 > (decimal)o1) ? 1 : -1;
        }

        public override Object DefaultValue(){
            return (decimal)0;
        }
      
        public override Object Read(byte[] bytes, int offset){
            int[] ints = new int[4];
            offset += 3;
            for(int i = 0; i < 4; i ++){
                ints[i] = (bytes[offset] & 255 | (bytes[--offset] & 255) << 8 | (bytes[--offset] & 255) << 16 | bytes[--offset] << 24);
                offset +=7;
            }
            return new Decimal(ints);
        }

        public override int TypeID(){
            return 21;
        }
      
        public override void Write(Object obj, byte[] bytes, int offset){
            decimal dec = (decimal)obj;
            int[] ints = Decimal.GetBits(dec);
            offset += 4;
            for(int i = 0; i < 4; i ++){
                bytes[--offset] = (byte)ints[i];
                bytes[--offset] = (byte)(ints[i] >>= 8);
                bytes[--offset] = (byte)(ints[i] >>= 8);
                bytes[--offset] = (byte)(ints[i] >>= 8);
                offset += 8;
            }
        }
    }
}
