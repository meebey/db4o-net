using System.Diagnostics;

namespace Db4objects.Db4o.Foundation
{
	public class PerformanceTracer
	{
		public delegate void Block();

		public static long TraceTime(string label, Block block)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			block();
			sw.Stop();
			Trace.WriteLine(label + ": " + sw.ElapsedMilliseconds + "ms\n");
			return sw.ElapsedMilliseconds;
		}

	}
}
