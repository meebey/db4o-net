/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;

namespace Sharpen.Lang.Reflect {

    public class JavaArray {

        public static int GetLength(object array) {
            return ((Array)array).GetLength(0);
        }

        public static Object Get(object array, int index) {
            return ((Array)array).GetValue(index);
        }
    }
}
