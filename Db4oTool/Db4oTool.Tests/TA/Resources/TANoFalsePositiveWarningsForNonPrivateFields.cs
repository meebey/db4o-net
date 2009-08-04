/* Copyright (C) 2009 Versant Corporation.   http://www.db4o.com */
using System;
using System.Threading;

class TAMixOfPersistentAndNoPersistentFields
{
	public int _persistentInt;

	[NonSerialized]
	public int _nonPersistentInt;

	public ParameterizedThreadStart _nonPersistentDelegate;

	public IntPtr _nonPersistentPointer;

#if NET_3_5
	private void ForceClassNameWithCurlyBraces()
	{
		int []value = {1, 2, 3};
	}
#endif
}

public class TAFilteredOut
{
	protected int _shouldNotGenerateWarning;
}