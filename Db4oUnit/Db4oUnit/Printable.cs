using System.IO;

namespace Db4oUnit
{
	public abstract class Printable
	{
		public override string ToString()
		{
			StringWriter writer = new StringWriter();
			try
			{
				Print(writer);
			}
			catch (IOException)
			{
			}
			return writer.ToString();
		}

		public abstract void Print(TextWriter writer);
	}
}
