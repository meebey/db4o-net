using System;
using System.Collections.Generic;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4oTool.MSBuild.Tests
{
    public class CommandlineTestCase : AbstractDb4oTestCase
    {
        public void testByName()
        {
            Type type = typeof(IntItem);
            Assert.IsTrue(IsInstrumented(type));

            type = typeof(NonTAItem);
            Assert.IsFalse(IsInstrumented(type));
        }

        private bool IsInstrumented(Type type)
        {
            Type t = type.GetInterface("Db4objects.Db4o.TA.IActivatable");
            return (t != null);
        }
    }
}
