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
			Db4objects.Db4o.Tutorial.F1.Chapter6.TranslatorExample.Main(args);
			Db4objects.Db4o.Tutorial.F1.Chapter21.IndexedExample.fillUpDB();
			Db4objects.Db4o.Tutorial.F1.Chapter21.IndexedExample.noIndex();
			Db4objects.Db4o.Tutorial.F1.Chapter21.IndexedExample.fullIndex();
			Db4objects.Db4o.Tutorial.F1.Chapter21.IndexedExample.pilotIndex();
			Db4objects.Db4o.Tutorial.F1.Chapter21.IndexedExample.pointsIndex();
		}
	}
}
