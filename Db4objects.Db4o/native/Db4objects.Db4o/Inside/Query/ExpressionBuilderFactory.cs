/* Copyright (C) 2006   db4objects Inc.   http://www.db4o.com */

namespace Db4objects.Db4o.Inside.Query
{
	using System;
	using System.Reflection;

	public class ExpressionBuilderFactory
	{
		public static ExpressionBuilder CreateExpressionBuilder()
		{
			Type type = Type.GetType("Db4oTools.NativeQueries.QueryExpressionBuilder, Db4oTools", true);
			return (ExpressionBuilder)Activator.CreateInstance(type);
		}
	}
}
