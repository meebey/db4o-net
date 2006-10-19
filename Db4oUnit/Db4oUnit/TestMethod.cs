namespace Db4oUnit
{
	/// <summary>Reflection based db4ounit.Test implementation.</summary>
	/// <remarks>Reflection based db4ounit.Test implementation.</remarks>
	public class TestMethod : Db4oUnit.ITest
	{
		public static string CreateLabel(object subject, System.Reflection.MethodInfo method
			)
		{
			return subject.GetType().FullName + "." + method.Name;
		}

		private object _subject;

		private System.Reflection.MethodInfo _method;

		public TestMethod(object instance, System.Reflection.MethodInfo method)
		{
			if (null == instance)
			{
				throw new System.ArgumentException("instance");
			}
			if (null == method)
			{
				throw new System.ArgumentException("method");
			}
			_subject = instance;
			_method = method;
		}

		public virtual object GetSubject()
		{
			return _subject;
		}

		public virtual System.Reflection.MethodInfo GetMethod()
		{
			return _method;
		}

		public virtual string GetLabel()
		{
			return CreateLabel(_subject, _method);
		}

		public virtual void Run(Db4oUnit.TestResult result)
		{
			result.TestStarted();
			try
			{
				SetUp();
				Invoke();
			}
			catch (System.Reflection.TargetInvocationException e)
			{
				result.TestFailed(this, e.InnerException);
			}
			catch (System.Exception e)
			{
				result.TestFailed(this, e);
			}
			finally
			{
				try
				{
					TearDown();
				}
				catch (Db4oUnit.TestException e)
				{
					result.TestFailed(this, e);
				}
			}
		}

		protected virtual void Invoke()
		{
			_method.Invoke(_subject, new object[0]);
		}

		protected virtual void TearDown()
		{
			if (_subject is Db4oUnit.ITestLifeCycle)
			{
				try
				{
					((Db4oUnit.ITestLifeCycle)_subject).TearDown();
				}
				catch (System.Exception e)
				{
					throw new Db4oUnit.TearDownFailureException(e);
				}
			}
		}

		protected virtual void SetUp()
		{
			if (_subject is Db4oUnit.ITestLifeCycle)
			{
				try
				{
					((Db4oUnit.ITestLifeCycle)_subject).SetUp();
				}
				catch (System.Exception e)
				{
					throw new Db4oUnit.SetupFailureException(e);
				}
			}
		}
	}
}
