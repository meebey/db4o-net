/* Copyright (C) 2010 Versant Inc. http://www.db4o.com */
namespace Db4oTool.Tests.Integration.Model
{
	public class Item
	{
		private string _name;

		public Item(string name)
		{
			_name = name;
		}

		public string Name
		{
			get { return _name; }
		}

		public override string ToString()
		{
			return "Item: " + _name;
		}

	}
}
