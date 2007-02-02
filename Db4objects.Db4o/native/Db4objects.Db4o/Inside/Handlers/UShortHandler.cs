/* Copyright (C) 2004 - 2007   db4objects Inc.   http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Inside.Handlers
{
	public class UShortHandler : IntegralTypeHandler
	{
		public UShortHandler(ObjectContainerBase containerBase)
			: base(containerBase)
		{
        }

        public override int Compare(Object o1, Object o2){
            return ((ushort)o2 > (ushort)o1) ? 1 : -1;
        }

        public override Object DefaultValue(){
            return (ushort)0;
        }
      
        public override Object Read(byte[] bytes, int offset){
            offset += 1;
            return (ushort) (bytes[offset] & 255 | (bytes[--offset] & 255) << 8);
        }
      
        public override int TypeID(){
            return 24;
        }
      
        public override void Write(Object obj, byte[] bytes, int offset){
            ushort us = (ushort)obj;
            offset += 2;
            bytes[--offset] = (byte)us;
            bytes[--offset] = (byte)(us >>= 8);
        }
    }
}
