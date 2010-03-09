/* Copyright (C) 2010 Versant Inc. http://www.db4o.com */
using System.Collections.Generic;

namespace Db4oTool.Tests.Integration.Model
{

public class CollectionHolder<T>
{
	private string _name;
	private IList<T> _list;
	
	public CollectionHolder()
	{
		// db4o creation constructor
	}
	
	public CollectionHolder(string name, params T[] items) 
	{
		_name = name;
		_list = new List<T>(items);
	}

	public IList<T> List
	{
		get { return _list; }
	}

	public override string ToString()
	{
		return _name + ": " + _list + "";
	}
}
}
