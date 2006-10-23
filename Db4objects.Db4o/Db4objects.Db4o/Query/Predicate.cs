namespace Db4objects.Db4o.Query
{
	/// <summary>Base class for native queries.</summary>
	/// <remarks>
	/// Base class for native queries.
	/// <br /><br />Native Queries allow typesafe, compile-time checked and refactorable
	/// querying, following object-oriented principles. Native Queries expressions
	/// are written as if one or more lines of code would be run against all
	/// instances of a class. A Native Query expression should return true to mark
	/// specific instances as part of the result set.
	/// db4o will  attempt to optimize native query expressions and execute them
	/// against indexes and without instantiating actual objects, where this is
	/// possible.<br /><br />
	/// The syntax of the enclosing object for the native query expression varies,
	/// depending on the language version used. Here are some examples,
	/// how a simple native query will look like in some of the programming languages
	/// and dialects that db4o supports:<br /><br />
	/// <code>
	/// <b>// C# .NET 2.0</b><br />
	/// IList &lt;Cat&gt; cats = db.Query &lt;Cat&gt; (delegate(Cat cat) {<br />
	/// &#160;&#160;&#160;return cat.Name == "Occam";<br />
	/// });<br />
	/// <br />
	/// <br />
	/// <b>// Java JDK 5</b><br />
	/// List &lt;Cat&gt; cats = db.query(new Predicate&lt;Cat&gt;() {<br />
	/// &#160;&#160;&#160;public boolean match(Cat cat) {<br />
	/// &#160;&#160;&#160;&#160;&#160;&#160;return cat.getName().equals("Occam");<br />
	/// &#160;&#160;&#160;}<br />
	/// });<br />
	/// <br />
	/// <br />
	/// <b>// Java JDK 1.2 to 1.4</b><br />
	/// List cats = db.query(new Predicate() {<br />
	/// &#160;&#160;&#160;public boolean match(Cat cat) {<br />
	/// &#160;&#160;&#160;&#160;&#160;&#160;return cat.getName().equals("Occam");<br />
	/// &#160;&#160;&#160;}<br />
	/// });<br />
	/// <br />
	/// <br />
	/// <b>// Java JDK 1.1</b><br />
	/// ObjectSet cats = db.query(new CatOccam());<br />
	/// <br />
	/// public static class CatOccam extends Predicate {<br />
	/// &#160;&#160;&#160;public boolean match(Cat cat) {<br />
	/// &#160;&#160;&#160;&#160;&#160;&#160;return cat.getName().equals("Occam");<br />
	/// &#160;&#160;&#160;}<br />
	/// });<br />
	/// <br />
	/// <br />
	/// <b>// C# .NET 1.1</b><br />
	/// IList cats = db.Query(new CatOccam());<br />
	/// <br />
	/// public class CatOccam : Predicate {<br />
	/// &#160;&#160;&#160;public boolean Match(Cat cat) {<br />
	/// &#160;&#160;&#160;&#160;&#160;&#160;return cat.Name == "Occam";<br />
	/// &#160;&#160;&#160;}<br />
	/// });<br />
	/// </code>
	/// <br />
	/// Summing up the above:<br />
	/// In order to run a Native Query, you can<br />
	/// - use the delegate notation for .NET 2.0.<br />
	/// - extend the Predicate class for all other language dialects<br /><br />
	/// A class that extends Predicate is required to
	/// implement the #match() / #Match() method, following the native query
	/// conventions:<br />
	/// - The name of the method is "#match()" (Java) / "#Match()" (.NET).<br />
	/// - The method must be public public.<br />
	/// - The method returns a boolean.<br />
	/// - The method takes one parameter.<br />
	/// - The Type (.NET) / Class (Java) of the parameter specifies the extent.<br />
	/// - For all instances of the extent that are to be included into the
	/// resultset of the query, the match method should return true. For all
	/// instances that are not to be included, the match method should return
	/// false.<br /><br />
	/// </remarks>
	[System.Serializable]
	public abstract class Predicate
	{
		/// <summary>public for implementation reasons, please ignore.</summary>
		/// <remarks>public for implementation reasons, please ignore.</remarks>
		public static readonly string PREDICATEMETHOD_NAME = "match";

		internal static readonly System.Type OBJECT_CLASS = typeof(object);

		private System.Type _extentType;

		[Db4objects.Db4o.Transient]
		private System.Reflection.MethodInfo cachedFilterMethod = null;

		public Predicate() : this(null)
		{
		}

		public Predicate(System.Type extentType)
		{
			_extentType = extentType;
		}

		internal virtual System.Reflection.MethodInfo GetFilterMethod()
		{
			if (cachedFilterMethod != null)
			{
				return cachedFilterMethod;
			}
			System.Reflection.MethodInfo[] methods = GetType().GetMethods();
			System.Reflection.MethodInfo untypedMethod = null;
			for (int methodIdx = 0; methodIdx < methods.Length; methodIdx++)
			{
				System.Reflection.MethodInfo method = methods[methodIdx];
				if (IsFilterMethod(method))
				{
					if (!OBJECT_CLASS.Equals(Sharpen.Runtime.GetParameterTypes(method)[0]))
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
			throw new System.ArgumentException("Invalid predicate.");
		}

		private bool IsFilterMethod(System.Reflection.MethodInfo method)
		{
			if (Sharpen.Runtime.GetParameterTypes(method).Length != 1)
			{
				return false;
			}
			return Sharpen.Runtime.EqualsIgnoreCase(method.Name, PREDICATEMETHOD_NAME);
			return method.Name.Equals(PREDICATEMETHOD_NAME);
		}

		/// <summary>public for implementation reasons, please ignore.</summary>
		/// <remarks>public for implementation reasons, please ignore.</remarks>
		public virtual System.Type ExtentType()
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
				System.Reflection.MethodInfo filterMethod = GetFilterMethod();
				Db4objects.Db4o.Platform4.SetAccessible(filterMethod);
				object ret = filterMethod.Invoke(this, new object[] { candidate });
				return ((bool)ret);
			}
			catch (System.Exception e)
			{
				return false;
			}
		}
	}
}
