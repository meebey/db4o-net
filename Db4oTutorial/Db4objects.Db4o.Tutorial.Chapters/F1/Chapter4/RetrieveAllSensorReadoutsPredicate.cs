using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Tutorial.F1.Chapter4
{
	public class RetrieveAllSensorReadoutsPredicate : Predicate 
	{
		public bool Match(SensorReadout candidate)
		{
			return true;
		}
	}
}