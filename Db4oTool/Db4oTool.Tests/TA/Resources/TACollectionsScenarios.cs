/* Copyright (C) 2010   Versant Inc.   http://www.db4o.com */

using System;
using System.Collections.Generic;
using System.Diagnostics;

public class TACollectionsScenarios
{
	private IList<string> _interface;
	private List<string> _list;

	public void ParameterLessConstructor()
	{
		_interface = new List<string>();
	}

	public void ConstructorTakingIEnumerable()
	{
	    _interface = new List<string>(new string[] {"foo", "bar" });
	}

	public void ConstructorTakingInitialSize()
	{
	    _interface = new List<string>(10);
	}

	public void LocalsAsIList()
	{
		IList<int> local = new List<int>();
	}

	public void SimpleInstantiation()
	{
	    IList<DateTime> local = CreateList();
	}

	private static IList<DateTime> CreateList()
	{
		IList<DateTime> theList = new List<DateTime>();
		theList.Add(DateTime.Now);

		return theList;
	}

	#region ThisMayWork
	// These 2 ones 'can' work with we ensure that:
	//	- The method declared as List<T> is private
	//  - We create a list of callers of this method and make sure no one assigns to List<T> (only to IList<T> or some other interface/object?);
	//  - We replace the return type with ActivatableList<T> / IList<T>
	//
	// For now, just ignore and emit warning.
	// 
	//public void SimpleInstantiationThroughList()
	//{
	//    IList<string> local = PrivateCreateConcreteList();
	//}

	//private List<string> PrivateCreateConcreteList()
	//{
	//    return new List<string>();
	//}
	#endregion

	public List<string> PublicCreateConcreteList()
	{
		return new List<string>();
	}

	public void CastFollowedByParameterLessMethod()
	{
		// Is it a cast to List<T> ?
		// Are we calling a method / property imediatly ?
		((List<string>)_interface).Sort();
	}

	public void CastFollowedByMethodWithSingleArgument()
	{
		((List<string>)_interface).Add("foo");
	}

	public void InitInterface(IList<string> list)
	{
		InitInterface(new List<string>());
	}

	/**
	 * The following instantiations of List<> should not be exchanged by its ActivatableList<> counterparts since
	 * they are assigning directly to List<> instead of IList<>
	 */
	#region ThisMustFailOrEmitWarning

	#region ShouldNotBeExchangedByActivatableListOfT

	public void AssignmentToConcreteList()
	{
		List<string> local = new List<string>();
	}

	public void InitConcrete(List<string> list)
	{
	    InitConcrete(new List<string>());
	}

	//public void AssignmentThroughDelegates()
	//{
	//    Action<List<string>> theAction = InitConcrete2;
	//    theAction(new List<string>());
	//}

	//public void InitConcrete2(List<string> list)
	//{
	//    InitConcrete2(list);
	//}

	//public void InitConcrete3(string str, List<string> list, int value)
	//{
	//    InitConcrete3(str, new List<string>(), value);
	//}

	#endregion

	[Conditional("CastNotFollowedByConcreteMethodCall")]
	public void CastNotFollowedByConcreteMethodCall()
	{
		IList<string> dubious = ((List<string>)_interface);
	}	

	#endregion
}

//TODO: Make this condition explicity. 
//      As of now the simple existence is enoght to tigger failures if the
//      code is note prepared to handle it. 
public class GenericHolder<T>
{
    private IList<T> _list;

    public void Init()
    {
        _list = new List<T>();
    }
}