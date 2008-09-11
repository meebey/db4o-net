/* Copyright (C) 2008   db4objects Inc.   http://www.db4o.com */
using System;
using System.Text;

namespace Db4objects.Db4o.Foundation
{
    public class UTF8Encoder
    {
        public byte[] Encode(String str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public String Decode(byte[] bytes, int start, int length)
        {
            return Encoding.UTF8.GetString(bytes, start, length);
        }

    }
    
}
