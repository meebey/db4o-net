/* Copyright (C) 2006   db4objects Inc.   http://www.db4o.com */

namespace Db4objects.Db4o.Internal.Query
{
	using System;

	public class ExpressionBuilderFactory
	{
		public static ExpressionBuilder CreateExpressionBuilder()
		{
			Type type = Type.GetType("Db4objects.Db4o.Tools.NativeQueries.QueryExpressionBuilder, Db4objects.Db4o.Tools", true);
			return (ExpressionBuilder)Activator.CreateInstance(type);
		}
	}
}
