/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */
namespace Db4objects.Db4o.Internal
{
    public sealed partial class Const4
    {
#if CF_2_0
        // MixedArrayTestCase fails with StackOverflow exceptions
        // when MaxStackDepth >= 20
        public const int MaxStackDepth = 19;
#else
        public const int MaxStackDepth = 20;
#endif
    }
}
