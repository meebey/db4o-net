using Db4objects.Db4o.Query;

using Db4objects.Db4o.Tutorial.F1.Chapter3;

namespace Db4objects.Db4o.Tutorial.F1.Chapter6
{	
	public class EvenHistoryEvaluation : IEvaluation
	{
		public void Evaluate(ICandidate candidate)
		{
			Car car=(Car)candidate.GetObject();
			candidate.Include(car.History.Count % 2 == 0);
		}
	}
}