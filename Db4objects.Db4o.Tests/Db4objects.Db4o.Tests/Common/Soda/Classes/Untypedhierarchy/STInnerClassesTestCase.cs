namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy
{
	/// <summary>
	/// epaul:
	/// Shows a bug.
	/// </summary>
	/// <remarks>
	/// epaul:
	/// Shows a bug.
	/// carlrosenberger:
	/// Fixed!
	/// The error was due to the the behaviour of STCompare.java.
	/// It compared the syntetic fields in inner classes also.
	/// I changed the behaviour to neglect all fields that
	/// contain a "$".
	/// </remarks>
	/// <author><a href="mailto:Paul-Ebermann@gmx.de">Paul Ebermann</a></author>
	/// <version>0.1</version>
	public class STInnerClassesTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public class Parent
		{
			public object child;

			public Parent(STInnerClassesTestCase _enclosing, object o)
			{
				this._enclosing = _enclosing;
				this.child = o;
			}

			public override string ToString()
			{
				return "Parent[" + this.child + "]";
			}

			public Parent(STInnerClassesTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly STInnerClassesTestCase _enclosing;
		}

		public class FirstClass
		{
			public object childFirst;

			public FirstClass(STInnerClassesTestCase _enclosing, object o)
			{
				this._enclosing = _enclosing;
				this.childFirst = o;
			}

			public override string ToString()
			{
				return "First[" + this.childFirst + "]";
			}

			public FirstClass(STInnerClassesTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly STInnerClassesTestCase _enclosing;
		}

		public STInnerClassesTestCase()
		{
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STInnerClassesTestCase.Parent
				(this, new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STInnerClassesTestCase.FirstClass
				(this, "Example")), new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STInnerClassesTestCase.Parent
				(this, new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STInnerClassesTestCase.FirstClass
				(this, "no Example")) };
		}

		/// <summary>Only</summary>
		public virtual void TestNothing()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Descend("child");
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(q, _array);
		}
	}
}
