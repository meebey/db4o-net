using Db4oUnit;

namespace Db4oUnit
{
	public interface ILabelProvider
	{
		string GetLabel(TestMethod method);
	}
}
