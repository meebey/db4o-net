namespace Db4oUnit
{
	public class TestFailureCollection : Db4oUnit.Printable
	{
		internal System.Collections.ArrayList _failures = new System.Collections.ArrayList
			();

		public virtual System.Collections.IEnumerator Iterator()
		{
			return _failures.GetEnumerator();
		}

		public virtual int Size()
		{
			return _failures.Count;
		}

		public virtual void Add(Db4oUnit.TestFailure failure)
		{
			_failures.Add(failure);
		}

		public override void Print(System.IO.TextWriter writer)
		{
			PrintSummary(writer);
			PrintDetails(writer);
		}

		private void PrintSummary(System.IO.TextWriter writer)
		{
			int index = 1;
			System.Collections.IEnumerator e = Iterator();
			while (e.MoveNext())
			{
				writer.Write(index.ToString());
				writer.Write(") ");
				writer.Write(((Db4oUnit.TestFailure)e.Current).GetTest().GetLabel());
				writer.Write("\n");
				++index;
			}
		}

		private void PrintDetails(System.IO.TextWriter writer)
		{
			int index = 1;
			System.Collections.IEnumerator e = Iterator();
			while (e.MoveNext())
			{
				writer.Write("\n");
				writer.Write(index.ToString());
				writer.Write(") ");
				((Db4oUnit.Printable)e.Current).Print(writer);
				writer.Write("\n");
				++index;
			}
		}
	}
}
