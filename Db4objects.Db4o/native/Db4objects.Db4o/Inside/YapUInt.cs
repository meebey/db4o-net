/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Inside
{
	internal class YapUInt : YapTypeIntegral
	{

		public YapUInt(Db4objects.Db4o.Inside.ObjectContainerBase stream)
			: base(stream)
		{
        }

        public override int Compare(Object o1, Object o2){
            return ((uint)o2 > (uint)o1) ? 1 : -1;
        }

        public override Object DefaultValue(){
            return (uint)0;
        }
      
        public override Object Read(byte[] bytes, int offset){
            offset += 3;
            return (uint) (bytes[offset] & 255 | (bytes[--offset] & 255) << 8 | (bytes[--offset] & 255) << 16 | bytes[--offset] << 24);
        }

        public override int TypeID(){
            return 22;
        }
      
        public override void Write(Object obj, byte[] bytes, int offset){
            uint ui = (uint)obj;
            offset += 4;
            bytes[--offset] = (byte)ui;
            bytes[--offset] = (byte)(ui >>= 8);
            bytes[--offset] = (byte)(ui >>= 8);
            bytes[--offset] = (byte)(ui >>= 8);
        }
    }
}
