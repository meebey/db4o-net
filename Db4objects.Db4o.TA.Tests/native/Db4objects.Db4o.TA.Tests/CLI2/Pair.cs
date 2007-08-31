namespace Db4objects.Db4o.TA.Tests.CLI2
{
	struct Pair<T0, T1> where T1: struct
	{
		public T0 First;
		public T1 Second;

		public Pair(T0 first, T1 second)
		{
			First = first;
			Second = second;
		}
	}
}