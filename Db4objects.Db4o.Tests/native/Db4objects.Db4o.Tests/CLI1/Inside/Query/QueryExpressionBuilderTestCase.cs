using System;
using System.Reflection;
using Db4objects.Db4o.Nativequery.Expr;
using Db4objects.Db4o.Nativequery.Expr.Cmp;
using Db4objects.Db4o.Nativequery.Expr.Cmp.Field;
using Db4objects.Db4o.Tests.CLI1.NativeQueries;
using Db4objects.Db4o.Tools.NativeQueries;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI1.Inside.Query
{	
	public class QueryExpressionBuilderTestCase : ITestCase
	{
		public void TestNameEqualsTo()
		{
			IExpression expression = ExpressionFromPredicate(typeof(NameEqualsTo));
			Assert.AreEqual(
				new ComparisonExpression(
				new FieldValue(CandidateFieldRoot.INSTANCE, "name"),
				new FieldValue(PredicateFieldRoot.INSTANCE, "_name"),
				ComparisonOperator.EQUALS),
				NullifyTags(expression));
		}
	
		public void TestHasPreviousWithPrevious()
		{
			// candidate.HasPrevious && candidate.Previous.HasPrevious
			IExpression expression = ExpressionFromPredicate(typeof(HasPreviousWithPrevious));
			IExpression expected = 
				new AndExpression(
				new NotExpression(
				new ComparisonExpression(
				new FieldValue(CandidateFieldRoot.INSTANCE, "previous"), 
				new ConstValue(null),
				ComparisonOperator.EQUALS)),
				new NotExpression(
				new ComparisonExpression(
				new FieldValue(
					new FieldValue(CandidateFieldRoot.INSTANCE, "previous"),
					"previous"),
				new ConstValue(null),
				ComparisonOperator.EQUALS)));

			Assert.AreEqual(expected, NullifyTags(expression));
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
				new FieldValue(CandidateFieldRoot.INSTANCE, "_priority"),
				new ConstValue(MessagePriority.High),
				ComparisonOperator.EQUALS);
			Assert.AreEqual(expected, NullifyTags(expression));
		}
		
		class TagNullifier : TraversingExpressionVisitor 
		{
			override public void Visit(FieldValue operand)
			{
				base.Visit(operand);
				operand.Tag(null);
			}
		}

		/// <summary>
		/// Set FieldValue.Tag to null so that Equals ignores it.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		private IExpression NullifyTags(IExpression expression)
		{
			expression.Accept(new TagNullifier());
			return expression;
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
	}
}
