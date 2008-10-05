using System;

namespace Db4objects.Db4o.Tutorial.Console
{
	class App
	{
		[STAThread]
		static void Main(string[] args)
		{
            Db4objects.Db4o.Tutorial.F1.Chapter1.FirstStepsExample.Main(args);
            Db4objects.Db4o.Tutorial.F1.Chapter2.StructuredExample.Main(args);
            Db4objects.Db4o.Tutorial.F1.Chapter3.CollectionsExample.Main(args);
            Db4objects.Db4o.Tutorial.F1.Chapter4.InheritanceExample.Main(args);
            Db4objects.Db4o.Tutorial.F1.Chapter5.ClientServerExample.Main(args);
            Db4objects.Db4o.Tutorial.F1.Chapter7.TransparentActivationExample.Main(args);
            Db4objects.Db4o.Tutorial.F1.Chapter8.TransparentPersistenceExample.Main(args);
            Db4objects.Db4o.Tutorial.F1.Chapter10.BenchmarkExample.Main(args);
        }
	}
}
