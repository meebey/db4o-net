/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System;

namespace Sharpen.Lang.Reflect {

    public class Modifier {

        public const int PUBLIC = 0x00000001;
        public const int PRIVATE = 0x00000002;
        public const int PROTECTED = 0x00000004;
        public const int STATIC = 0x00000008;
        public const int FINAL = 0x00000010;
        public const int TRANSIENT = 0x00000080;
        public const int INTERFACE = 0x00000200;
        public const int ABSTRACT = 0x00000400;

        public static bool IsAbstract(int mod) {
            return (mod & ABSTRACT) != 0;
        }

        public static bool IsFinal(int mod) {
            return (mod & FINAL) != 0;
        }

        public static bool IsInterface(int mod) {
            return (mod & INTERFACE) != 0;
        }

        public static bool IsPrivate(int mod) {
            return (mod & PRIVATE) != 0;
        }

        public static bool IsProtected(int mod) {
            return (mod & PROTECTED) != 0;
        }

        public static bool IsPublic(int mod) {
            return (mod & PUBLIC) != 0;
        }

        public static bool IsStatic(int mod) {
            return (mod & STATIC) != 0;
        }

        public static bool IsTransient(int mod) {
            return (mod & TRANSIENT) != 0;
        }
    }
}
