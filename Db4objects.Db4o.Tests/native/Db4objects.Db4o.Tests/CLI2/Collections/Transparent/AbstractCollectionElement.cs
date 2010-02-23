namespace Db4objects.Db4o.Tests.CLI2.Collections.Transparent
{
	public class AbstractCollectionElement : ICollectionElement
	{
		protected string _name;

		public AbstractCollectionElement(string name)
		{
			_name = name;	
		}

		public int CompareTo(ICollectionElement other)
		{
			if(Name == null)
			{
				if(other.Name == null)
				{
					return 0;
				}
				
				return -1;
			}
		
			return Name.CompareTo(other.Name);
		}

		public string Name
		{
			get
			{
				ReadFieldAccess();
				return _name;
			}
		}

		protected virtual void ReadFieldAccess()
		{
			// Do nothing
		}
	}
}
