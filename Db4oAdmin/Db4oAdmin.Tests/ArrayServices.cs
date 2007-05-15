/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;

namespace Db4oAdmin.Tests
{
	class ArrayServices
	{
		public static T[] Append<T>(T[] line, T path)
		{
			Array.Resize(ref line, line.Length + 1);
			line[line.Length - 1] = path;
			return line;
		}
	}
}
