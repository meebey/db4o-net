/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using System.IO;
using Db4oUnit;

namespace Db4oUnit
{
	public class TestFailureCollection : Printable
	{
		internal ArrayList _failures = new ArrayList();

		public virtual IEnumerator Iterator()
		{
			return _failures.GetEnumerator();
		}

		public virtual int Size()
		{
			return _failures.Count;
		}

		public virtual void Add(TestFailure failure)
		{
			_failures.Add(failure);
		}

		/// <exception cref="IOException"></exception>
		public override void Print(TextWriter writer)
		{
			PrintSummary(writer);
			PrintDetails(writer);
		}

		/// <exception cref="IOException"></exception>
		private void PrintSummary(TextWriter writer)
		{
			int index = 1;
			IEnumerator e = Iterator();
			while (e.MoveNext())
			{
				writer.Write(index.ToString());
				writer.Write(") ");
				writer.Write(((TestFailure)e.Current).GetTest().GetLabel());
				writer.Write(TestPlatform.NewLine);
				++index;
			}
		}

		/// <exception cref="IOException"></exception>
		private void PrintDetails(TextWriter writer)
		{
			int index = 1;
			IEnumerator e = Iterator();
			while (e.MoveNext())
			{
				writer.Write(TestPlatform.NewLine);
				writer.Write(index.ToString());
				writer.Write(") ");
				((Printable)e.Current).Print(writer);
				writer.Write(TestPlatform.NewLine);
				++index;
			}
		}
	}
}
