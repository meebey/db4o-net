/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;

namespace Db4oUnit
{
	public sealed partial class Assert
	{
		public static Exception Expect(Type exception, ICodeBlock block)
		{
			Exception e = GetThrowable(block);
			AssertThrowable(exception, e);
			return e;
		}

		public static Exception Expect(Type exception, Type cause, ICodeBlock block)
		{
			Exception e = GetThrowable(block);
			AssertThrowable(exception, e);
			AssertThrowable(cause, e.InnerException);
			return e;
		}

		private static void AssertThrowable(Type exception, Exception e)
		{
			if (e == null)
			{
				Fail("Exception '" + exception.FullName + "' expected");
			}
			if (exception.IsInstanceOfType(e))
			{
				return;
			}
			Fail("Expecting '" + exception.FullName + "' but got '" + e.GetType().FullName + 
				"'", e);
		}

		private static Exception GetThrowable(ICodeBlock block)
		{
			try
			{
				block.Run();
			}
			catch (Exception e)
			{
				return e;
			}
			return null;
		}

		public static void Fail()
		{
			Fail("FAILURE");
		}

		public static void Fail(string msg)
		{
			throw new AssertionException(msg);
		}

		public static void Fail(string msg, Exception cause)
		{
			throw new AssertionException(msg, cause);
		}

		public static void IsTrue(bool condition)
		{
			IsTrue(condition, "FAILURE");
		}

		public static void IsTrue(bool condition, string msg)
		{
			if (condition)
			{
				return;
			}
			Fail(msg);
		}

		public static void IsNull(object reference)
		{
			if (reference != null)
			{
				Fail(FailureMessage("null", reference));
			}
		}

		public static void IsNull(object reference, string message)
		{
			if (reference != null)
			{
				Fail(message);
			}
		}

		public static void IsNotNull(object reference)
		{
			if (reference == null)
			{
				Fail(FailureMessage("not null", reference));
			}
		}

		public static void IsNotNull(object reference, string message)
		{
			if (reference == null)
			{
				Fail(message);
			}
		}

		public static void AreEqual(bool expected, bool actual)
		{
			if (expected == actual)
			{
				return;
			}
			Fail(FailureMessage(expected, actual));
		}

		public static void AreEqual(int expected, int actual)
		{
			AreEqual(expected, actual, null);
		}

		public static void AreEqual(int expected, int actual, string message)
		{
			if (expected == actual)
			{
				return;
			}
			Fail(FailureMessage(expected, actual, message));
		}

		public static void AreEqual(double expected, double actual)
		{
			AreEqual(expected, actual, null);
		}

		public static void AreEqual(double expected, double actual, string message)
		{
			if (expected == actual)
			{
				return;
			}
			Fail(FailureMessage(expected, actual, message));
		}

		public static void AreEqual(long expected, long actual)
		{
			if (expected == actual)
			{
				return;
			}
			Fail(FailureMessage(expected, actual));
		}

		public static void AreEqual(object expected, object actual, string message)
		{
			if (Check.ObjectsAreEqual(expected, actual))
			{
				return;
			}
			Fail(FailureMessage(expected, actual, message));
		}

		public static void AreEqual(object expected, object actual)
		{
			AreEqual(expected, actual, null);
		}

		public static void AreSame(object expected, object actual)
		{
			if (expected == actual)
			{
				return;
			}
			Fail(FailureMessage(expected, actual));
		}

		public static void AreNotSame(object expected, object actual)
		{
			if (expected != actual)
			{
				return;
			}
			Fail("Expecting not '" + expected + "'.");
		}

		private static string FailureMessage(object expected, object actual)
		{
			return FailureMessage(expected, actual, null);
		}

		private static string FailureMessage(object expected, object actual, string customMessage
			)
		{
			return FailureMessage(expected, actual, string.Empty, customMessage);
		}

		private static string FailureMessage(object expected, object actual, string cmpOper
			, string customMessage)
		{
			return (customMessage == null ? string.Empty : customMessage + ": ") + "Expected "
				 + cmpOper + "'" + expected + "' but was '" + actual + "'";
		}

		public static void IsFalse(bool condition)
		{
			IsTrue(!condition);
		}

		public static void IsFalse(bool condition, string message)
		{
			IsTrue(!condition, message);
		}

		public static void IsInstanceOf(Type expectedClass, object actual)
		{
			IsTrue(expectedClass.IsInstanceOfType(actual), FailureMessage(expectedClass, actual
				 == null ? null : actual.GetType()));
		}

		public static void IsGreater(long expected, long actual)
		{
			if (actual > expected)
			{
				return;
			}
			Fail(FailureMessage(expected, actual, "greater than ", null));
		}

		public static void IsGreaterOrEqual(long expected, long actual)
		{
			if (actual >= expected)
			{
				return;
			}
			Fail(expected, actual, "greater than or equal to ");
		}

		public static void IsSmaller(long expected, long actual)
		{
			if (actual < expected)
			{
				return;
			}
			Fail(FailureMessage(expected, actual, "smaller than ", null));
		}

		private static void Fail(long expected, long actual, string @operator)
		{
			Fail(FailureMessage(expected, actual, @operator, null));
		}

		public static void AreNotEqual(long expected, long actual)
		{
			if (actual != expected)
			{
				return;
			}
			Fail(expected, actual, "not equal to ");
		}

		public static void AreNotEqual(object notExpected, object actual)
		{
			if (!Check.ObjectsAreEqual(notExpected, actual))
			{
				return;
			}
			Fail("Expecting not '" + notExpected + "'");
		}

		public static void EqualsAndHashcode(object obj, object same, object other)
		{
			AreEqual(obj, obj);
			AreEqual(obj, same);
			AreNotEqual(obj, other);
			AreEqual(obj.GetHashCode(), same.GetHashCode());
			AreEqual(same, obj);
			AreNotEqual(other, obj);
			AreNotEqual(obj, null);
		}
	}
}
