/* Copyright (C) 2004 - 2007   db4objects Inc.   http://www.db4o.com */

using System;

namespace Db4objects.Db4o.Inside.Handlers
{
    public class DateTimeHandler : StructHandler {

		public DateTimeHandler(ObjectContainerBase containerBase)
			: base(containerBase)
		{
        }

        public override int Compare(object o1, object o2){
            return ((DateTime)o2 > (DateTime)o1) ? 1 : -1;
        }

        public override Object DefaultValue(){
            return new DateTime(0);
        }

        public override bool IsEqual(Object o1, Object o2){
            return o1.Equals(o2);
        }

        public override Object Read(byte[] bytes, int offset){
            long ticks = 0;
            for (int i = 0; i < 8; i++) {
                ticks = (ticks << 8) + (long)(bytes[offset++] & 255);
            }
            return new DateTime(ticks);
        }

        public override int TypeID(){
            return 25;
        }

        public override void Write(object obj, byte[] bytes, int offset){
            long ticks = ((DateTime)obj).Ticks;
            for (int i = 0; i < 8; i++){
                bytes[offset++] = (byte)(int)(ticks >> (7 - i) * 8);
            }
        }
    }
}
