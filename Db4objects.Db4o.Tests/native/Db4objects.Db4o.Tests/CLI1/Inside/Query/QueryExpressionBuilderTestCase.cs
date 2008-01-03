/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.Reflection;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.NativeQueries.Expr;
using Db4objects.Db4o.NativeQueries.Expr.Cmp;
using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tests.CLI1.NativeQueries;
using Db4objects.Db4o.NativeQueries;
using Db4objects.Db4o.Tests.NativeQueries.Mocks;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI1.Inside.Query
{	
	public class QueryExpressionBuilderTestCase : ITestCase
	{
		public class Item
		{
			public string name;
		}

		bool MatchWithActivate(Item item)
		{
			((IActivatable) item).Activate(ActivationPurpose.Read);
			return item.name.StartsWith("foo");
		}

		public void TestActivateCallsAreIgnored()
		{
			IExpression expression = ExpressionFromMethod("MatchWithActivate");
			Assert.AreEqual(
				new ComparisonExpression(
					NewFieldValue(CandidateFieldRoot.Instance, "name", typeof(string)),
					new ConstValue("foo"),
					ComparisonOperator.StartsWith),
				expression);
		}

		public void TestNameEqualsTo()
		{
			IExpression expression = ExpressionFromPredicate(typeof(NameEqualsTo));
			Assert.AreEqual(
				new ComparisonExpression(
				NewFieldValue(CandidateFieldRoot.Instance, "name", typeof(string)),
				NewFieldValue(PredicateFieldRoot.Instance, "_name", typeof(string)),
				ComparisonOperator.ValueEquality),
				expression);
		}

		public void TestHasPreviousWithPrevious()
		{
			// candidate.HasPrevious && candidate.Previous.HasPrevious
			IExpression expression = ExpressionFromPredicate(typeof(HasPreviousWithPrevious));
			IExpression expected = 
				new AndExpression(
				new NotExpression(
				new ComparisonExpression(
				NewFieldValue(CandidateFieldRoot.Instance, "previous", typeof(Data)), 
				new ConstValue(null),
				ComparisonOperator.ValueEquality)),
				new NotExpression(
				new ComparisonExpression(
				NewFieldValue(
					NewFieldValue(CandidateFieldRoot.Instance, "previous", typeof(Data)),
					"previous",
					typeof(Data)),
				new ConstValue(null),
				ComparisonOperator.ValueEquality)));

			Assert.AreEqual(expected, expression);
		}
		
		enum MessagePriority
		{
			None,
			Low,
			Normal,
			High
		}
		
		class Message
		{
			private MessagePriority _priority;

			public MessagePriority Priority
			{
				get { return _priority;  }
				set { _priority = value;  }
			}
		}
		
		private bool MatchEnumConstrain(Message message)
		{
			return message.Priority == MessagePriority.High;
		}
		
		public void TestQueryWithEnumConstrain()
		{
			IExpression expression = ExpressionFromMethod("MatchEnumConstrain");
			IExpression expected = new ComparisonExpression(
				NewFieldValue(CandidateFieldRoot.Instance, "_priority", typeof(MessagePriority)),
				new ConstValue(MessagePriority.High),
				ComparisonOperator.ValueEquality);
			Assert.AreEqual(expected, expression);
		}
		
		private IExpression ExpressionFromMethod(string methodName)
		{
			return new QueryExpressionBuilder().FromMethod(GetType().GetMethod(methodName, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static));
		}

		private IExpression ExpressionFromPredicate(Type type)
		{
			// XXX: move knowledge about IMethodDefinition to QueryExpressionBuilder
			return (new QueryExpressionBuilder()).FromMethod(type.GetMethod("Match"));
		}

		private FieldValue NewFieldValue(IComparisonOperandAnchor anchor, string name, Type type)
		{
			return new FieldValue(anchor,
				new MockFieldRef(name,
					new MockTypeRef(type)));
		}

	}
}
