namespace Db4oUnit
{
	public abstract class Printable
	{
		public override string ToString()
		{
			System.IO.StringWriter writer = new System.IO.StringWriter();
			try
			{
				Print(writer);
			}
			catch (System.IO.IOException e)
			{
			}
			return writer.ToString();
		}

		public abstract void Print(System.IO.TextWriter writer);
	}
}
