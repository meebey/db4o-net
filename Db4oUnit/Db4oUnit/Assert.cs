namespace Db4oUnit
{
	public sealed class Assert
	{
		public static void Expect(System.Type exception, Db4oUnit.ICodeBlock block)
		{
			try
			{
				block.Run();
			}
			catch (System.Exception e)
			{
				if (exception.IsInstanceOfType(e))
				{
					return;
				}
				Fail("Expecting '" + exception.FullName + "' but got '" + e.GetType().FullName + 
					"'");
			}
			Fail("Exception '" + exception.FullName + "' expected");
		}

		public static void Fail()
		{
			Fail("FAILURE");
		}

		public static void Fail(string msg)
		{
			throw new Db4oUnit.AssertionException(msg);
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
			IsNull(reference, "Expected reference to be null, but was " + reference);
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
			IsNotNull(reference, FailureMessage("not null", reference));
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
			if (expected == actual)
			{
				return;
			}
			Fail(FailureMessage(expected, actual));
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
			if (ObjectsAreEqual(expected, actual))
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
			return FailureMessage(expected, actual, "", customMessage);
		}

		private static string FailureMessage(object expected, object actual, string cmpOper
			, string customMessage)
		{
			return (customMessage == null ? "" : customMessage + ": ") + "Expected " + cmpOper
				 + "'" + expected + "' but was '" + actual + "'";
		}

		private static bool ObjectsAreEqual(object expected, object actual)
		{
			return expected == actual || (expected != null && actual != null && expected.Equals
				(actual));
		}

		public static void IsFalse(bool condition)
		{
			IsTrue(!condition);
		}

		public static void IsFalse(bool condition, string message)
		{
			IsTrue(!condition, message);
		}

		public static void IsInstanceOf(System.Type expectedClass, object actual)
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
	}
}
