/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Reflection;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Query
{
	/// <summary>Base class for native queries.</summary>
	/// <remarks>
	/// Base class for native queries.
	/// &lt;br&gt;&lt;br&gt;Native Queries allow typesafe, compile-time checked and refactorable
	/// querying, following object-oriented principles. Native Queries expressions
	/// are written as if one or more lines of code would be run against all
	/// instances of a class. A Native Query expression should return true to mark
	/// specific instances as part of the result set.
	/// db4o will  attempt to optimize native query expressions and execute them
	/// against indexes and without instantiating actual objects, where this is
	/// possible.&lt;br&gt;&lt;br&gt;
	/// The syntax of the enclosing object for the native query expression varies,
	/// depending on the language version used. Here are some examples,
	/// how a simple native query will look like in some of the programming languages
	/// and dialects that db4o supports:&lt;br&gt;&lt;br&gt;
	/// &lt;code&gt;
	/// &lt;b&gt;// C# .NET 2.0&lt;/b&gt;&lt;br&gt;
	/// IList &lt;Cat&gt; cats = db.Query &lt;Cat&gt; (delegate(Cat cat) {&lt;br&gt;
	/// &#160;&#160;&#160;return cat.Name == "Occam";&lt;br&gt;
	/// });&lt;br&gt;
	/// &lt;br&gt;
	/// &lt;br&gt;
	/// &lt;b&gt;// Java JDK 5&lt;/b&gt;&lt;br&gt;
	/// List &lt;Cat&gt; cats = db.query(new Predicate&lt;Cat&gt;() {&lt;br&gt;
	/// &#160;&#160;&#160;public boolean match(Cat cat) {&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;return cat.getName().equals("Occam");&lt;br&gt;
	/// &#160;&#160;&#160;}&lt;br&gt;
	/// });&lt;br&gt;
	/// &lt;br&gt;
	/// &lt;br&gt;
	/// &lt;b&gt;// Java JDK 1.2 to 1.4&lt;/b&gt;&lt;br&gt;
	/// List cats = db.query(new Predicate() {&lt;br&gt;
	/// &#160;&#160;&#160;public boolean match(Cat cat) {&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;return cat.getName().equals("Occam");&lt;br&gt;
	/// &#160;&#160;&#160;}&lt;br&gt;
	/// });&lt;br&gt;
	/// &lt;br&gt;
	/// &lt;br&gt;
	/// &lt;b&gt;// Java JDK 1.1&lt;/b&gt;&lt;br&gt;
	/// ObjectSet cats = db.query(new CatOccam());&lt;br&gt;
	/// &lt;br&gt;
	/// public static class CatOccam extends Predicate {&lt;br&gt;
	/// &#160;&#160;&#160;public boolean match(Cat cat) {&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;return cat.getName().equals("Occam");&lt;br&gt;
	/// &#160;&#160;&#160;}&lt;br&gt;
	/// });&lt;br&gt;
	/// &lt;br&gt;
	/// &lt;br&gt;
	/// &lt;b&gt;// C# .NET 1.1&lt;/b&gt;&lt;br&gt;
	/// IList cats = db.Query(new CatOccam());&lt;br&gt;
	/// &lt;br&gt;
	/// public class CatOccam : Predicate {&lt;br&gt;
	/// &#160;&#160;&#160;public boolean Match(Cat cat) {&lt;br&gt;
	/// &#160;&#160;&#160;&#160;&#160;&#160;return cat.Name == "Occam";&lt;br&gt;
	/// &#160;&#160;&#160;}&lt;br&gt;
	/// });&lt;br&gt;
	/// &lt;/code&gt;
	/// &lt;br&gt;
	/// Summing up the above:&lt;br&gt;
	/// In order to run a Native Query, you can&lt;br&gt;
	/// - use the delegate notation for .NET 2.0.&lt;br&gt;
	/// - extend the Predicate class for all other language dialects&lt;br&gt;&lt;br&gt;
	/// A class that extends Predicate is required to
	/// implement the #match() method, following the native query
	/// conventions:&lt;br&gt;
	/// - The name of the method is "#match()" (Java).&lt;br&gt;
	/// - The method must be public public.&lt;br&gt;
	/// - The method returns a boolean.&lt;br&gt;
	/// - The method takes one parameter.&lt;br&gt;
	/// - The Type (.NET) / Class (Java) of the parameter specifies the extent.&lt;br&gt;
	/// - For all instances of the extent that are to be included into the
	/// resultset of the query, the match method should return true. For all
	/// instances that are not to be included, the match method should return
	/// false.&lt;br&gt;&lt;br&gt;
	/// </remarks>
	[System.Serializable]
	public abstract class Predicate
	{
		internal static readonly Type ObjectClass = typeof(object);

		private Type _extentType;

		[System.NonSerialized]
		private MethodInfo cachedFilterMethod = null;

		public Predicate() : this(null)
		{
		}

		public Predicate(Type extentType)
		{
			_extentType = extentType;
		}

		// IMPORTANT: must have package visibility because it is used as
		// internal on the .net side
		internal virtual MethodInfo GetFilterMethod()
		{
			if (cachedFilterMethod != null)
			{
				return cachedFilterMethod;
			}
			MethodInfo[] methods = GetType().GetMethods();
			MethodInfo untypedMethod = null;
			for (int methodIdx = 0; methodIdx < methods.Length; methodIdx++)
			{
				MethodInfo method = methods[methodIdx];
				if (PredicatePlatform.IsFilterMethod(method))
				{
					if (!ObjectClass.Equals(Sharpen.Runtime.GetParameterTypes(method)[0]))
					{
						cachedFilterMethod = method;
						return method;
					}
					untypedMethod = method;
				}
			}
			if (untypedMethod != null)
			{
				cachedFilterMethod = untypedMethod;
				return untypedMethod;
			}
			throw new ArgumentException("Invalid predicate.");
		}

		/// <summary>public for implementation reasons, please ignore.</summary>
		/// <remarks>public for implementation reasons, please ignore.</remarks>
		public virtual Type ExtentType()
		{
			return (_extentType != null ? _extentType : Sharpen.Runtime.GetParameterTypes(GetFilterMethod
				())[0]);
		}

		/// <summary>public for implementation reasons, please ignore.</summary>
		/// <remarks>public for implementation reasons, please ignore.</remarks>
		public virtual bool AppliesTo(object candidate)
		{
			try
			{
				MethodInfo filterMethod = GetFilterMethod();
				Platform4.SetAccessible(filterMethod);
				object ret = filterMethod.Invoke(this, new object[] { candidate });
				return ((bool)ret);
			}
			catch (Exception)
			{
				// FIXME: Exceptions should be logged for app developers,
				// but we can't print them out here.
				// e.printStackTrace();
				return false;
			}
		}
	}
}
