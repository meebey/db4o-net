/* Copyright (C) 2004 - 2006  db4objects Inc.   http://www.db4o.com */

using System;
using System.IO;
using System.Collections.Generic;
using Db4objects.Db4o;
using Db4oUnit;

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
}

// TODO: query invocation with comparator
// TODO: query invocation with comparison
public class CFNQSubject : Db4oAdmin.Tests.InstrumentedTestCase
{
	override public void SetUp()
	{	
		_container.Set(new Item("foo"));
		_container.Set(new Item("bar"));
	}

	public void TestInlineStaticDelegate()
	{	
		IList<Item> items = _container.Query<Item>(delegate(Item candidate)
		{
			return candidate.Name == "foo";
		});
		CheckResult(items);
	}

	public void TestInlineClosureDelegate()
	{	
		string name = "foo";
		IList<Item> items = _container.Query<Item>(delegate(Item candidate)
		{
			return candidate.Name == name;
		});
		CheckResult(items);
	}

	public void TestStaticMemberDelegate()
	{	
		IList<Item> items = _container.Query<Item>(CFNQSubject.MatchFoo);
		CheckResult(items);
	}

	public void TestMultipleQueryInvocations()
	{	
		CheckResult(_container.Query<Item>(CFNQSubject.MatchFoo));
		CheckResult(_container.Query<Item>(CFNQSubject.MatchFoo));
		CheckResult(_container.Query<Item>(CFNQSubject.MatchFoo));
	}

	delegate IObjectContainer ObjectContainerAccessor();

	public void TestInlineStaticDelegateInsideExpression()
	{
		ObjectContainerAccessor getter = delegate { return _container; };
		CheckResult(getter().Query<Item>(delegate(Item candidate)
		{
			return candidate.Name == "foo";
		}));
	}

	public void TestInstanceMemberDelegate()
	{
		IList<Item> items = _container.Query<Item>(new QueryItemByName("foo").Match);
		CheckResult(items);
	}

	private void CheckResult(IList<Item> items)
	{
		Assert.AreEqual(1, items.Count);
		Assert.AreEqual("foo", items[0].Name);
	}

	static bool MatchFoo(Item candidate)
	{
		return candidate.Name == "foo";
	}

	class QueryItemByName
	{
		string _name;

		public QueryItemByName(string name)
		{
			_name = name;
		}

		public bool Match(Item candidate)
		{
			return candidate.Name == _name;
		}
	}
}
