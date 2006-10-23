namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class Visitor4Dispatch : Db4objects.Db4o.Foundation.IVisitor4
	{
		public readonly Db4objects.Db4o.Foundation.IVisitor4 _target;

		public Visitor4Dispatch(Db4objects.Db4o.Foundation.IVisitor4 visitor)
		{
			_target = visitor;
		}

		public virtual void Visit(object a_object)
		{
			((Db4objects.Db4o.Foundation.IVisitor4)a_object).Visit(_target);
		}
	}
}
