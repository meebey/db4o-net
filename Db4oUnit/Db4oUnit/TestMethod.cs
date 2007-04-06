using System;
using System.Reflection;
using Db4oUnit;

namespace Db4oUnit
{
	/// <summary>Reflection based db4ounit.Test implementation.</summary>
	/// <remarks>Reflection based db4ounit.Test implementation.</remarks>
	public class TestMethod : ITest
	{
		public interface ILabelProvider
		{
			string GetLabel(TestMethod method);
		}

		private sealed class _AnonymousInnerClass15 : TestMethod.ILabelProvider
		{
			public _AnonymousInnerClass15()
			{
			}

			public string GetLabel(TestMethod method)
			{
				return method.GetSubject().GetType().FullName + "." + method.GetMethod().Name;
			}
		}

		public static TestMethod.ILabelProvider DEFAULT_LABEL_PROVIDER = new _AnonymousInnerClass15
			();

		private readonly object _subject;

		private readonly MethodInfo _method;

		private readonly TestMethod.ILabelProvider _labelProvider;

		public TestMethod(object instance, MethodInfo method) : this(instance, method, DEFAULT_LABEL_PROVIDER
			)
		{
		}

		public TestMethod(object instance, MethodInfo method, TestMethod.ILabelProvider labelProvider
			)
		{
			if (null == instance)
			{
				throw new ArgumentException("instance");
			}
			if (null == method)
			{
				throw new ArgumentException("method");
			}
			if (null == labelProvider)
			{
				throw new ArgumentException("labelProvider");
			}
			_subject = instance;
			_method = method;
			_labelProvider = labelProvider;
		}

		public virtual object GetSubject()
		{
			return _subject;
		}

		public virtual MethodInfo GetMethod()
		{
			return _method;
		}

		public virtual string GetLabel()
		{
			return _labelProvider.GetLabel(this);
		}

		public virtual void Run(TestResult result)
		{
			try
			{
				result.TestStarted(this);
				SetUp();
				Invoke();
			}
			catch (TargetInvocationException e)
			{
				result.TestFailed(this, e.InnerException);
			}
			catch (Exception e)
			{
				result.TestFailed(this, e);
			}
			finally
			{
				try
				{
					TearDown();
				}
				catch (TestException e)
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
			if (_subject is ITestLifeCycle)
			{
				try
				{
					((ITestLifeCycle)_subject).TearDown();
				}
				catch (Exception e)
				{
					throw new TearDownFailureException(e);
				}
			}
		}

		protected virtual void SetUp()
		{
			if (_subject is ITestLifeCycle)
			{
				try
				{
					((ITestLifeCycle)_subject).SetUp();
				}
				catch (Exception e)
				{
					throw new SetupFailureException(e);
				}
			}
		}
	}
}
