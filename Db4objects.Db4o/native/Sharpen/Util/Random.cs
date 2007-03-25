/* Copyright (C) 2004   db4objects Inc.   http://www.db4o.com */

using System.Collections;

namespace Sharpen.Util 
{
	public class Random 
	{
		System.Random _random = new System.Random();

		public Random() 
		{
		}
		
		public long NextLong() 
		{
			return _random.Next();
		}
	}
}