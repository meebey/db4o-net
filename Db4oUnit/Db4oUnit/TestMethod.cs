namespace Db4oUnit
{
	/// <summary>Reflection based db4ounit.Test implementation.</summary>
	/// <remarks>Reflection based db4ounit.Test implementation.</remarks>
	public class TestMethod : Db4oUnit.ITest
	{
		public interface ILabelProvider
		{
			string GetLabel(Db4oUnit.TestMethod method);
		}

		private sealed class _AnonymousInnerClass15 : Db4oUnit.TestMethod.ILabelProvider
		{
			public _AnonymousInnerClass15()
			{
			}

			public string GetLabel(Db4oUnit.TestMethod method)
			{
				return method.GetSubject().GetType().FullName + "." + method.GetMethod().Name;
			}
		}

		public static Db4oUnit.TestMethod.ILabelProvider DEFAULT_LABEL_PROVIDER = new _AnonymousInnerClass15
			();

		private readonly object _subject;

		private readonly System.Reflection.MethodInfo _method;

		private readonly Db4oUnit.TestMethod.ILabelProvider _labelProvider;

		public TestMethod(object instance, System.Reflection.MethodInfo method) : this(instance
			, method, DEFAULT_LABEL_PROVIDER)
		{
		}

		public TestMethod(object instance, System.Reflection.MethodInfo method, Db4oUnit.TestMethod.ILabelProvider
			 labelProvider)
		{
			if (null == instance)
			{
				throw new System.ArgumentException("instance");
			}
			if (null == method)
			{
				throw new System.ArgumentException("method");
			}
			if (null == labelProvider)
			{
				throw new System.ArgumentException("labelProvider");
			}
			_subject = instance;
			_method = method;
			_labelProvider = labelProvider;
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
			return _labelProvider.GetLabel(this);
		}

		public virtual void Run(Db4oUnit.TestResult result)
		{
			try
			{
				result.TestStarted(this);
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
