using System;
using System.Collections.Generic;
using System.Text;

namespace Db4oTools.MSBuild.Tests
{
    public class MSBuildTaskTest
    {
        public static int Main(string[] args)
        {
            new IntItemTestCase().RunSolo();
            return 0;
        }
    }
}
