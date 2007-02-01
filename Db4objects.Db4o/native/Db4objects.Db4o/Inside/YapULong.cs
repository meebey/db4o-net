/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Inside
{
    internal class YapULong : YapTypeIntegral {

		public YapULong(Db4objects.Db4o.Inside.ObjectContainerBase stream)
			: base(stream)
		{
        }

        public override int Compare(Object o1, Object o2){
            return ((ulong)o2 > (ulong)o1) ? 1 : -1;
        }

        public override Object DefaultValue(){
            return (ulong)0;
        }
      
        public override void Write(object obj, byte[] bytes, int offset){
            ulong ul = (ulong)obj;
            for (int i = 0; i < 8; i++){
                bytes[offset++] = (byte)(int)(ul >> (7 - i) * 8);
            }
        }

        public override int TypeID(){
            return 23;
        }

        public override Object Read(byte[] bytes, int offset){
            ulong ul = 0;
            for (int i = 0; i < 8; i++) {
                ul = (ul << 8) + (ulong)(bytes[offset++] & 255);
            }
            return ul;
        }
    }
}
