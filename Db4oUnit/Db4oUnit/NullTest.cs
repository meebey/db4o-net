using Db4oUnit;

namespace Db4oUnit
{
	public class NullTest : ITest
	{
		internal string _label;

		public NullTest(string label)
		{
			_label = label;
		}

		public virtual string GetLabel()
		{
			return _label;
		}

		public virtual void Run(TestResult result)
		{
		}
	}
}
