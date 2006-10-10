/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;
using System.Text;

namespace Sharpen.Lang {

    public class StringBuffer {

        private System.Text.StringBuilder stringBuilder;

        public StringBuffer() {
            stringBuilder = new StringBuilder();
        }
        
        public StringBuffer(string str) {
        	stringBuilder = new StringBuilder(str);
        }

        public StringBuffer Append(char c) {
            stringBuilder.Append(c);
            return this;
        }

        public StringBuffer Append(String str) {
            stringBuilder.Append(str);
            return this;
        }

        public StringBuffer Append(Object obj) {
            stringBuilder.Append(obj);
            return this;
        }

        public override String ToString() {
            return stringBuilder.ToString();
        }
    }
}
